using System;
using System.Collections.Generic;
using System.Linq;
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
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
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
