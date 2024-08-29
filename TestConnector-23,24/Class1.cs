using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;
using static TestConnector2.Class1;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;

namespace TestConnector2
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]


    public class Class1 : IExternalCommand


    {

        ConnectorElement GetElectricalConnector(Document familyDoc)
        {
            FilteredElementCollector extrusioncollector = new FilteredElementCollector(familyDoc).OfClass(typeof(Extrusion));
            FilteredElementCollector revolutioncollector = new FilteredElementCollector(familyDoc).OfClass(typeof(Revolution));

            IList<Element> extrusions = extrusioncollector.ToElements();
            IList<Element> revolutions = revolutioncollector.ToElements();
            List<Element> revolutionsAndExtrusions = extrusions.Concat(revolutions).ToList();

            PlanarFace planarface = null;
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;
            options.DetailLevel = ViewDetailLevel.Fine;
            List<PlanarFace> pf = new List<PlanarFace>();
            try
            {
                foreach (Element e in revolutionsAndExtrusions)
                {
                    GeometryElement geom = e.get_Geometry(options);

                    foreach (GeometryObject geomObj in geom)
                    {
                        if (geomObj is Solid solid)
                        {
                            foreach (Face face in solid.Faces)
                            {
                                if (face is PlanarFace)
                                {
                                    planarface = face as PlanarFace;
                                    try
                                    {
                                        ConnectorElement connector = ConnectorElement.CreateElectricalConnector(familyDoc, ElectricalSystemType.PowerBalanced, planarface.Reference);
                                        return connector;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    //pf.Add(planarface);
                                    //goto LoopEnd;
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + "Failed to get planarFace");
                return null;
            }


            return null;

        }






        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {



            Options options = new Options();
            options.ComputeReferences = true;

            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            //string title = doc.Title;
            //title = title.Substring(23, 2);
            FilteredElementCollector light_fixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_LightingFixtures).WhereElementIsNotElementType();
            IList<Element> instances = light_fixtures.ToElements();

            //Reference pickedref = null;
            //Selection sel = uiapp.ActiveUIDocument.Selection;
            //pickedref = sel.PickObject(ObjectType.Element, "wassup");
            //Element elem = doc.GetElement(pickedref);
            //FamilyInstance famins = elem as FamilyInstance;



            IFamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();




            foreach (Element elem in instances)
            {

                Transaction xx = new Transaction(doc, "xx");
                xx.Start();
                doc.Regenerate();
                xx.Commit();



                FamilySymbol familySymbol = null;
                Family family = null;


                if (elem is FamilyInstance familyinstance)
                {
                    familySymbol = doc.GetElement(familyinstance.Symbol.Id) as FamilySymbol;
                }
                else if (elem is ElementType elementType)
                {
                    familySymbol = elementType as FamilySymbol;

                }


                if (familySymbol != null)
                {
                    family = doc.GetElement(familySymbol.Family.Id) as Family;

                }

                Transaction t = new Transaction(doc, "xx");
                t.Start();
                doc.Regenerate();
                t.Commit();


                Document familyDoc = doc.EditFamily(family);
                FilteredElementCollector connectores = new FilteredElementCollector(familyDoc).OfClass(typeof(ConnectorElement));
                IList<Element> connectorElements;
                connectorElements = connectores.ToElements();
                List<ConnectorElement> electrical_connectors = new List<ConnectorElement>();

                foreach (ConnectorElement c in connectorElements)
                {
                    if (c != null)
                    {
                        if (c.Domain == Domain.DomainElectrical)
                        {
                            electrical_connectors.Add(c);
                        }
                    }
                }




                Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;

                app.SharedParametersFilename = @"M:\_Master AutoCAD & REVIT\Revit\Shared Parameters\SE_Shared_Parameters.txt";
                DefinitionFile defFile = app.OpenSharedParameterFile();
                DefinitionGroups groups = defFile.Groups;
                DefinitionGroup electrical = groups.get_Item("Electrical");
                Definitions parameterDefinition = electrical.Definitions;

                ExternalDefinition apparent_load = null;
                ExternalDefinition voltage_first_ckt = null;
                ExternalDefinition number_of_poles = null;
                ExternalDefinition voltage_second_ckt = null;
                ExternalDefinition switchleg = null;
                ExternalDefinition voltage_text = null;
                ExternalDefinition wattage_text = null;
                ExternalDefinition manufacturer = null;
                ExternalDefinition model_number = null;
                ExternalDefinition lamp = null;
                ExternalDefinition description = null;



                foreach (ExternalDefinition e in parameterDefinition)
                {
                    if (e.Name == "SE_E_APPARENT LOAD")
                    {
                        apparent_load = e;

                    }
                    if (e.Name == "SE_E_VOLTAGE")
                    {
                        voltage_first_ckt = e;
                    }
                    if (e.Name == "SE_E_NUMBER OF POLES")
                    {
                        number_of_poles = e;
                    }
                    if (e.Name == "SE_E_VOLTAGE CKT 2")
                    {
                        voltage_second_ckt = e;
                    }
                    if (e.Name == "SE_E_SWITCHLEG")
                    {
                        switchleg = e;
                    }
                    if (e.Name == "SE_E_VOLTAGE TEXT")
                    {
                        voltage_text = e;
                    }
                    if (e.Name == "SE_E_WATTAGE TEXT")
                    {
                        wattage_text = e;
                    }
                    if (e.Name == "SE_E_MANUFACTURER")
                    {
                        manufacturer = e;
                    }
                    if (e.Name == "SE_E_MODEL_NUMBER")
                    {
                        model_number = e;
                    }
                    if (e.Name == "SE_E_LAMP")
                    {
                        lamp = e;
                    }
                    if (e.Name == "SE_E_DESCRIPTION")
                    {
                        description = e;
                    }
                }


                FamilyManager familyManager = familyDoc.FamilyManager;
                //FamilyParameter ApparentLoad_family = familyManager.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD);
                FamilyParameter Manufacturer = familyManager.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);
                FamilyParameter Description = familyManager.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);
                FamilyParameter Model = familyManager.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);




               


                //IList<Parameter> parameterList = new FilteredElementCollector(doc).OfClass.OfType(Parameter).ToList<Parameter>();

                ElementId elemId = elem.GetTypeId();

                FamilyParameter voltage_parameter = null;
                FamilyParameter apparent_load_parameter = null;

                FamilyParameter shared_param_apparent_load = null;
                FamilyParameter shared_param_voltage = null;
                FamilyParameter shared_param_number_of_poles = null;
                FamilyParameter shared_param_voltage_second_ckt = null;
                FamilyParameter shared_param_switchleg = null;
                FamilyParameter shared_param_voltage_text = null;
                FamilyParameter shared_param_wattage_text = null;
                FamilyParameter shared_param_manufacturer = null;
                FamilyParameter shared_param_model_number = null;
                FamilyParameter shared_param_lamp = null;
                FamilyParameter shared_param_description = null;


                FamilyParameter param = null;


                if ((familySymbol.LookupParameter("SE_E_VOLTAGE") == null) & electrical_connectors.Count > 0)
                {


                    Transaction t1 = new Transaction(familyDoc, "Adding VOLTAGE Parameter");
                    t1.Start();

                    IList<FamilyParameter> all_parameters = familyManager.GetParameters();
                    int length = all_parameters.Count;

                    IList<FamilyParameter> listOfParameters = familyManager.GetParameters();

                    foreach (FamilyParameter famParam in listOfParameters)
                    {
                        if (famParam.Definition.GetGroupTypeId() != GroupTypeId.Length || famParam.AssociatedParameters.IsEmpty == true || famParam.Definition.ParameterGroup != BuiltInParameterGroup.PG_CONSTRAINTS)
                        {
                            try
                            {
                                familyManager.RemoveParameter(famParam);
                            }

                            catch
                            { }
                        }
                    }

                    if ((Description.Formula != null) | (Manufacturer.Formula != null) | (Model.Formula != null))
                    {
                        try
                        {
                            if (Description.Formula != null)
                            {
                                familyManager.SetFormula(Description, null);
                                familyManager.Set(Description, "");
                            }
                            if (Manufacturer.Formula != null)
                            {
                                familyManager.SetFormula(Manufacturer, null);
                                familyManager.Set(Manufacturer, "");
                            }
                            if (Model.Formula != null)
                            {
                                familyManager.SetFormula(Model, null);
                                familyManager.Set(Model, "");
                            }
                        }

                        catch
                        {

                        }
                    }


                    try
                    {
                        shared_param_manufacturer = familyManager.AddParameter(manufacturer, GroupTypeId.Electrical, false);
                        shared_param_model_number = familyManager.AddParameter(model_number, GroupTypeId.Electrical, false);
                        shared_param_lamp = familyManager.AddParameter(lamp, GroupTypeId.Electrical, false);
                        shared_param_description = familyManager.AddParameter(description, GroupTypeId.Electrical, false);
                        shared_param_wattage_text = familyManager.AddParameter(wattage_text, GroupTypeId.Electrical, false);
                        shared_param_voltage_text = familyManager.AddParameter(voltage_text, GroupTypeId.Electrical, false);
                        shared_param_voltage_second_ckt = familyManager.AddParameter(voltage_second_ckt, GroupTypeId.Electrical, false);
                        shared_param_switchleg = familyManager.AddParameter(switchleg, GroupTypeId.Electrical, true);
                        shared_param_apparent_load = familyManager.AddParameter(apparent_load, GroupTypeId.Electrical, false);
                        shared_param_voltage = familyManager.AddParameter(voltage_first_ckt, GroupTypeId.Electrical, false);
                        shared_param_number_of_poles = familyManager.AddParameter(number_of_poles, GroupTypeId.Electrical, false);
                        

                        
                        
                        Parameter loadClassification = electrical_connectors[0].get_Parameter(BuiltInParameter.RBS_ELEC_LOAD_CLASSIFICATION);




                        try
                        {
                            familyManager.AssociateElementParameterToFamilyParameter(loadClassification, null);
                        }
                        catch
                        {

                        }

                        ElementId elec = loadClassification.AsElementId();
                        Element loadClassAsElement = familyDoc.GetElement(elec);
                        ElementId loadClassAsElementId = loadClassAsElement.Id;
                        ElectricalLoadClassification eLoadClass = loadClassAsElement as ElectricalLoadClassification;



                        if (eLoadClass.Name != "Lighting")
                        {
                            try
                            {
                                ElectricalLoadClassification lighting = ElectricalLoadClassification.Create(familyDoc, "Lighting");
                                ElementId eId = lighting.Id;
                                loadClassification.Set(eId);
                                //familyManager.Set(loadC, eId);
                            }

                            catch
                            {

                            }
                        }

                        if (eLoadClass.Name == "Lighting")
                        {
                            try
                            {
                                //familyManager.Set(loadC, loadClassAsElementId);
                            }
                            catch
                            {

                            }
                        }


                        //familyManager.SetFormula(ApparentLoad_family, null);



                        if (electrical_connectors[0].SystemClassification == MEPSystemClassification.PowerUnBalanced)
                        {
                            electrical_connectors[0].SystemClassification = MEPSystemClassification.PowerBalanced;
                            Parameter ApparentLoad_connector = electrical_connectors[0].LookupParameter("Apparent Load");
                            Parameter Voltage_Parameter = electrical_connectors[0].LookupParameter("Voltage");
                            Parameter NumberOfPoles_Parameter = electrical_connectors[0].LookupParameter("Number of Poles");
                            if (shared_param_voltage != null)
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(Voltage_Parameter, shared_param_voltage);
                            }
                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, shared_param_apparent_load);

                            }
                            catch
                            {
                                try
                                {
                                    familyManager.SetFormula(apparent_load_parameter, "[Apparent Load]");
                                    familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, apparent_load_parameter);
                                }
                                catch { }
                            }
                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(NumberOfPoles_Parameter, shared_param_number_of_poles);
                            }
                            catch { }
                            try
                            {
                                //familyManager.AssociateElementParameterToFamilyParameter(loadClassification, loadC);
                            }
                            catch
                            {

                            }

                        }

                        if (electrical_connectors[0].SystemClassification == MEPSystemClassification.PowerBalanced)
                        {
                            Parameter ApparentLoad_connector = electrical_connectors[0].LookupParameter("Apparent Load");
                            Parameter Voltage_Parameter = electrical_connectors[0].LookupParameter("Voltage");
                            Parameter NumberOfPoles_Parameter = electrical_connectors[0].LookupParameter("Number of Poles");
                            if (shared_param_voltage != null)
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(Voltage_Parameter, shared_param_voltage);
                            }
                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, shared_param_apparent_load);
                            }

                            catch
                            {
                                try
                                {
                                    familyManager.SetFormula(apparent_load_parameter, "[Apparent Load]");
                                    familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, apparent_load_parameter);
                                }

                                catch { }
                            }

                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(NumberOfPoles_Parameter, shared_param_number_of_poles);
                            }
                            catch { }

                            try
                            {
                                //familyManager.AssociateElementParameterToFamilyParameter(loadClassification, loadC);
                            }
                            catch
                            {

                            }

                        }

                        if ((electrical_connectors[0].SystemClassification == MEPSystemClassification.DataCircuit) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.FireAlarm) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.Controls) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.Communication) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.NurseCall) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.Telephone))
                        {

                        }

                        Family modifiedfamily = familyDoc.LoadFamily(doc, familyLoadOptions);

                        //familyDoc = doc.EditFamily(modifiedfamily);
                        //familyManager = familyDoc.FamilyManager;
                    }

                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }

                    t1.Commit();
                }

                if ((familySymbol.LookupParameter("SE_E_VOLTAGE") != null) & (electrical_connectors.Count > 0))
                {

                }

                if (familySymbol.LookupParameter("SE_E_VOLTAGE") == null && electrical_connectors.Count == 0)
                {
                    Transaction t2 = new Transaction(familyDoc, "adding VOLTAGE instance parameter");
                    t2.Start();

                    IList<FamilyParameter> listOfParameters = familyManager.GetParameters();

                    foreach (FamilyParameter famParam in listOfParameters)
                    {
                        if (famParam.Definition.GetGroupTypeId() != GroupTypeId.Length || famParam.AssociatedParameters.IsEmpty == true || famParam.Definition.ParameterGroup != BuiltInParameterGroup.PG_CONSTRAINTS)
                        {
                            try
                            {
                                familyManager.RemoveParameter(famParam);
                            }

                            catch
                            { }
                        }
                    }

                    if ((Description.Formula != null) | (Manufacturer.Formula != null) | (Model.Formula != null))
                    {
                        try
                        {
                            if (Description.Formula != null)
                            {
                                familyManager.SetFormula(Description, null);
                                familyManager.SetValueString(Description, "");
                            }
                            if (Manufacturer.Formula != null)
                            {
                                familyManager.SetFormula(Manufacturer, null);
                                familyManager.SetValueString(Description, "");
                            }
                            if (Model.Formula != null)
                            {
                                familyManager.SetFormula(Model, null);
                                familyManager.SetValueString(Description, "");
                            }
                        }

                        catch
                        {

                        }
                    }

                    shared_param_manufacturer = familyManager.AddParameter(manufacturer, GroupTypeId.Electrical, false);
                    shared_param_model_number = familyManager.AddParameter(model_number, GroupTypeId.Electrical, false);
                    shared_param_lamp = familyManager.AddParameter(lamp, GroupTypeId.Electrical, false);
                    shared_param_description = familyManager.AddParameter(description, GroupTypeId.Electrical, false);
                    shared_param_wattage_text = familyManager.AddParameter(wattage_text, GroupTypeId.Electrical, false);
                    shared_param_voltage_text = familyManager.AddParameter(voltage_text, GroupTypeId.Electrical, false);
                    shared_param_switchleg = familyManager.AddParameter(switchleg, GroupTypeId.Electrical, true);
                    shared_param_voltage_second_ckt = familyManager.AddParameter(voltage_second_ckt, GroupTypeId.Electrical, false);
                    shared_param_number_of_poles = familyManager.AddParameter(number_of_poles, GroupTypeId.Electrical, false);
                    shared_param_apparent_load = familyManager.AddParameter(apparent_load, GroupTypeId.Electrical, false);
                    shared_param_voltage = familyManager.AddParameter(voltage_first_ckt, BuiltInParameterGroup.PG_ELECTRICAL, false);
                    

                   

                    t2.Commit();



                    Transaction transaction = new Transaction(familyDoc, "adding electrical connector, linking parameters");
                    transaction.Start();

                    ConnectorElement connector = GetElectricalConnector(familyDoc);
                    FilteredElementCollector LC = new FilteredElementCollector(familyDoc).OfClass(typeof(ElectricalLoadClassification));
                    IList<Element> lCS = LC.ToElements();
                    ElementId elecLCId = null;
                    foreach (Element e in lCS)
                    {
                        ElectricalLoadClassification elecLC = e as ElectricalLoadClassification;
                        if (elecLC.Name == "Lighting")
                        {
                            elecLCId = elecLC.Id;
                            break;
                        }
                    }


                    if (connector != null)
                    {
                        Parameter loadClassification = connector.get_Parameter(BuiltInParameter.RBS_ELEC_LOAD_CLASSIFICATION);
                        if (elecLCId != null)
                        {
                            loadClassification.Set(elecLCId);
                        }

                        else
                        {
                            try
                            {
                                ElectricalLoadClassification lighting = ElectricalLoadClassification.Create(familyDoc, "Lighting");
                                ElementId eId = lighting.Id;
                                loadClassification.Set(eId);
                            }

                            catch
                            {

                            }


                        }



                        try
                        {

                            Parameter ApparentLoad_connector = connector.LookupParameter("Apparent Load");

                            Parameter NumberOfPoles_Parameter = connector.LookupParameter("Number of Poles");
                            Parameter Voltage_parameter = connector.LookupParameter("Voltage");
                            familyManager.AssociateElementParameterToFamilyParameter(Voltage_parameter, shared_param_voltage);
                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, shared_param_apparent_load);
                            }

                            catch
                            {
                                try
                                {
                                    familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, apparent_load_parameter);
                                    familyManager.SetFormula(apparent_load_parameter, "[Apparent Load]");
                                }
                                catch { }
                            }

                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(NumberOfPoles_Parameter, shared_param_number_of_poles);
                            }
                            catch { }

                        }

                        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    }

                    transaction.Commit();

                    familyDoc.LoadFamily(doc, familyLoadOptions);
                }
            }

            return Result.Succeeded;
        }



    }

    public class FamilyLoadOptions : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            // Set the behavior for when a family is found
            overwriteParameterValues = true; // Specify whether to overwrite parameter values

            // Return true to continue loading the family, or false to cancel loading
            return true;
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            throw new NotImplementedException();
        }
    }

}