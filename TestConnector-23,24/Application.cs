using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Collections.Generic;


namespace TestConnector2
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public static class GlobalDictionary
    {
        public static Dictionary<string, Element> Dictionary { get; set; } = new Dictionary<string, Element>();
    }
    //

    public class Application : IExternalApplication
    {
        //new code
        private Dictionary<string, Element> elementMap = new Dictionary<string, Element>();

        ComboBoxData cDataSecond = new ComboBoxData("Select Panel");
        private ComboBox cBoxSecond;
        //new code finished

        public static double SelectedValue { get; set; } = 0.00;
        public static double totalArea { get; set; } = 0.00;
        public static bool overRide { get; set; } = false;


        public Result OnStartup(UIControlledApplication application)
        {
            String nameSpaceName = "TestConnector2";
            String tabName = "ElectricalTestyTesty";
            application.CreateRibbonTab(tabName);

            application.Idling += (sender, args) =>
            {
                UIApplication uu = sender as UIApplication;
                UIApp.UiApp = uu;
            };

            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Tools");
            RibbonPanel ribbonPanel1 = application.CreateRibbonPanel(tabName, "ComCheck");
            RibbonPanel ribbonPanel2 = application.CreateRibbonPanel(tabName, "HVAC Circuiting");


            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData("Run ComCheck", "Run ComCheck", assemblyPath, nameSpaceName + ".ComCheck");
            PushButtonData buttonData0 = new PushButtonData("Optional: Input Square Footage", "Optional: Input Square Footage", assemblyPath, nameSpaceName + ".Option0Command");
            PushButtonData b1Data = new PushButtonData("Create Connector", "Create Connector", assemblyPath, nameSpaceName + ".Class1");
            PushButtonData b2Data = new PushButtonData("Create panel schedules", "Create panel schedules", assemblyPath, nameSpaceName + ".Class3");
            PulldownButtonData pulldownData = new PulldownButtonData("DropdownButton", "Select Building Type");

            TextBoxData data = new TextBoxData("changeTemplate");

            IList<RibbonItem> stackedItems = ribbonPanel.AddStackedItems(b1Data, b2Data, data);
            IList<RibbonItem> stackedItems1 = ribbonPanel1.AddStackedItems(pulldownData, buttonData0, buttonData);

            PulldownButton pulldownButton = stackedItems1[0] as PulldownButton;

            TextBox textbox = stackedItems[2] as TextBox;

            textbox.PromptText = "Enter Panel Template";

            //application.ControlledApplication.DocumentOpened += OnDocumentOpened;
            textbox.EnterPressed += TextBox_EnterPressed;

            PushButtonData buttonData1 = new PushButtonData("Automotive Facility", "Automotive Facility", assemblyPath, nameSpaceName + ".Option1Command");
            pulldownButton.AddPushButton(buttonData1);

            PushButtonData buttonData2 = new PushButtonData("Office/Convention Center", "Office/Convention Center", assemblyPath, nameSpaceName + ".Option2Command");
            pulldownButton.AddPushButton(buttonData2);

            PushButtonData buttonData3 = new PushButtonData("Court House", "Court House", assemblyPath, nameSpaceName + ".Option3Command");
            pulldownButton.AddPushButton(buttonData3);

            PushButtonData buttonData4 = new PushButtonData("Bar Lounge/Leisure", "Bar Lounge/Leisure", assemblyPath, nameSpaceName + ".Option4Command");
            pulldownButton.AddPushButton(buttonData4);

            PushButtonData buttonData5 = new PushButtonData("Cafeteria/Fast Food/Gym/Arena", "Cafeteria/Fast Food/Gym/Arena", assemblyPath, nameSpaceName + ".Option5Command");
            pulldownButton.AddPushButton(buttonData5);

            PushButtonData buttonData6 = new PushButtonData("Family Dining", "Family Dining", assemblyPath, nameSpaceName + ".Option6Command");
            pulldownButton.AddPushButton(buttonData6);

            PushButtonData buttonData7 = new PushButtonData("Dormitory", "Dormitory", assemblyPath, nameSpaceName + ".Option7Command");
            pulldownButton.AddPushButton(buttonData7);

            PushButtonData buttonData8 = new PushButtonData("Exercise Center/School", "Exercise Center/School", assemblyPath, nameSpaceName + ".Option8Command");
            pulldownButton.AddPushButton(buttonData8);

            PushButtonData buttonData9 = new PushButtonData("Hotel/Fire Station", "Hotel/Fire Station", assemblyPath, nameSpaceName + ".Option9Command");
            pulldownButton.AddPushButton(buttonData9);

            PushButtonData buttonData10 = new PushButtonData("Healthcare Clinic", "Healthcare Clinic", assemblyPath, nameSpaceName + ".Option10Command");
            pulldownButton.AddPushButton(buttonData10);

            PushButtonData buttonData11 = new PushButtonData("Hospital", "Hospital", assemblyPath, nameSpaceName + ".Option11Command");
            pulldownButton.AddPushButton(buttonData11);

            PushButtonData buttonData12 = new PushButtonData("Library", "Library", assemblyPath, nameSpaceName + ".Option12Command");
            pulldownButton.AddPushButton(buttonData12);

            PushButtonData buttonData13 = new PushButtonData("Manufacturing Facility", "Manufacturing Facility", assemblyPath, nameSpaceName + ".Option13Command");
            pulldownButton.AddPushButton(buttonData13);

            PushButtonData buttonData14 = new PushButtonData("Movie Theater", "Movie Theater", assemblyPath, nameSpaceName + ".Option14Command");
            pulldownButton.AddPushButton(buttonData14);

            PushButtonData buttonData15 = new PushButtonData("Multifamily/Warehouse", "Multifamily/Warehouse", assemblyPath, nameSpaceName + ".Option15Command");
            pulldownButton.AddPushButton(buttonData15);

            PushButtonData buttonData16 = new PushButtonData("Museum", "Museum", assemblyPath, nameSpaceName + ".Option16Command");
            pulldownButton.AddPushButton(buttonData16);

            PushButtonData buttonData17 = new PushButtonData("Parking Garage", "Parking Garage", assemblyPath, nameSpaceName + ".Option17Command");
            pulldownButton.AddPushButton(buttonData17);

            PushButtonData buttonData18 = new PushButtonData("Penitentiary/Town Hall", "Penitentiary/Town Hall", assemblyPath, nameSpaceName + ".Option18Command");
            pulldownButton.AddPushButton(buttonData18);

            PushButtonData buttonData19 = new PushButtonData("Retail/Performing Arts Theater", "Retail/Performing Arts Theater", assemblyPath, nameSpaceName + ".Option19Command");
            pulldownButton.AddPushButton(buttonData19);

            PushButtonData buttonData20 = new PushButtonData("Police", "Police", assemblyPath, nameSpaceName + ".Option20Command");
            pulldownButton.AddPushButton(buttonData20);

            PushButtonData buttonData21 = new PushButtonData("Post Office", "Post Office", assemblyPath, nameSpaceName + ".Option21Command");
            pulldownButton.AddPushButton(buttonData21);

            PushButtonData buttonData22 = new PushButtonData("Religious Building", "Religious Building", assemblyPath, nameSpaceName + ".Option22Command");
            pulldownButton.AddPushButton(buttonData22);

            PushButtonData buttonData23 = new PushButtonData("Transportation", "Transportation", assemblyPath, nameSpaceName + ".Option23Command");
            pulldownButton.AddPushButton(buttonData23);

            PushButtonData buttonData24 = new PushButtonData("Workshop", "Workshop", assemblyPath, nameSpaceName + ".Option24Command");
            pulldownButton.AddPushButton(buttonData24);


            //new code

            application.ControlledApplication.DocumentOpened += OnDocumentOpened;

            ribbonPanel2.Name = "HVAC Circuits";

            application.ControlledApplication.DocumentOpened += OnDocumentOpened;

            PushButtonData b3Data = new PushButtonData("Add", "Add", Assembly.GetExecutingAssembly().Location, nameSpaceName + ".Class4");
            ribbonPanel2.AddStackedItems(cDataSecond, b3Data);

            PushButtonData b4Data = new PushButtonData("Remove", "Remove", Assembly.GetExecutingAssembly().Location, nameSpaceName + ".Class4");

            IList<RibbonItem> items = ribbonPanel2.GetItems();
            cBoxSecond = items[0] as ComboBox;

            ComboBoxData cData = new ComboBoxData("Select Type");
            //ComboBox cBox = ribbonPanel.AddItem(cData) as ComboBox;
            ribbonPanel2.AddStackedItems(cData, b4Data);
            IList<RibbonItem> items1 = ribbonPanel2.GetItems();
            ComboBox cBox = items1[2] as ComboBox;
            ComboBoxMemberData jBoxData = new ComboBoxMemberData("J-Box", "J-Box");
            ComboBoxMemberData disconnectData = new ComboBoxMemberData("Disconnect", "Disconnect");

            IList<ComboBoxMemberData> memberData = new List<ComboBoxMemberData> { jBoxData, disconnectData };
            cBox.AddItems(memberData);

            //new code finished 


            return Result.Succeeded;
        }
        private void TextBox_EnterPressed(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var text = textBox.Value;
            Class2.ChangeTemplate(DocumentInstance.Instance, text.ToString());
        }

        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            DocumentInstance.Instance = e.Document;
            UIApplication uiapp = sender as UIApplication;
            UIApp.UiApp = uiapp;
            Document doc = e.Document;

            //new code

            IList<Element> electricalEquipment = new FilteredElementCollector(DocumentInstance.Instance).OfCategory(BuiltInCategory.OST_ElectricalEquipment).WhereElementIsNotElementType().ToElements();

            foreach (FamilyInstance element in electricalEquipment)
            {

                Parameter param = element.LookupParameter("Distribution System");
                if (param != null)
                {
                    string panelName = element.LookupParameter("Panel Name").AsString();
                    if (panelName != null)
                    {
                        ComboBoxMemberData cPanelData = new ComboBoxMemberData(panelName, panelName);
                        cBoxSecond.AddItem(cPanelData);
                        elementMap[panelName] = element;
                        GlobalDictionary.Dictionary.Add(panelName, element);
                    }
                }

            }

            //new code finished
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}