using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TestConnector2
{
    public static class DocumentInstance
    {
        public static Document Instance { get; set; }
    }

    public static class uiApplication
    {
        public static UIApplication UiApp { get; set; }
    }

    public static class MoveCircuitsProperties
    {
        public static Dictionary<string, ElectricalSystem> elecSysDictionary = new Dictionary<string, ElectricalSystem>();

        private static Dictionary<string, Element> _panelDictionary = new Dictionary<string, Element>();
        public static Dictionary<string, Element> panelDictionary
        { 
            get
            {
                _panelDictionary.Clear();

                IList<Element> panelList = new FilteredElementCollector(DocumentInstance.Instance).OfCategory(BuiltInCategory.OST_ElectricalEquipment).
                    WhereElementIsNotElementType().ToElements();
                foreach (Element element in panelList)
                {
                    try { _panelDictionary.Add(element.Name, element); } catch { }
                }
                return _panelDictionary;
            }

            set { }
        }

        public static Dictionary<string, PanelScheduleTemplate> panelScheduleTemplateDictionary = new Dictionary<string, PanelScheduleTemplate>();  
    }

    public static class SetWindowHandle
    {
        public static IntPtr revitMainWindowHandle = Process.GetProcessesByName("Revit").FirstOrDefault()?.MainWindowHandle ?? IntPtr.Zero;
    }

    public class CommonProps
    {
        private static Dictionary<string, List<string>> stateAndCities = new Dictionary<string, List<string>>();

        //static CommonProps()
        //{
        //    SetStateCities();
        //}
        public static Dictionary<string, List<string>> SetStateCities()
        {
            string filePath = "C:\\Users\\pravi\\OneDrive\\Desktop\\data.json";


            // Read the JSON file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON into a Dictionary<string, List<string>>
            stateAndCities = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            return stateAndCities;
        }
    }
}
