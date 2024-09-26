using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConnector2
{
    public static class DocumentInstance
    {
        public static Document Instance { get; set; }
    }

    public static class uiApplication
    {
        public static UIApplication UiApp { get; set; }
    }
}
