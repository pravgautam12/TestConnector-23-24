using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using TestConnector2.Electrical_Connectors_and_Parameters.Views;
using System.Windows.Interop;

namespace TestConnector2
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]


    public class CreateConnector : IExternalCommand

    {
        public static MyExternalEventHandler eventHandler { get; set; }
        public void ChangeFamilyParameters(FamilyManager familyManager, FamilyParameter parameter, ElementId id)
        {
            FamilyTypeSet familyTypeSet = familyManager.Types;
            foreach (FamilyType familyType in familyTypeSet)
            {
                familyManager.CurrentType = familyType;
                familyManager.Set(parameter, id);
            }
        }

        public ConnectorElement GetElectricalConnector(Document familyDoc)
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
            //getting the current Revit process
            IntPtr revitMainWindowHandle = SetWindowHandle.revitMainWindowHandle;
            if (revitMainWindowHandle == IntPtr.Zero)
            {
                TaskDialog.Show("Error", "Revit window not found");
            }

            CreateConnectorW connectorWindow = new CreateConnectorW(commandData);
            connectorWindow.Show();

            //setting the WPF window so that if the user clicks outside the window in Revit, the window does not disappear. 
            WindowInteropHelper helper = new WindowInteropHelper(connectorWindow);
            helper.Owner = revitMainWindowHandle;

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


    public class MyExternalEventHandler : IExternalEventHandler
    {
        public bool _newFile { get; set; }

        public MyExternalEventHandler(bool newFile)
        {
            _newFile = newFile;
        }

        public ExternalDefinition CreateSharedParameter(DefinitionGroup defGroup, string parameterName, ForgeTypeId specTypeId)
        {
            ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(parameterName, specTypeId);
            ExternalDefinition newSharedParameter = defGroup.Definitions.Create(options) as ExternalDefinition;
            return newSharedParameter;
        }
        public void Execute(UIApplication application)
        {
            CreateConnector newCreateConnectorClass = new CreateConnector();
            Document doc = DocumentInstance.Instance;


            string company = variables.company;

            ElementId lightingLoadClassificationId = null;

            Options options = new Options();
            options.ComputeReferences = true;

            FilteredElementCollector light_fixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_LightingFixtures).WhereElementIsElementType();
            IList<Element> loadClasses = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ElectricalLoadClassifications).ToElements();
            IList<Element> instances = light_fixtures.ToElements();

            foreach (Element element in loadClasses)
            {
                if (element.Name == "Lighting")
                {
                    lightingLoadClassificationId = element.Id;
                    break;
                }
            }

            IList<Element> lightFixtureTypes = light_fixtures.ToElements();


            IFamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();

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
            ExternalDefinition loadClassificationProjectDocument = null;


            Definitions parameterDefinition = null;


            Autodesk.Revit.ApplicationServices.Application app = uiApplication.UiApp.Application;

            
            if (_newFile == true)
            {
                app.SharedParametersFilename = @"C:\ProgramData\Autodesk\Revit\Addins\2021\Electrical Tab DLLs\Autometica_Shared_Parameters.txt";
                DefinitionFile defFile = app.OpenSharedParameterFile();

                if (defFile != null)
                {
                    DefinitionGroups groups = defFile.Groups;
                    DefinitionGroup electrical = groups.get_Item("AMCA_Electrical");
                    parameterDefinition = electrical.Definitions;

                    foreach (ExternalDefinition e in parameterDefinition)
                    {
                        if (e.Name == company + "APPARENT LOAD")
                        {
                            apparent_load = e;

                        }
                        if (e.Name == company + "VOLTAGE")
                        {
                            voltage_first_ckt = e;
                        }
                        if (e.Name == company + "NUMBER OF POLES")
                        {
                            number_of_poles = e;
                        }
                        if (e.Name == company + "VOLTAGE CKT 2")
                        {
                            voltage_second_ckt = e;
                        }
                        if (e.Name == company + "SWITCHLEG")
                        {
                            switchleg = e;
                        }
                        if (e.Name == company + "VOLTAGE TEXT")
                        {
                            voltage_text = e;
                        }
                        if (e.Name == company + "WATTAGE TEXT")
                        {
                            wattage_text = e;
                        }
                        if (e.Name == company + "MANUFACTURER")
                        {
                            manufacturer = e;
                        }
                        if (e.Name == company + "MODEL_NUMBER")
                        {
                            model_number = e;
                        }
                        if (e.Name == company + "LAMP")
                        {
                            lamp = e;
                        }
                        if (e.Name == company + "DESCRIPTION")
                        {
                            description = e;
                        }

                        if (e.Name == company + "LOAD CLASSIFICATION")
                        {
                            loadClassificationProjectDocument = e;
                        }
                    }
                }
            }

            if (_newFile == false)
            {
                DefinitionFile defFile = app.OpenSharedParameterFile();
                if (defFile == null)
                {
                    TaskDialog.Show("Shared Parameters File Not Found", "Please specify a shared parameters file in the project before running.");
                    return;
                }
                else
                {
                    DefinitionGroup elecGroup = defFile.Groups.get_Item("AMCA_Electrical");
                    if (elecGroup == null)
                    {
                        elecGroup = defFile.Groups.Create("AMCA_Electrical");
                        
                        apparent_load = CreateSharedParameter(elecGroup, company + "APPARENT LOAD", SpecTypeId.ApparentPower);
                        voltage_first_ckt = CreateSharedParameter(elecGroup, company + "VOLTAGE", SpecTypeId.ElectricalPotential);
                        number_of_poles = CreateSharedParameter(elecGroup, company + "NUMBER OF POLES", SpecTypeId.Int.NumberOfPoles);
                        voltage_second_ckt = CreateSharedParameter(elecGroup, company + "VOLTAGE CKT 2", SpecTypeId.ElectricalPotential);
                        switchleg = CreateSharedParameter(elecGroup, company + "SWITCHLEG", SpecTypeId.String.Text);
                        voltage_text = CreateSharedParameter(elecGroup, company + "VOLTAGE TEXT", SpecTypeId.String.Text);
                        wattage_text = CreateSharedParameter(elecGroup, company + "WATTAGE TEXT", SpecTypeId.String.Text);
                        manufacturer = CreateSharedParameter(elecGroup, company + "MANUFACTURER", SpecTypeId.String.Text);
                        model_number = CreateSharedParameter(elecGroup, company + "MODEL_NUMBER", SpecTypeId.String.Text);
                        lamp = CreateSharedParameter(elecGroup, company + "LAMP", SpecTypeId.String.Text);
                        description = CreateSharedParameter(elecGroup, company + "DESCRIPTION", SpecTypeId.String.Text);
                        loadClassificationProjectDocument = CreateSharedParameter(elecGroup, company + "LOAD CLASSIFICATION", SpecTypeId.Reference.LoadClassification);
                        
                        
                    }
                    else {
                        DefinitionGroups groups = defFile.Groups;
                        DefinitionGroup electrical = groups.get_Item("AMCA_Electrical");
                        parameterDefinition = electrical.Definitions;

                        foreach (ExternalDefinition e in parameterDefinition)
                        {
                            if (e.Name == company + "APPARENT LOAD")
                            {
                                apparent_load = e;

                            }
                            if (e.Name == company + "VOLTAGE")
                            {
                                voltage_first_ckt = e;
                            }
                            if (e.Name == company + "NUMBER OF POLES")
                            {
                                number_of_poles = e;
                            }
                            if (e.Name == company + "VOLTAGE CKT 2")
                            {
                                voltage_second_ckt = e;
                            }
                            if (e.Name == company + "SWITCHLEG")
                            {
                                switchleg = e;
                            }
                            if (e.Name == company + "VOLTAGE TEXT")
                            {
                                voltage_text = e;
                            }
                            if (e.Name == company + "WATTAGE TEXT")
                            {
                                wattage_text = e;
                            }
                            if (e.Name == company + "MANUFACTURER")
                            {
                                manufacturer = e;
                            }
                            if (e.Name == company + "MODEL_NUMBER")
                            {
                                model_number = e;
                            }
                            if (e.Name == company + "LAMP")
                            {
                                lamp = e;
                            }
                            if (e.Name == company + "DESCRIPTION")
                            {
                                description = e;
                            }

                            if (e.Name == company + "LOAD CLASSIFICATION")
                            {
                                loadClassificationProjectDocument = e;
                            }
                        }

                    }
                }
            }

           

           
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
            FamilyParameter shared_param_load_classification = null;

            foreach (Element elem in lightFixtureTypes)
            {
                Family family = null;
                string familyName = "";

                FamilySymbol familySymbol = elem as FamilySymbol;
                if (familySymbol != null)
                {
                    family = doc.GetElement(familySymbol.Family.Id) as Family;
                    familyName = family.Name;
                }

                Transaction t = new Transaction(doc, "creating connectors and associating parameters");

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

                FamilyManager familyManager = familyDoc.FamilyManager;

                ElementId elemId = elem.GetTypeId();

                if ((familySymbol.LookupParameter(company + "VOLTAGE") == null) & electrical_connectors.Count > 0)
                {
                    Transaction t1 = new Transaction(familyDoc, "Adding VOLTAGE Parameter");
                    t1.Start();

                    IList<FamilyParameter> all_parameters = familyManager.GetParameters();
                    int length = all_parameters.Count;

                    IList<FamilyParameter> listOfParameters = familyManager.GetParameters();

                    foreach (FamilyParameter famParam in listOfParameters)
                    {
                        try
                        {
                            if (famParam.GetUnitTypeId().Equals(UnitTypeId.Feet) || famParam.GetUnitTypeId().Equals(UnitTypeId.Inches) || famParam.GetUnitTypeId().Equals(UnitTypeId.FeetFractionalInches) || famParam.GetUnitTypeId().Equals(UnitTypeId.FractionalInches))

                            {
                                continue;
                            }

                            else
                            {
                                try
                                {
                                    familyManager.RemoveParameter(famParam);
                                }
                                catch { }
                            }
                        }

                        catch
                        {
                            try { familyManager.RemoveParameter(famParam); }
                            catch { }
                        }
                    }

                    Parameter loadClassification = null;
                    IList<Element> familyLoadClasses = new FilteredElementCollector(familyDoc).OfCategory(BuiltInCategory.OST_ElectricalLoadClassifications).ToElements();
                    ElementId firstLoadClass = familyLoadClasses[0].Id;

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

                        shared_param_load_classification = familyManager.AddParameter(loadClassificationProjectDocument, GroupTypeId.Electrical, false);


                        loadClassification = electrical_connectors[0].get_Parameter(BuiltInParameter.RBS_ELEC_LOAD_CLASSIFICATION);


                    }
                    catch { }

                    newCreateConnectorClass.ChangeFamilyParameters(familyManager, shared_param_load_classification, firstLoadClass);

                    try { familyManager.AssociateElementParameterToFamilyParameter(loadClassification, shared_param_load_classification); } catch { }


                    ElementId elec = loadClassification.AsElementId();
                    Element loadClassAsElement = familyDoc.GetElement(elec);
                    ElementId loadClassAsElementId = loadClassAsElement.Id;
                    ElectricalLoadClassification eLoadClass = loadClassAsElement as ElectricalLoadClassification;


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
                            familyManager.AssociateElementParameterToFamilyParameter(loadClassification, shared_param_load_classification);
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
                                ApparentLoad_connector = electrical_connectors[0].LookupParameter("Apparent Power");
                                familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, shared_param_apparent_load);
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
                            familyManager.AssociateElementParameterToFamilyParameter(loadClassification, shared_param_load_classification);
                        }
                        catch
                        {

                        }

                    }

                    if ((electrical_connectors[0].SystemClassification == MEPSystemClassification.DataCircuit) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.FireAlarm) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.Controls) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.Communication) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.NurseCall) | (electrical_connectors[0].SystemClassification == MEPSystemClassification.Telephone))
                    { }



                    Family modifiedfamily = familyDoc.LoadFamily(doc, familyLoadOptions);

                    t1.Commit();
                }

                if ((familySymbol.LookupParameter(company + "VOLTAGE") != null) & (electrical_connectors.Count > 0))
                { }

                if (familySymbol.LookupParameter(company + "VOLTAGE") == null && electrical_connectors.Count == 0)
                {
                    ElementId firstLoadClass = null;
                    IList<Element> familyLoadClasses = new FilteredElementCollector(familyDoc).OfCategory(BuiltInCategory.OST_ElectricalLoadClassifications).ToElements();
                    if (familyLoadClasses.Count > 0)
                    {
                        firstLoadClass = familyLoadClasses[0].Id;
                    }
                    else
                    {
                        try
                        {
                            Transaction addingLoadClassification = new Transaction(familyDoc, "adding new load classification");
                            addingLoadClassification.Start();
                            ElectricalLoadClassification lighting = ElectricalLoadClassification.Create(familyDoc, "Lighting");
                            firstLoadClass = lighting.Id;
                            addingLoadClassification.Commit();
                        }
                        catch { }
                    }

                    IList<FamilyParameter> listOfParameters = familyManager.GetParameters();

                    foreach (FamilyParameter famParam in listOfParameters)
                    {
                        try
                        {
                            if (famParam.GetUnitTypeId().Equals(UnitTypeId.Feet) || famParam.GetUnitTypeId().Equals(UnitTypeId.Inches) || famParam.GetUnitTypeId().Equals(UnitTypeId.FeetFractionalInches) || famParam.GetUnitTypeId().Equals(UnitTypeId.FractionalInches))

                            {
                                continue;
                            }

                            else
                            {
                                try
                                {
                                    familyManager.RemoveParameter(famParam);
                                }
                                catch { }
                            }
                        }

                        catch
                        {
                            try { familyManager.RemoveParameter(famParam); }
                            catch { }
                        }
                    }

                    Transaction t2 = new Transaction(familyDoc, "adding VOLTAGE instance parameter");
                    t2.Start();
                    try
                    {
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
                        shared_param_voltage = familyManager.AddParameter(voltage_first_ckt, GroupTypeId.Electrical, false);

                        shared_param_load_classification = familyManager.AddParameter(loadClassificationProjectDocument, GroupTypeId.Electrical, false);
                    }
                    catch { }

                    newCreateConnectorClass.ChangeFamilyParameters(familyManager, shared_param_load_classification, firstLoadClass);

                    t2.Commit();

                    Transaction transaction = new Transaction(familyDoc, "adding electrical connector, linking parameters");
                    transaction.Start();

                    ConnectorElement connector = newCreateConnectorClass.GetElectricalConnector(familyDoc);

                    if (connector != null)
                    {
                        Parameter loadClassification = connector.get_Parameter(BuiltInParameter.RBS_ELEC_LOAD_CLASSIFICATION);

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
                                    ApparentLoad_connector = connector.LookupParameter("Apparent Power");
                                    familyManager.AssociateElementParameterToFamilyParameter(ApparentLoad_connector, shared_param_apparent_load);
                                }
                                catch { }
                            }

                            try
                            {
                                familyManager.AssociateElementParameterToFamilyParameter(NumberOfPoles_Parameter, shared_param_number_of_poles);
                            }
                            catch { }

                            try { familyManager.AssociateElementParameterToFamilyParameter(loadClassification, shared_param_load_classification); } catch { }

                        }

                        catch (Exception x) { Console.WriteLine(x.ToString()); }
                    }

                    transaction.Commit();

                    familyDoc.LoadFamily(doc, familyLoadOptions);
                }
            }

            IList<FamilySymbol> LFFamilySymbols = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_LightingFixtures).WhereElementIsElementType().Cast<FamilySymbol>().ToList();
            Transaction t3 = new Transaction(doc, "changing load classification to lighting");
            t3.Start();
            foreach (FamilySymbol f in LFFamilySymbols)
            {
                f.LookupParameter(company + "LOAD CLASSIFICATION").Set(lightingLoadClassificationId);
                f.LookupParameter(company + "VOLTAGE TEXT").Set("UNV");
                f.LookupParameter(company + "VOLTAGE").SetValueString("120");
            }
            t3.Commit();

        }

        public string GetName() => "External Event Handler";
        
    }

    

}