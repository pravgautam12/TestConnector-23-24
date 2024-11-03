using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.MoveCircuitsXAML
{
    public class MoveCircuitsViewModel
    {
        public ObservableCollection<CircuitItem> OddCircuitItems { get; set; }
        public ObservableCollection<CircuitItem> EvenCircuitItems { get; set; }

        public Visibility OddListBoxVisibility;

        public MoveCircuitsViewModel()
        {
            OddCircuitItems = new ObservableCollection<CircuitItem>();
            EvenCircuitItems = new ObservableCollection<CircuitItem>();
            OddListBoxVisibility = Visibility.Hidden;
        }

        public List<CircuitItem> GetCheckedItems()
        {
            return OddCircuitItems.Where(item => item.IsChecked).Concat(EvenCircuitItems.Where(item => item.IsChecked)).ToList();
        }

    }

    
}