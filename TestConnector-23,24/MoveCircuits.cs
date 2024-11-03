using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using TestConnector2.Electrical_Connectors_and_Parameters.Views.MoveCircuitsXAML;
using System.Windows.Interop;

namespace TestConnector2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class MoveCircuits : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            IntPtr revitMainWindowHandle = SetWindowHandle.revitMainWindowHandle;

            if (revitMainWindowHandle == IntPtr.Zero)
            {
                TaskDialog.Show("Error", "Revit window not found");
            }

            MoveCircuitsW moveCircuits = new MoveCircuitsW(commandData);
            moveCircuits.Show();

            WindowInteropHelper helper = new WindowInteropHelper(moveCircuits);
            helper.Owner = revitMainWindowHandle;

            return Result.Succeeded;
        }
    }

    public class MoveCircuitsEventHandler : IExternalEventHandler
    {
        private List<ElectricalSystem> elecSysList;
        private Element panel;
        private ObservableCollection<CircuitItem> oddItems;
        private ObservableCollection<CircuitItem> evenItems;
        private MoveCircuitsViewModel viewModel;
        public void SetData(List<ElectricalSystem> electricalSystemsList, Element constructorPanel, ObservableCollection<CircuitItem> odd,
            ObservableCollection<CircuitItem> even, MoveCircuitsViewModel vm)
        {
            elecSysList = electricalSystemsList;
            panel = constructorPanel;
            oddItems = odd;
            evenItems = even;
            viewModel = vm;
        }

        public void Execute(UIApplication application)
        {
            string message = "";
            Transaction transaction = new Transaction(DocumentInstance.Instance, "moving circuits");
            transaction.Start();
            foreach (ElectricalSystem eS in elecSysList)
            {
                bool wasCircuitMovedSuccessfully = false;
                try { eS.SelectPanel(panel as FamilyInstance); wasCircuitMovedSuccessfully = true; } 
                catch 
                {
                    message += eS.BaseEquipment.Name + '-' + eS.CircuitNumber.Replace(",", "/") + ", ";
                }

                CircuitItem itemToBeRemoved = null;
                if (wasCircuitMovedSuccessfully)
                {
                    itemToBeRemoved = viewModel.OddCircuitItems.FirstOrDefault(item => item.CircuitNumber == eS.CircuitNumber);
                    if (itemToBeRemoved != null)
                    {
                        try { viewModel.OddCircuitItems.Remove(itemToBeRemoved); } catch { }
                    }
                    else
                    {
                        itemToBeRemoved = viewModel.EvenCircuitItems.FirstOrDefault(item => item.CircuitNumber == eS.CircuitNumber);

                        try { viewModel.EvenCircuitItems.Remove(itemToBeRemoved); } catch { }
                    }

                }
            }

            if (message != "")
            {
                string newMsg = message.Remove(message.Length - 1);
                newMsg = newMsg.Remove(newMsg.Length - 1);
                TaskDialog.Show("Could not move", $"The following circuit(s) could not be moved: {newMsg}");
            }

            transaction.Commit();
        }
        public string GetName() => "External Event Handler";
    }
}
