using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Teigha.DatabaseServices;
using Teigha.Runtime;
using Teigha.Geometry;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Platform = HostMgd;
using PlatformDb = Teigha;

using static NanoCAD_GIS.GeneralData;
namespace NanoCAD_GIS
{
    public class CS_Identify
    {
        public CS_Identify()
        {
            //Check layers table -> if exists return that code;
            //If not exists - run a dialog box window to select CS and assigned it to drawing
            if (check_layer_table()) //Если там есть запись - то выходим из метода
            {
                PromptStringOptions opts = new PromptStringOptions($"Система координат уже установлена - {CS_code}. " +
                    $"Если хотите заменить введите ДА, если хотите оставить - введите НЕТ");
                opts.AllowSpaces = true;
                PromptResult pr = nc_doc.Editor.GetString(opts);
                if (PromptStatus.OK == pr.Status && pr.StringResult == "ДА")
                {
                    string to_assign = select_cs();
                    CS_code = to_assign;
                    assign_to_layer_table(to_assign);
                }
                else if (PromptStatus.OK == pr.Status && pr.StringResult == "НЕТ")
                {
                    //выход
                }
                else
                {
                    nc_doc.Editor.WriteMessage("Не удалось распознать ввод. Повторите команду смены СК еще раз");
                }
            }
            else //Если записи нет - то вызываем выбор
            {
                string to_assign = select_cs();
                CS_code = to_assign;
                assign_to_layer_table(to_assign);
            }
        }
        private bool check_layer_table()
        {
            using (DocumentLock acDocLock = nc_doc.LockDocument())
            {
                using (Transaction acTrans = nc_db.TransactionManager.StartTransaction())
                {
                    ObjectId lt_id = nc_db.LayerTableId;
                    LayerTable lt = acTrans.GetObject(lt_id, OpenMode.ForRead) as LayerTable;

                    foreach (ObjectId layerId in lt)
                    {
                        LayerTableRecord layer = acTrans.
                            GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        if (layer.Name == "0")
                        {
                            string descr = null;
                            try
                            {
                                descr = layer.Description;
                            }
                            catch
                            {

                            }
                            if (descr != null && descr.Length > 3) 
                            {
                                //Как-то проверить еще валидность этого комментария
                                CS_code = descr;
                                
                            }
                            break;
                        }
                    }

                    acTrans.Commit();
                }

            }
            if (CS_code != null) return true;
            else return false;
        }
        private string select_cs()
        {
            PromptStringOptions opts = new PromptStringOptions("Введите код СК в форме числового значения EPSG:");
            opts.AllowSpaces = false;
            PromptResult pr = nc_doc.Editor.GetString(opts);
            if (PromptStatus.OK == pr.Status)
            {
                return pr.StringResult;
            }
            else
            {
                return select_cs();
            }
            //return new CS_Load().selected_code;

        }
        private void assign_to_layer_table(string selected_cs)
        {
            using (DocumentLock acDocLock = nc_doc.LockDocument())
            {
                using (Transaction acTrans = nc_db.TransactionManager.StartTransaction())
                {
                    ObjectId lt_id = nc_db.LayerTableId;
                    LayerTable lt = acTrans.GetObject(lt_id, OpenMode.ForRead) as LayerTable;

                    foreach (ObjectId layerId in lt)
                    {
                        LayerTableRecord layer = acTrans.
                            GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        if (layer.Name == "0")
                        {
                            layer.Description = selected_cs;
                            break;
                        }
                    }

                    acTrans.Commit();
                }

            }

        }
        public string finded_cs = null;
    }
}
