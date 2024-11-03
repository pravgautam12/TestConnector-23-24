using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;

namespace TestConnector2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    internal class TemplateChange: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //getting the current Revit process
            IntPtr revitMainWindowHandle = SetWindowHandle.revitMainWindowHandle;
            if (revitMainWindowHandle == IntPtr.Zero)
            {
                TaskDialog.Show("Error", "Revit window not found");
            }

            ChangeTemplateW changeTemplateWindow = new ChangeTemplateW(commandData);
            changeTemplateWindow.Show();

            return Result.Succeeded;
        }
    }

    public class ChangeTemplatesEventHandler : IExternalEventHandler
    {
        public List<TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML.Panel> checkedItemsList;
        public PanelScheduleTemplate desiredTemplate;

        public void SetData(List<TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML.Panel> _checkedItemsList, PanelScheduleTemplate _desiredTemplate)
        {
            checkedItemsList = _checkedItemsList;
            desiredTemplate = _desiredTemplate;
        }

        public void Execute(UIApplication application)
        {
            foreach (TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML.Panel p in checkedItemsList) 
            {
                Element panelWhoseTemplateNeedsToChange = MoveCircuitsProperties.panelDictionary[p.PanelName];

                PanelScheduleView associatedView = new FilteredElementCollector(DocumentInstance.Instance).OfClass(typeof(PanelScheduleView)).Cast<PanelScheduleView>().FirstOrDefault(
                    view => view.GetPanel() == panelWhoseTemplateNeedsToChange.Id);

                Transaction trans = new Transaction(DocumentInstance.Instance, "Changing Template");
                trans.Start();

                try
                {
                    associatedView.GenerateInstanceFromTemplate(desiredTemplate.Id);
                    trans.Commit();
                }
                catch { TaskDialog.Show("Error", "Unable to change template."); }
            }

        }

        public string GetName() => "External Event Handler";


    }




}
