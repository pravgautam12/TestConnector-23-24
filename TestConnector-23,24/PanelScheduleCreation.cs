using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Electrical;
using System.Collections.Generic;


namespace TestConnector2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PanelScheduleCreation : IExternalCommand
    {

        private bool IsAPanelBoard(Element elem)
        {
            FamilyInstance panelInstance = elem as FamilyInstance;

            if (panelInstance == null) return false;

            var distParam = panelInstance.LookupParameter("Distribution System") ?? panelInstance.Symbol.LookupParameter("Distribution System");
            var secDistParam = panelInstance.LookupParameter("Secondary Distribution System") ?? panelInstance.Symbol.LookupParameter("Secondary Distribution System");
            var mainsParam = panelInstance.LookupParameter("Mains Type") ?? panelInstance.Symbol.LookupParameter("Mains Type");

            return distParam != null && secDistParam == null && mainsParam != null;
        }

        public Result Execute(
            ExternalCommandData commandData,
        ref string message,
            ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;

            // make a string to filter by family name
            string familyName = "208V Lighting and Appliance Panelboard";

            // Start a new read-only transaction
            using (Transaction trans = new Transaction(doc, "Count Panels"))
            {
                trans.Start();

                IList<Element> panels = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                    .WhereElementIsNotElementType().Where(e => IsAPanelBoard(e)).ToList();


                // Count the panels
                int panelCount = panels.Count;
                int j = 0;
                for (int i = 0; i < panelCount; i++)
                {
                    if (!PanelScheduleExists(doc, panels[i].Id))
                    {
                        PanelScheduleView.CreateInstanceView(doc, panels[i].Id);
                        j++;
                    }

                }
                // Show the count in a message box
                TaskDialog.Show("Success", $"Created panel schedules for {j} panels in the model.");

                trans.Commit();
            }

            return Result.Succeeded;
        }
        private bool PanelScheduleExists(Document doc, ElementId panelId)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(PanelScheduleView));

            //return collector
            //    .Cast<FamilyInstance>()
            //    .Any(fs => fs.Id == panelId);

            return collector.Cast<PanelScheduleView>()
                .Any(fs => fs.GetPanel() == panelId);
        }
    }
}