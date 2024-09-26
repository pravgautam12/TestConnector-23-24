using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Diagnostics;

namespace TestConnector2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option0Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a number:", "Input Number", "0");

            if (double.TryParse(input, out double number))
            {
                Application.totalArea = number;
                Application.overRide = true;
                return Result.Succeeded;
            }
            else
            {
                TaskDialog.Show("Invalid Input", "The input is not a valid number. Reverting to default area.");
                Application.overRide = false;
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option1Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.75;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.75");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option2Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.64;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.64");
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option3Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.79;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.79");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option4Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.80;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.80");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option5Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.76;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.76");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option6Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.71;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.71");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option7Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.53;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.53");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option8Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.72;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.72");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option9Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.56;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.56");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option10Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.81;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.81");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option11Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.96;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.96");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option12Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.83;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.83");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option13Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.82;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.82");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option14Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.44;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.44");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option15Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.45;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.45");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option16Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.55;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.55");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option17Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.18;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.18");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option18Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.69;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.69");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option19Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.84;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.84");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option20Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.66;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.66");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option21Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.65;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.65");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option22Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.67;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.67");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option23Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.50;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.50");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Option24Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application.SelectedValue = 0.91;
            TaskDialog.Show("Room Selected", "Allowed Watts per Square Foot: 0.91");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class ComCheck : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            List<Element> rooms = new List<Element>();
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            List<Element> filteredLightingFixtures = null;

            if (doc == null)
            {
                TaskDialog.Show("Error", "Active document is null.");
            }

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FilteredElementCollector light_fixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_LightingFixtures).WhereElementIsNotElementType();

            ICollection<Element> linkInstances = collector.OfClass(typeof(RevitLinkInstance)).ToElements();
            ICollection<Element> lightingFixtures = light_fixtures.ToElements();

            string worksetNameToExclude = "ELEC - SITE";
            WorksetId worksetIdToExclude = null;

            foreach (Workset workset in new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset))
            {
                if (workset.Name.Equals(worksetNameToExclude))
                {
                    worksetIdToExclude = workset.Id;
                    break;
                }
            }

            string resultMessage = "";
            double apparent_load = 0.0;

            filteredLightingFixtures = lightingFixtures
                .Where(fixture => fixture.WorksetId != worksetIdToExclude)
                .ToList();

            if (linkInstances == null || linkInstances.Count == 0)
            {
                TaskDialog.Show("Info", "No linked instances found.");
            }

            if (filteredLightingFixtures.Count == 0)
            {
                TaskDialog.Show("Info", "No Fixtures Found. Check Active View.\n");
            }

            else
            {
                foreach (Element fixture in filteredLightingFixtures)
                {

                    FamilyInstance x = fixture as FamilyInstance;

                    FamilySymbol symbol = x.Symbol;

                    Parameter wattageParam = symbol.LookupParameter("AUTOMETICA_APPARENT LOAD");

                    string type_mark = symbol.LookupParameter("Type Mark").AsString();

                    if (wattageParam != null)
                    {
                        string wattage = wattageParam.AsValueString();
                        string[] wattageParts = wattage.Split(' ');
                        string wattageValue = wattageParts[0];
                        int wattage1 = int.Parse(wattageValue);
                        apparent_load += wattage1;
                    }
                    else
                    {
                        Parameter wattageParam1 = x.LookupParameter("AUTOMETICA_APPARENT LOAD");

                        if (wattageParam1 != null)
                        {
                            string wattage2 = wattageParam1.AsValueString();
                            string[] wattageParts1 = wattage2.Split(' ');
                            string wattageValue1 = wattageParts1[0];
                            int wattage3 = int.Parse(wattageValue1);
                            apparent_load += wattage3;
                        }

                        else
                        {
                            resultMessage += $"Element Id: {type_mark}, Wattage not found. Please make sure wattages are specified for each light placed in the model.\n";
                            TaskDialog.Show("Lighting Fixtures Wattage", resultMessage);
                            break;
                        }
                    }
                }

                foreach (RevitLinkInstance linkInstance in linkInstances)
                {
                    Document linkedDoc = linkInstance.GetLinkDocument();
                    if (linkedDoc != null)
                    {
                        FilteredElementCollector linkedCollector = new FilteredElementCollector(linkedDoc);
                        ICollection<Element> linkedRooms = linkedCollector.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToElements();

                        foreach (Room r in linkedRooms)
                        {
                            rooms.Add(r);
                        }
                    }
                }

                rooms.RemoveAll(room => room.Name.Contains(" B"));

                if (!Application.overRide)
                {
                    Application.totalArea = 0.0;
                    foreach (Room r in rooms)
                    {
                        Application.totalArea += r.Area;
                    }
                }

                double wattsPerSF = (int)(Application.totalArea * Application.SelectedValue);
                double percentDiff = ((wattsPerSF - apparent_load) / wattsPerSF) * 100;
                string formattedPercentDiff = percentDiff.ToString("F2");
                string formattedArea = Application.totalArea.ToString("F0");

                if (apparent_load >= wattsPerSF)
                {
                    TaskDialog.Show("ComCheck", $"Square Footage = {formattedArea}\nAllowed Wattage = {wattsPerSF}\nProposed Wattage = {apparent_load}\nFailed by {formattedPercentDiff}%");
                }

                else
                {
                    TaskDialog.Show("ComCheck", $"Square Footage = {formattedArea}\nAllowed Wattage = {wattsPerSF}\nProposed Wattage = {apparent_load}\nPassed by {formattedPercentDiff}%");
                }
            }

            return Result.Succeeded;
        }
    }
}