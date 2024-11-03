using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TestConnector2.Electrical_Connectors_and_Parameters.Views.COMcheckXAML
{
    /// <summary>
    /// Interaction logic for COMcheckUC.xaml
    /// </summary>
    public partial class COMcheckUC : UserControl
    {
        public COMcheckUC()
        {
            InitializeComponent();
        }

        private void ProjectAddressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text when the TextBox gets focus
            projectAddressTextBox.Focus();
            projectAddressTextBox.SelectAll();
            e.Handled = true;
        }

        private void ProjectCityTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text when the TextBox gets focus
            projectCityTextBox.Focus();
            projectCityTextBox.SelectAll();
            e.Handled = true;
        }

        private void zipTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text when the TextBox gets focus
            zipTextBox.Focus();
            zipTextBox.SelectAll();
            e.Handled = true;
        }

        private void CodeYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (codeYear.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)codeYear.SelectedItem;
                //myTextBlock.Text = $"Selected: {selectedItem.Content.ToString()}";
            }
        }

        private void State_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string stateString = "";
            ComboBoxItem stateItem = (ComboBoxItem)State.SelectedItem;
            try { stateString = stateItem.Content.ToString(); } catch { }

            Dictionary<string, List<string>> stateAndCities = CommonProps.SetStateCities();
            int x = stateAndCities.Count;

            List<string> listOfCitiesInThatState = stateAndCities[stateString];
            City.Items.Clear();

            foreach (string city in listOfCitiesInThatState)
            {
                string formattedCity = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.ToLower());
                City.Items.Add(formattedCity);
            }

        }

        private void TypeOfConstructionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RunComcheck_Click(object sender, RoutedEventArgs e)
        {

            IList<FamilyInstance> lightFixtures = new FilteredElementCollector(DocumentInstance.Instance).OfCategory(BuiltInCategory.OST_LightingFixtures).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            var fixtureCounts = lightFixtures.GroupBy(
                fixture =>
                {
                    var familySymbol = fixture.Symbol.Name;
                    //var typeMark = fixture.Symbol.LookupParameter("Type Mark");

                    //return typeMark?.AsString() ?? "Unknown";
                    return familySymbol ?? "Unknown";
                })
                .Select(group =>
                {
                    var familySymbol = group.Key;
                    var count = group.Count();
                    var typeMark = group.Select(fixture => fixture.Symbol.LookupParameter("Type Mark").AsString()).FirstOrDefault();
                    var apparentLoad = group.Select(fixture => fixture.Symbol.LookupParameter("Apparent Power").AsValueString()).FirstOrDefault();

                    if (typeMark == null)
                    {
                        typeMark = "-";
                    }

                    if (apparentLoad == null)
                    {
                        apparentLoad = "0 VA";
                    }
                    else
                    {
                        apparentLoad = apparentLoad.ToString();
                    }
                    apparentLoad = apparentLoad.Split(' ')[0];
                    double apparentLoadNumber = double.Parse(apparentLoad);
                    //.Select(param => param.AsDouble() != null ? param.AsDouble() : 0).FirstOrDefault();

                    return $"{typeMark} {count} {apparentLoad}";
                }).ToList();

            string fixtureData = string.Join("\n", fixtureCounts);


            string projectAddressTextBoxContent = "";
            string projectCityTextBoxContent = "";
            string zipTextBoxContent = "";
            string codeYearString = "";
            string stateString = "";
            string cityString = "";
            string constructionTypeString = "";
            string spaceTypeString = "";
            string browserString = "";

            try { projectAddressTextBoxContent = projectAddressTextBox.Text; } catch { }
            try { projectCityTextBoxContent = projectCityTextBox.Text; } catch { }
            try { zipTextBoxContent = zipTextBox.Text; } catch { }

            ComboBoxItem codeYearItem = (ComboBoxItem)codeYear.SelectedItem;

            try { codeYearString = codeYearItem.Content.ToString(); } catch { }


            ComboBoxItem stateItem = (ComboBoxItem)State.SelectedItem;
            try { stateString = stateItem.Content.ToString(); } catch { }


            try { cityString = City.SelectedItem as string; } catch { }

            ComboBoxItem constructionTypeItem = (ComboBoxItem)TypeOfConstruction.SelectedItem;
            try { constructionTypeString = constructionTypeItem.Content.ToString(); } catch { }

            ComboBoxItem spaceTypeItem = (ComboBoxItem)SpaceType.SelectedItem;
            try { spaceTypeString = spaceTypeItem.Content.ToString(); } catch { }

            ComboBoxItem browserItem = (ComboBoxItem)Browser.SelectedItem;
            try { browserString = browserItem.Content.ToString(); } catch { }

            try { projectAddressTextBoxContent = projectAddressTextBox.Text; } catch { }
            try { projectCityTextBoxContent = projectCityTextBox.Text; } catch { }
            try { zipTextBoxContent = zipTextBox.Text; } catch { }

            string pythonPath = @"C:\Python311\python.exe";
            string scriptPath = @"C:\Users\pravi\Selenium Python Testing\test.py";


            if (codeYearString != "" && stateString != "" && constructionTypeString != "" && spaceTypeString != "" && browserString != "")
            {
                ProcessStartInfo start = new ProcessStartInfo();

                start.FileName = pythonPath;
                start.Arguments = $"\"{scriptPath}\" \"{codeYearString}\" \"{stateString}\"  \"{cityString}\" \"{constructionTypeString}\" \"{spaceTypeString}\" \"{browserString}\" \"{fixtureData}\" \"{projectAddressTextBoxContent}\" \"{projectCityTextBoxContent}\" \"{zipTextBoxContent}\"";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                using (Process process = Process.Start(start))
                {
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        TaskDialog.Show("Python Output", result);
                    }
                    using (System.IO.StreamReader reader = process.StandardError)
                    {
                        string error = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(error))
                        {
                            TaskDialog.Show("Python Error", error);

                        }
                    }
                }
            }
            else { TaskDialog.Show("Error", "Please select appropriate values for all fields."); }
        }

    }
}
