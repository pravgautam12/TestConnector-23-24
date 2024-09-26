using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TestConnector2
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class EquipmentCircuiting : IExternalCommand
    {
        private static string ribbonPanelName = "Autometica_Electrical";
        private static string parameter_prefix = "AMCA_";
        private static string disconnectFamilyName = "Disconnect Switches - Equipment";
        private static string jBoxFamilyName = "Junction Box";
        public static FamilyInstance electricalPanel;



        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            List<ComboBox> comboBoxes = uiApplication.UiApp.GetRibbonPanels(ribbonPanelName).SelectMany(panel => panel.GetItems()).OfType<ComboBox>().ToList();
            String jBoxOrDisconnect = null;
            String panelName = null;

            foreach (ComboBox cBox in comboBoxes)
            {
                if (cBox.Name == "Select Type")
                {
                    jBoxOrDisconnect = cBox.Current.Name;
                }

                if (cBox.Name == "Select Panel")
                {
                    panelName = cBox.Current.Name;
                }

            }

            electricalPanel = GlobalDictionary.Dictionary[panelName] as FamilyInstance;



            Options options = new Options();
            options.ComputeReferences = true;

            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            Reference pickedref = null;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            pickedref = sel.PickObject(ObjectType.Element, "wassup");
            Element elem = doc.GetElement(pickedref);
            FamilyInstance rtu = elem as FamilyInstance;

            CircuitToPanel(uiApplication.UiApp, doc, electricalPanel, rtu, jBoxOrDisconnect);

            IFamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();


            return Result.Succeeded;

        }

        public static FamilyInstance PlaceDisconnectOrJBox(Document doc, Element equipment, Element disconnect, String jBoxorDisconnect)
        {
            FamilyInstance x = null;


            //Parameter apparent_load = x.LookupParameter("APPARENT POWER");

            FamilySymbol symbol = disconnect as FamilySymbol;

            Parameter rtuLevel = equipment.LookupParameter("Level");
            ElementId rtuLevelId = rtuLevel.AsElementId();

            Level level = doc.GetElement(rtuLevelId) as Level;

            Options geomOptions = new Options();
            geomOptions.ComputeReferences = true;
            geomOptions.DetailLevel = ViewDetailLevel.Fine;
            geomOptions.IncludeNonVisibleObjects = true;
            Face verticalFace;

            if (jBoxorDisconnect == "Disconnect")
            {
                GeometryElement geomElement = equipment.get_Geometry(geomOptions);
                PlanarFace planarFace = null;

                foreach (GeometryObject obj in geomElement)
                {
                    GeometryInstance geomInstance = obj as GeometryInstance;

                    if (geomInstance != null)
                    {
                        GeometryElement symbolGeometry = geomInstance.GetInstanceGeometry();

                        foreach (GeometryObject obj2 in symbolGeometry)
                        {
                            Solid geomSolid = obj2 as Solid;

                            if (geomSolid != null)
                            {
                                foreach (Face geomFace in geomSolid.Faces)
                                {
                                    planarFace = geomFace as PlanarFace;
                                    //planarFace = geomFace;
                                    if (planarFace != null)

                                    {
                                        XYZ faceNormal = planarFace.FaceNormal;

                                        //if (Math.Abs(faceNormal.Z) < 0.001)
                                        //{
                                        verticalFace = planarFace;

                                        BoundingBoxUV boundingBox = verticalFace.GetBoundingBox();
                                        UV centerUV = (boundingBox.Min + boundingBox.Max) / 2.0;
                                        XYZ center = verticalFace.Evaluate(centerUV);
                                        XYZ location = verticalFace.Evaluate(centerUV);
                                        symbol.Activate();
                                        XYZ normal = verticalFace.ComputeNormal(centerUV);


                                        XYZ refDir = normal.CrossProduct(XYZ.BasisZ);
                                        Transaction trans = new Transaction(doc, "Creating Electrical System");
                                        trans.Start();
                                        try
                                        {
                                            x = doc.Create.NewFamilyInstance(verticalFace, location, refDir, symbol);


                                            //Parameter rtuLevel = equipment.LookupParameter("Level");
                                            //ElementId rtuLevelId = rtuLevel.AsElementId();

                                            Parameter xLevel = x.LookupParameter("Schedule Level");
                                            xLevel.Set(rtuLevelId);


                                            trans.Commit();
                                            goto LoopEnd;
                                        }
                                        catch (Exception ex)
                                        {
                                            trans.RollBack();
                                        }

                                    }
                                }



                            }
                        }
                    }


                }
            }
        LoopEnd: Console.WriteLine("Loop has ended");

            if (jBoxorDisconnect == "J-Box")
            {
                BoundingBoxXYZ bBox = equipment.get_BoundingBox(doc.ActiveView);
                XYZ center = (bBox.Min + bBox.Max) / 2;
                Transaction trans = new Transaction(doc, "Creating Electrical System");
                trans.Start();
                try
                {
                    x = doc.Create.NewFamilyInstance(center, symbol, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    trans.Commit();
                }

                catch (Exception ex)
                {
                    trans.RollBack();
                }

            }
            return x;
        }

        public static void CircuitToPanel(UIApplication uiApp, Document doc, FamilyInstance panel, FamilyInstance rtu, String jBoxorDisconnect)
        {
            IList<ElectricalLoadClassification> loadClassifications = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ElectricalLoadClassifications).ToElements().Cast<ElectricalLoadClassification>().ToList();
            //IList<Element> mechEquip = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MechanicalEquipment).WhereElementIsNotElementType().ToElements();
            ElectricalLoadClassification heating = null;
            ElectricalLoadClassification cooling = null;
            IList<Element> disconnects = null;
            IList<Element> jBoxes = null;
            Double VA = 0;
            Double voltage = 0;
            Double factor = 0;
            Double current = 0;

            Dictionary<string, Element> disconnectOrJBoxDictionary = new Dictionary<string, Element>();
            Dictionary<string, Element> disconnectDictionary = new Dictionary<string, Element>();
            Dictionary<string, Element> jBoxDictionary = new Dictionary<string, Element>();


            foreach (ElectricalLoadClassification lc in loadClassifications)
            {
                if (lc.Name == "Heating")
                {
                    heating = lc;
                }

                if (lc.Name == "Cooling")
                {
                    cooling = lc;
                }
            }

            if (jBoxorDisconnect == "Disconnect")
            {

                disconnects = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ElectricalEquipment).WhereElementIsElementType().ToElements();
                disconnects = disconnects.Where(e => e is ElementType elementType && elementType.FamilyName.Equals(disconnectFamilyName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (jBoxorDisconnect == "J-Box")
            {
                jBoxes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ElectricalFixtures).WhereElementIsElementType().ToElements();
                jBoxes = jBoxes.Where(j => j is ElementType elementType && elementType.FamilyName.Equals(jBoxFamilyName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (jBoxes == null)
            {
                foreach (Element elem in disconnects)
                {
                    if (elem.Name != "")
                    {
                        disconnectDictionary.Add(elem.Name, elem);
                        //break;
                    }
                }
            }

            else if (disconnects == null)
            {
                foreach (Element elem in jBoxes)
                {
                    if (elem.Name != "")
                    {
                        jBoxDictionary.Add(elem.Name, elem);
                    }
                }

            }

            String voltageAndPhase = "";
            String MOCP = "";
            String KW = "";
            String FLA = "";
            String MCA = "";

            FamilySymbol symbol = rtu.Symbol;
            try { voltageAndPhase = symbol.LookupParameter(parameter_prefix + "M_ELEC_VOLTS PH TEXT").AsString(); } catch { }
            try { MOCP = symbol.LookupParameter(parameter_prefix + "M_ELEC_MOCP TEXT").AsString(); } catch { }
            try { KW = symbol.LookupParameter(parameter_prefix + "M_FULL LOAD KW TEXT").AsString(); } catch { }
            try { FLA = symbol.LookupParameter(parameter_prefix + "M_ELEC_FLA TEXT").AsString(); } catch { }
            try { MCA = symbol.LookupParameter(parameter_prefix + "M_ELEC_MCA TEXT").AsString(); } catch { }

            CircuitInformation cktInfo = new CircuitInformation();
            cktInfo.SetVoltageAndFactor(voltageAndPhase);
            cktInfo.CalculateCircuitParameters(MOCP, KW, FLA, MCA);

            if (KW != "")
            {
                VA = Double.Parse(KW) * 1000;

            }

            if (KW == "")
            {
                VA = cktInfo.voltage * cktInfo.factor * cktInfo.current;
            }

            Element desired_Disconnect = null;
            Element desired_jBox = null;
            FamilyInstance disconnectOrJBoxPlaced = null;

            if (disconnects != null)
            {
                desired_Disconnect = disconnectDictionary[cktInfo.amps_name + " -" + " " + cktInfo.volts_name + " -" + " " + cktInfo.phase_name + "PH"] as Element;
                disconnectOrJBoxPlaced = PlaceDisconnectOrJBox(doc, rtu, desired_Disconnect, jBoxorDisconnect);
            }

            if (disconnects == null)
            {
                desired_jBox = jBoxDictionary[cktInfo.volts_name + " -" + " " + cktInfo.phase_name + "PH"] as Element;
                disconnectOrJBoxPlaced = PlaceDisconnectOrJBox(doc, rtu, desired_jBox, jBoxorDisconnect);
            }

            Transaction circuit = new Transaction(doc, "Creating Electrical System");
            circuit.Start();

            try
            {
                disconnectOrJBoxPlaced.LookupParameter("APPARENT POWER").SetValueString(VA.ToString());
            }

            catch
            {
                disconnectOrJBoxPlaced.LookupParameter("Apparent Load").SetValueString(VA.ToString());
            }

            Connector electricalConnector = GetConnector(disconnectOrJBoxPlaced);

            ElectricalSystem elecSys = null;

            if (electricalConnector != null)
            {
                elecSys = ElectricalSystem.Create(electricalConnector, ElectricalSystemType.PowerCircuit);
                elecSys.Rating = cktInfo.BreakerRating;
                elecSys.LoadName = rtu.LookupParameter("Mark").AsString() + cktInfo.GetWireSize(elecSys.Rating);
                try
                {
                    elecSys.SelectPanel(electricalPanel);
                }
                catch
                {
                    //Parameter param = electricalPanel.LookupParameter("Distribution System");
                    String distributionSystem = electricalPanel.LookupParameter("Distribution System").AsValueString().Replace(" Wye", "");
                    //distributionSystem.Replace(" Wye", "");
                    string[] splitArray = distributionSystem.Split('/');
                    List<string> distributionVoltages = new List<string>(splitArray);
                    if (cktInfo.voltage != Double.Parse(distributionVoltages[0]) && cktInfo.voltage != Double.Parse(distributionVoltages[1]))
                    {
                        TaskDialog.Show("Cannot connect to panel", "Please make sure the panel voltage and equipment voltage match.");
                    }
                    else
                    {
                        ReplacingSparesOrSpaces replacingSparesOrSpaces = new ReplacingSparesOrSpaces(cktInfo.factor, cktInfo.voltage, electricalPanel, doc);
                        replacingSparesOrSpaces.CircuitWhenSparesOrSpaces(elecSys);
                    }

                }
            }

            else
            {
                TaskDialog.Show("Error", "Unable to find the electrical connector of element");
            }

            bool isElementOutside = OutsideOrNot(doc, disconnectOrJBoxPlaced);

            if (isElementOutside == true)
            {
                if (jBoxorDisconnect == "Disconnect")
                {
                    //disconnectOrJBoxPlaced.LookupParameter("Comments").Set($"{amps_name}");
                    Parameter comments = disconnectOrJBoxPlaced.LookupParameter("Comments");

                    if (cktInfo.phase_name == "1")
                    {
                        comments.Set($"{cktInfo.amps_name.Replace("A", "")}/2/NF/N3R");
                    }
                    else if (cktInfo.phase_name == "3")
                    {
                        comments.Set($"{cktInfo.amps_name.Replace("A", "")}/3/NF/N3R");
                    }
                }
            }

            else
            {
                if (jBoxorDisconnect == "Disconnect")
                {
                    Parameter comments = disconnectOrJBoxPlaced.LookupParameter("Comments");
                    if (cktInfo.phase_name == "1")
                    {
                        comments.Set($"{cktInfo.amps_name.Replace("A", "")}/2/NF");
                    }
                    else if (cktInfo.phase_name == "3")
                    {
                        comments.Set($"{cktInfo.amps_name.Replace("A", "")}/3/NF");
                    }
                }
            }


            circuit.Commit();
        }

        public static PanelScheduleView GetPanelSchedule(FamilyInstance panel, Document doc)
        {
            var panelScheduleViews = new FilteredElementCollector(doc).OfClass(typeof(PanelScheduleView)).Cast<PanelScheduleView>().ToList();

            foreach (PanelScheduleView panelScheduleView in panelScheduleViews)
            {
                ElementId panelScheduleViewId = panelScheduleView.GetPanel();
                if (panelScheduleViewId == panel.Id)
                {
                    return panelScheduleView;
                }
            }
            return null;
        }

        public static Connector GetConnector(FamilyInstance disconnectOrJBox)
        {

            MEPModel m = disconnectOrJBox.MEPModel;
            ConnectorManager cm = m.ConnectorManager;
            ConnectorSet c = cm.UnusedConnectors;
            Connector firstConnector = null;

            if (c.Size > 0)
            {
                foreach (Connector conn in c)
                {
                    if (conn.Domain == Domain.DomainElectrical)
                    {
                        firstConnector = conn;
                        return firstConnector;
                    }
                }
            }

            else
            {

            }
            return null;
        }

        public static bool OutsideOrNot(Document doc, FamilyInstance familyInstance)
        {
            WorksetTable worksetTable = doc.GetWorksetTable();
            WorksetId worksetId = familyInstance.WorksetId;
            Workset workset = worksetTable.GetWorkset(worksetId);

            if (workset.Name == "ELEC - ROOF" || workset.Name == "ELEC - SITE")
            {
                return true;
            }
            return false;
        }

    }

}


