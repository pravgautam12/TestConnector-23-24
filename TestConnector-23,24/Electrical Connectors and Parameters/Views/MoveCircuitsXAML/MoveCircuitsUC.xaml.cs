using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Autodesk.Revit.DB.Electrical;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.MoveCircuitsXAML
{
    /// <summary>
    /// Interaction logic for MoveCircuitsUC.xaml
    /// </summary>
    public partial class MoveCircuitsUC : UserControl
    {
        private ExternalCommandData _commandData;
        private ExternalEvent _externalEvent;
        private MoveCircuitsEventHandler handler;
        public MoveCircuitsUC MoveCircuitsInstance { get; set; }
        private List<ElectricalSystem> elecSys;
        private Element panel;
        public MoveCircuitsUC(MoveCircuitsViewModel viewModel, ExternalCommandData commandData)
        {
            InitializeComponent();
            DataContext = viewModel;
            MoveCircuitsInstance = this;
            _commandData = commandData;
            handler = new MoveCircuitsEventHandler();
            _externalEvent = ExternalEvent.Create(handler);
        }

        private void MoveCircuits(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItemOfMoveFromPanel = null;
            ComboBoxItem selectedItemOfMoveToPanel = null;
            var viewModel = DataContext as MoveCircuitsViewModel;
            string moveFromPanelString = "";
            string moveToPanelString = "";
            try
            {
                selectedItemOfMoveFromPanel = MoveCircuitsInstance.moveFromPanel.SelectedItem as ComboBoxItem;
            }
            catch { }

            try { moveFromPanelString = selectedItemOfMoveFromPanel.Content.ToString(); } catch { }


            try { selectedItemOfMoveToPanel = (MoveCircuitsInstance.moveToPanel.SelectedItem as ComboBoxItem); } catch { }

            if (selectedItemOfMoveFromPanel == null && selectedItemOfMoveToPanel == null)
            {
                TaskDialog.Show("Error", "Please select the panels you would like to move the circuits to and from.");
                return;
            }

            if (selectedItemOfMoveToPanel != null && selectedItemOfMoveFromPanel == null)
            {
                TaskDialog.Show("Error", "Please select the panel you would like to move the circuits from.");
                return;
            }

            if (selectedItemOfMoveToPanel == null && selectedItemOfMoveFromPanel != null)
            {
                TaskDialog.Show("Error", "Please select the panel you would like to move the circuits to.");
                return;
            }

           
            try { moveToPanelString = selectedItemOfMoveToPanel.Content.ToString(); } catch { }


            FamilyInstance baseEquipmentPanel = null;
            List<CircuitItem> checkedCircuitItems = viewModel.GetCheckedItems();
            List<ElectricalSystem> circuitsToBeMoved = new List<ElectricalSystem>();

            if (checkedCircuitItems.Count == 0)
            {
                TaskDialog.Show("Error", "Please select the circuits you would like to move.");
                return;
            }

            foreach (CircuitItem circuitItem in checkedCircuitItems)
            {
                ElectricalSystem elecSys = MoveCircuitsProperties.elecSysDictionary[circuitItem.CircuitNumber];
                circuitsToBeMoved.Add(elecSys);
                baseEquipmentPanel = elecSys.BaseEquipment;
            }

            Element moveToPanel = MoveCircuitsProperties.panelDictionary[moveToPanelString];
            handler.SetData(circuitsToBeMoved, moveToPanel, viewModel.OddCircuitItems, viewModel.EvenCircuitItems, viewModel);
            _externalEvent.Raise();
        }
    }
}
