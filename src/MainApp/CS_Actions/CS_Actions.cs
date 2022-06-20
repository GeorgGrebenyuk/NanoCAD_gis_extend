using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Teigha.DatabaseServices;
using Teigha.Runtime;
using Teigha.Geometry;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Platform = HostMgd;
using PlatformDb = Teigha;


using static NanoCAD_GIS.GeneralData;
using static NanoCAD_GIS.Tools;
using System.Windows.Forms;

namespace NanoCAD_GIS.CS_Actions
{
    public class CS_Actions
    {
        string target_cs = null;
        public CS_Actions(string type, string target_cs_input)
        {
            this.target_cs = target_cs_input;
            switch (type)
            {
                default:
                    test();
                    break;

            }
        }
        void test()
        {
            string layer_name = null;
            Tools.GettingUserInput("Выберите слой, с которого надо эжкспортировать данные", ref layer_name);
            string save_dir = null;
            select_path();
            void select_path()
            {
                FolderBrowserDialog select_dir = new FolderBrowserDialog();
                save_dir = select_dir.SelectedPath;
                if (select_dir.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(select_dir.SelectedPath))
                    {
                        save_dir = select_dir.SelectedPath;
                    }
                    else
                    {
                        nc_doc.Editor.WriteMessage("Неверно выбрана папка. Повторите выбор!");
                        select_path();
                    }
                }
                else
                {
                    nc_doc.Editor.WriteMessage("Ошибка! Повторите выбор!");
                    select_path();

                }

            }
            //Получаем коллекцию точек
            List<string> recal_result = new List<string>();
            List<Point3d> currents_pos = new List<Point3d>();
            using (DocumentLock acLckDoc = nc_doc.LockDocument())
            {
                using (Transaction acTrans = nc_db.TransactionManager.StartTransaction())
                {
                    TypedValue[] acTypValAr = new TypedValue[2];
                    acTypValAr.SetValue(new TypedValue((int)DxfCode.LayerName, layer_name), 0);
                    acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "POINT"), 1);
                    SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
                    PromptSelectionResult acSSPrompt = nc_doc.Editor.SelectAll(acSelFtr);
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        ObjectId[] ids = acSSPrompt.Value.GetObjectIds();
                        foreach (ObjectId one_id in ids)
                        {
                            Entity ent = acTrans.GetObject(one_id, OpenMode.ForWrite) as Entity;
                            
                            DBPoint model_point = acTrans.GetObject(one_id, OpenMode.ForWrite) as DBPoint;
                            
                            ;
                            currents_pos.Add(model_point.Position);

                            
                        }
                    }
                }
            }
            //ILibraryImport recalc_data = LibraryImport.Select();
            foreach (Point3d p in currents_pos)
            {
                point recalced_point = Tools.for_recalc_data.crs2crs_tranform(CS_code.ToCharArray(), this.target_cs.ToCharArray(),
                                p.X, p.Y, p.Z);
                recal_result.Add($"{recalced_point.x}    {recalced_point.y}");
            }
            File.WriteAllText(save_dir + $"\\result_recalc_{Guid.NewGuid()}.txt", String.Join("\r\n", recal_result));

        }
    }
}
