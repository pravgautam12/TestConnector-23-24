using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.ChangeTemplateXAML
{
    public class ChangeTemplateViewModel
    {
        public ObservableCollection<Panel> PanelsList { get; set; }

        public ChangeTemplateViewModel()
        {
            PanelsList = new ObservableCollection<Panel>();
        }

        public List<Panel> GetCheckedItems()
        {
            return PanelsList.Where(item => item.IsChecked).ToList();
        }
    }
}
