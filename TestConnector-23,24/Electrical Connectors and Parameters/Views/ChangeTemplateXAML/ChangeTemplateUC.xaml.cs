using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML
{
    /// <summary>
    /// Interaction logic for ChangeTemplateUC.xaml
    /// </summary>
    public partial class ChangeTemplateUC : UserControl
    {

        private ExternalEvent _externalEvent;
        private ChangeTemplatesEventHandler _handler;

        public ChangeTemplateUC(ChangeTemplateViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _handler = new ChangeTemplatesEventHandler();
            _externalEvent = ExternalEvent.Create(_handler);
        }

        private void ChangeTemplates(object sender, RoutedEventArgs e) 
        {
            
            string selectedPanelScheduleTemplateString = "";

            try { selectedPanelScheduleTemplateString = this.SelectPanelTemplate.SelectedItem.ToString(); } catch { }

            
            PanelScheduleTemplate panelTemplate = MoveCircuitsProperties.panelScheduleTemplateDictionary[selectedPanelScheduleTemplateString];
            var viewModel = DataContext as ChangeTemplateViewModel;

            List<Panel> checkedPanels = viewModel.GetCheckedItems();

            _handler.SetData(checkedPanels, panelTemplate);
            _externalEvent.Raise();
        }

        private void SelectPanelTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
