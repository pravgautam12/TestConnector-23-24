using System.Windows;
using Autodesk.Revit.UI;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views
{
    /// <summary>
    /// Interaction logic for CreateConnectorW.xaml
    /// </summary>
    public partial class CreateConnectorW : Window
    {
        
        public CreateConnectorW(ExternalCommandData commandData)
        {
            InitializeComponent();
            CreateConnectorUC createConnectorUserControl = new CreateConnectorUC(commandData);
            MainGrid.Children.Add(createConnectorUserControl);
        }
    }
}
