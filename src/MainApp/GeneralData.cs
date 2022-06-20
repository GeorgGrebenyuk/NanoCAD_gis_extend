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

namespace NanoCAD_GIS
{
    /// <summary>
    /// Static class as container for constants
    /// </summary>
    public static class GeneralData
    {
        public static Database nc_db;
        public static Document nc_doc;
        public static string path_to_db = $@"C:\Users\{Environment.UserName}\AppData\Local\proj\proj.db";
        public static string CS_code = null;
    }
    public static class Tools
    {
        public static ILibraryImport for_recalc_data;
        public static void GettingUserInput (string FirstMessage, ref string OutPutInfo)
        {
            PromptStringOptions opts = new PromptStringOptions(FirstMessage);
            opts.AllowSpaces = false;
            PromptResult pr = GeneralData.nc_doc.Editor.GetString(opts);
            if (PromptStatus.OK == pr.Status)
            {
                OutPutInfo = pr.StringResult;
            }
            else
            {
                GeneralData.nc_doc.Editor.WriteMessage("Повторите еще раз");
                GettingUserInput(FirstMessage, ref OutPutInfo);
            }

        }

    }
}
