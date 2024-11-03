using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Windows.Controls;


namespace TestConnector2.Electrical_Connectors_and_Parameters.Views
{
    
    public partial class CreateConnectorUC : UserControl
    {
        private ExternalCommandData _commandData;
        private ExternalEvent _externalEvent;
        private MyExternalEventHandler handler;

        public CreateConnectorUC()
        {
            InitializeComponent();
        }

        public CreateConnectorUC(ExternalCommandData commandData)
        {
            InitializeComponent();
            _commandData = commandData;
            handler = new MyExternalEventHandler(false);
            _externalEvent = ExternalEvent.Create(handler);
        }
        
        private void Create_Connector(object sender, RoutedEventArgs ex)
        {
            if (Existing.IsChecked != true && New.IsChecked != true)
            {
                TaskDialog.Show("No field selected", "Please select whether you would like to use existing or new shared parameters file.");
            }
            else 
            {
                if (Existing.IsChecked == true)
                {
                    handler._newFile = false;
                    _externalEvent.Raise();
                }
                else
                {
                    handler._newFile = true;
                    _externalEvent.Raise();
                }
            }
        }

    }
}

