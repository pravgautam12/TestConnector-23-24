using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Interop;
using System;
using TestConnector2.Electrical_Connectors_and_Parameters.Views.COMcheckXAML;

namespace TestConnector2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RunCOMcheck : IExternalCommand
    {
        public static Document CurrentDocument { get; set; }
        public Result Execute(ExternalCommandData cmdData, ref string message, ElementSet Elements)
        {
            CurrentDocument = cmdData.Application.ActiveUIDocument.Document;

            IntPtr revitMainWindowHandle = Process.GetProcessesByName("Revit").FirstOrDefault()?.MainWindowHandle ?? IntPtr.Zero;

            if (revitMainWindowHandle == IntPtr.Zero)
            {
                TaskDialog.Show("Error", "Revit window not found");
            }

            Dictionary<string, List<string>> stateAndCities = CommonProps.SetStateCities();


            COMcheckW window = new COMcheckW();
            window.Show();

            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = revitMainWindowHandle;

            return Result.Succeeded;
        }
    }
}