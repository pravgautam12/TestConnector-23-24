using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML
{
    public class Panel : INotifyPropertyChanged
    {
        private string _panelName {  get; set; } 
        private bool _isChecked { get; set; }

        public string PanelName
        {
            get { return _panelName; }
            set
            {
                if (_panelName != value)
                {
                    _panelName = value;
                    OnPropertyChanged(nameof(PanelName));   
                }
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
