using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.MoveCircuitsXAML
{
    /// <summary>
    /// Interaction logic for MoveCircuitsW.xaml
    /// </summary>
    public partial class MoveCircuitsW : Window
    {
        private ObservableCollection<CircuitItem> oddItems;
        private int? _lastSelectedIndex = null;
        

        public Dictionary<string, ElectricalSystem> windowDictionaryOfElectricalSystems = new Dictionary<string, ElectricalSystem>();
        Dictionary<string, Element> panelDictionary = new Dictionary<string, Element>();
        private MoveCircuitsViewModel viewModel = new MoveCircuitsViewModel();
        private MoveCircuitsUC moveCircuitsUC;
        public MoveCircuitsW(ExternalCommandData commandData)
        {
            InitializeComponent();

            viewModel = new MoveCircuitsViewModel();
            DataContext = viewModel;
            moveCircuitsUC = new MoveCircuitsUC(viewModel, commandData);
            MoveCircuitsGrid.Children.Add(moveCircuitsUC);
            Document doc = DocumentInstance.Instance;
            PanelScheduleCreation panelScheduleCreation = new PanelScheduleCreation();
            IList<Element> panels = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                    .WhereElementIsNotElementType().Where(e => panelScheduleCreation.IsAPanelBoard(e)).ToList();

            moveCircuitsUC.moveFromPanel.IsEditable = true; moveCircuitsUC.moveFromPanel.IsReadOnly = true; moveCircuitsUC.moveFromPanel.Focusable = false;
            moveCircuitsUC.moveFromPanel.Text = "Move From";

            moveCircuitsUC.moveToPanel.IsEditable = true; moveCircuitsUC.moveToPanel.IsReadOnly = true; moveCircuitsUC.moveToPanel.Focusable = true;
            moveCircuitsUC.moveToPanel.Text = "Move To";

            foreach (Element panel in panels)
            {
                string panelName = "";
                try { panelName = (panel as FamilyInstance).LookupParameter("Panel Name").AsString(); } catch { }

                if (panelName != "")
                {
                    try { panelDictionary.Add(panelName, panel); } catch { }
                }
            }

            foreach (Element panel in panels)
            {
                FamilyInstance panelFamIns = panel as FamilyInstance;
                if (panelFamIns != null) 
                {
                    string panelName = "";
                    try { panelName = panelFamIns.LookupParameter("Panel Name").AsString(); } catch { }
                    
                    if (panelName == "")
                    {
                        moveCircuitsUC.moveFromPanel.Items.Add(new ComboBoxItem { Content = panelFamIns.Name });
                        moveCircuitsUC.moveToPanel.Items.Add(new ComboBoxItem { Content = panelFamIns.Name});
                    }

                    if (panelName != "")
                    {
                        moveCircuitsUC.moveFromPanel.Items.Add(new ComboBoxItem { Content = panelName});
                        moveCircuitsUC.moveToPanel.Items.Add(new ComboBoxItem { Content = panelName });
                    }
                }
            }

            moveCircuitsUC.moveFromPanel.SelectionChanged += SelectedPanelName_SelectionChanged;
            MoveCircuitsProperties.panelDictionary = panelDictionary;

            
            moveCircuitsUC.OddCircuitList.PreviewMouseDown += OddListBox_MouseDown;
            moveCircuitsUC.EvenCircuitList.PreviewMouseDown += OddListBox_MouseDown;
        }

        private void SelectedPanelName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.OddCircuitItems.Clear();
            viewModel.EvenCircuitItems.Clear();
            windowDictionaryOfElectricalSystems.Clear();
            MoveCircuitsProperties.elecSysDictionary.Clear();
            ComboBoxItem selectedItem = moveCircuitsUC.moveFromPanel.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string content = selectedItem.Content as string;

                if (content == null)
                {
                    TaskDialog.Show("Error", "Panel name not found.");
                    return;
                }
                
                Element selectedPanel = panelDictionary[content];
                FamilyInstance panelInstance = selectedPanel as FamilyInstance;

                MEPModel selectedPanelMEPModel = panelInstance.MEPModel;
                ISet<ElectricalSystem> unsortedElecSys = selectedPanelMEPModel.GetAssignedElectricalSystems();
                List<ElectricalSystem> sortedElecSys = unsortedElecSys.OrderBy(es => int.Parse(es.CircuitNumber.Split(',')[0])).ToList();
                
                foreach (ElectricalSystem elecSys in sortedElecSys)
                {
                    string CircuitNumber = elecSys.CircuitNumber;
                    string firstCircuitNumber = CircuitNumber.Split(',')[0];
                    int circuitNumberInt = int.Parse(firstCircuitNumber);
                    string Name = elecSys.LoadName;
                    windowDictionaryOfElectricalSystems.Add(CircuitNumber, elecSys);

                    try
                    {
                        if (circuitNumberInt % 2 == 0)
                        {
                            viewModel.EvenCircuitItems.Add(new CircuitItem { Name = Name, IsChecked = false, CircuitNumber = CircuitNumber });

                        }
                        else
                        {
                            viewModel.OddCircuitItems.Add(new CircuitItem { Name = Name, IsChecked = false, CircuitNumber = CircuitNumber });
                        }
                    }

                    catch { }
                }

                if (viewModel.OddCircuitItems.Count > 0)
                {
                    viewModel.OddListBoxVisibility = System.Windows.Visibility.Visible;
                }
                MoveCircuitsProperties.elecSysDictionary = windowDictionaryOfElectricalSystems;

            }
        }

        private void OddListBox_MouseDown(object sender, MouseButtonEventArgs e) 
        {
            ObservableCollection<CircuitItem> circuitItems;
            ListBox listbox = sender as ListBox;
            string listBoxName = listbox.Name;
            if( listBoxName == "OddCircuitList")
            {
                circuitItems = viewModel.OddCircuitItems;
            }
            else
            {
                circuitItems = viewModel.EvenCircuitItems;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                var listBox = sender as System.Windows.Controls.ListBox;
                var item = GetClickedItem(listBox, e.GetPosition(listBox));
                

                if (item != null)
                {
                    int currentIndex = circuitItems.IndexOf(item);

                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        bool isBottomToUp = false;
                        if (_lastSelectedIndex.HasValue)
                        {
                            // Select range of items
                            int start = _lastSelectedIndex.Value;
                            int end = currentIndex;

                            if (start > end)
                            {
                                (start, end) = (end, start);
                                isBottomToUp = true;
                            }

                            for (int i = start; i <= end; i++)
                            {
                                if ((isBottomToUp && i != end) || (!isBottomToUp && i != start))
                                {
                                    circuitItems[i].IsChecked = !circuitItems[i].IsChecked;
                                }

                            }
                        }

                        else
                        {
                            item.IsChecked = !item.IsChecked;
                        }
                    }
                    else
                    {
                        // Single selection
                        item.IsChecked = !item.IsChecked;
                    }

                    _lastSelectedIndex = currentIndex;
                    e.Handled = true;
                }
            }
        }


        private CircuitItem GetClickedItem(System.Windows.Controls.ListBox listBox, System.Windows.Point position)
        {
            var element = listBox.InputHitTest(position) as FrameworkElement;
            while (element != null && !(element is ListBoxItem))
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return element?.DataContext as CircuitItem;
        }

    }
}
