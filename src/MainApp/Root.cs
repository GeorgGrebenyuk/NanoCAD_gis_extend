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
using static NanoCAD_GIS.Tools;
namespace NanoCAD_GIS
{
    /// <summary>
    /// Здесь будет реализзация загрузки методов в среду NanoCAD
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Общий метод для фиксации информации о проекте - сущности Документа и прочем (чего?...)
        /// </summary>
        public void GettingProjectInfo()
        {
            nc_doc = Platform.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            nc_db = nc_doc.Database;
            
        }
        [CommandMethod("NCGIS_ASSIGN_CS")]
        public void Identify_cs ()
        {
            GettingProjectInfo();
            CS_Identify identidy_coord_system = new NanoCAD_GIS.CS_Identify();
            //CS_code = identidy_coord_system.finded_cs;
        }
        [CommandMethod("NCGIS_EXPORT_POINTS_CSV")]
        public void export_points_coords()
        {
            string user_cs_target = null;
            Tools.GettingUserInput("Укажите целевую СК", ref user_cs_target);
            new CS_Actions.CS_Actions("test", user_cs_target);

        }


    }
}

