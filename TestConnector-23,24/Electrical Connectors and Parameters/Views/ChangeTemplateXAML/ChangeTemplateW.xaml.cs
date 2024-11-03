using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML
{
    /// <summary>
    /// Interaction logic for ChangeTemplateW.xaml
    /// </summary>
    public partial class ChangeTemplateW : Window
    {
        ChangeTemplateViewModel changeTemplateViewModel;
        private int? _lastSelectedIndex = null;
        Dictionary<string, PanelScheduleTemplate> panelScheduleTemplateDictionary = new Dictionary<string, PanelScheduleTemplate>();

        public ChangeTemplateW(ExternalCommandData commandData)
        {
            InitializeComponent();
            //DataContext = changeTemplateViewModel;
            changeTemplateViewModel = new ChangeTemplateViewModel();
            DataContext = changeTemplateViewModel;
            ChangeTemplateUC changeTemplateUC = new ChangeTemplateUC(changeTemplateViewModel);
            TemplateChangeGrid.Children.Add(changeTemplateUC);
            
            

            IList<Element> panels = new FilteredElementCollector(DocumentInstance.Instance).OfCategory(BuiltInCategory.OST_ElectricalEquipment).WhereElementIsNotElementType().ToElements();
            string panelName = "";

            IList<Element> sortedPanels = panels.OrderBy(p =>
            {
                FamilyInstance famInst = p as FamilyInstance;
                ElectricalEquipment panelEquipment = famInst.MEPModel as ElectricalEquipment;

                return panelEquipment.IsSwitchboard ? 0 : 1;
            }).ThenBy(p => p.Name).ToList();

            foreach (Element panel in sortedPanels)
            {
                Parameter panelNameParameter = (panel as FamilyInstance).LookupParameter("Panel Name");
                if (panelNameParameter != null)
                {
                    if (panelNameParameter.HasValue)
                    {
                        string panelNameParameterString = panelNameParameter.AsString();
                        changeTemplateViewModel.PanelsList.Add(new Panel { PanelName = panelNameParameterString, IsChecked = false });
                    }

                    else
                    {
                        changeTemplateViewModel.PanelsList.Add( new Panel { PanelName = (panel as FamilyInstance).Name, IsChecked = false });
                    }
                }
            }

            IList<Element> branchPanelScheduleTemplates = new FilteredElementCollector(DocumentInstance.Instance).OfCategory(BuiltInCategory.OST_BranchPanelScheduleTemplates).ToElements();
            IList<Element> switchBoardScheduleTemplates = new FilteredElementCollector(DocumentInstance.Instance).OfCategory(BuiltInCategory.OST_SwitchboardScheduleTemplates).ToElements();

            List<Element> allPanelScheduleTemplates = new List<Element>(branchPanelScheduleTemplates);
            allPanelScheduleTemplates.AddRange(switchBoardScheduleTemplates);

            foreach (Element panelScheduleTemplate in allPanelScheduleTemplates)
            {
                try { panelScheduleTemplateDictionary.Add((panelScheduleTemplate as PanelScheduleTemplate).Name, panelScheduleTemplate as PanelScheduleTemplate); }
                catch { }
                changeTemplateUC.SelectPanelTemplate.Items.Add((panelScheduleTemplate as PanelScheduleTemplate).Name);
            }

            MoveCircuitsProperties.panelScheduleTemplateDictionary = panelScheduleTemplateDictionary;

            changeTemplateUC.PanelsList.PreviewMouseDown += PanelsList_MouseDown;
            

        }


        private void PanelsList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //ObservableCollection<Panel> listBox;
            ListBox listbox = sender as ListBox;
            string listBoxName = listbox.Name;
            
            if (e.ChangedButton == MouseButton.Left)
            {
                var listBox = sender as System.Windows.Controls.ListBox;
                var item = GetClickedItem(listBox, e.GetPosition(listBox));


                if (item != null)
                {
                    int currentIndex = changeTemplateViewModel.PanelsList.IndexOf(item);

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
                                    changeTemplateViewModel.PanelsList[i].IsChecked = !changeTemplateViewModel.PanelsList[i].IsChecked;
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

        private Panel GetClickedItem(System.Windows.Controls.ListBox listBox, System.Windows.Point position)
        {
            var element = listBox.InputHitTest(position) as FrameworkElement;
            while (element != null && !(element is ListBoxItem))
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return element?.DataContext as Panel;
        }

    }
}
