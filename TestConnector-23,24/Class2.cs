using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;
using static TestConnector2.Class2;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;




namespace TestConnector2
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]


    public class Class2 : IExternalCommand


    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            IList<Element> template = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_BranchPanelScheduleTemplates).ToElements();
            PanelScheduleTemplate temp = null;

            foreach (PanelScheduleTemplate t in template)
            {
                if (t.Name == "Branch Panel - TI Template")
                {
                    temp = t;
                    break;
                }
            }

            IList<Element> view = new FilteredElementCollector(doc).OfClass(typeof(PanelScheduleView)).ToElements();

            foreach (PanelScheduleView v in view)
            {


                Transaction trans = new Transaction(doc, "Changing Template");
                trans.Start();

                try
                {
                    v.GenerateInstanceFromTemplate(temp.Id);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.RollBack();
                    continue;
                }

            }
            return Result.Succeeded;
        }

        public static PanelScheduleTemplate GetTemplate(Document doc, string templateName)
        {
            IList<Element> templateList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_BranchPanelScheduleTemplates).ToElements();
            PanelScheduleTemplate temp = null;

            foreach (PanelScheduleTemplate t in templateList)
            {
                if (t.Name == templateName)
                {
                    temp = t;
                    break;
                }
            }
            return temp;
        }

        public static void ChangeTemplate(Document doc, string templateName)
        {
            PanelScheduleTemplate t = GetTemplate(doc, templateName);

            IList<Element> view = new FilteredElementCollector(doc).OfClass(typeof(PanelScheduleView)).ToElements();

            if (t != null)
            {
                foreach (PanelScheduleView v in view)
                {


                    Transaction trans = new Transaction(doc, "Changing Template");
                    trans.Start();

                    try
                    {
                        v.GenerateInstanceFromTemplate(t.Id);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.RollBack();
                        TaskDialog.Show("DIDN't WORK", "Hey fool, did you enter the right template name? CHECK!");
                        continue;
                    }

                }
            }


        }
        public class FamilyLoadOptions : IFamilyLoadOptions
        {
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                // Set the behavior for when a family is found
                overwriteParameterValues = true; // Specify whether to overwrite parameter values

                // Return true to continue loading the family, or false to cancel loading
                return true;
            }

            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                throw new NotImplementedException();
            }
        }

    }

}