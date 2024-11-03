using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConnector2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class FixBreakerAndWireSizes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            IList<Element> electricalSystemsElements = new FilteredElementCollector(DocumentInstance.Instance).OfClass(typeof(ElectricalSystem)).ToElements();
            string breakerSizeNotEnoughMessage = "";

           foreach (Element element in electricalSystemsElements)
            {
                ElectricalSystem elecSys = element as ElectricalSystem;

                if (elecSys != null)
                {
                    double tripRating = elecSys.Rating;
                    double current = elecSys.ApparentCurrent;
                    double apparentLoad = elecSys.ApparentLoad;
                    string loadClassifications = elecSys.LoadClassifications;
                    string circuitNumber = elecSys.CircuitNumber;
                    
                    int hotConductorsNumber = elecSys.HotConductorsNumber;
                    Location location = elecSys.Location;
                    FamilyInstance panel = elecSys.BaseEquipment;


                    if (current * 1.25 > tripRating)
                    {
                        ChangePanelScheduleAppearance(elecSys, panel, circuitNumber, true);
                    }
                    else if (current*1.25 < tripRating)
                    {
                        ChangePanelScheduleAppearance(elecSys, panel, circuitNumber, false);
                    }
                }
            }
            return Result.Succeeded;
        }

        private void ChangePanelScheduleAppearance(ElectricalSystem e, FamilyInstance panelboard, string circuitNumber, bool isOverloaded)
        {
            if (panelboard == null) return;



            PanelScheduleView panelScheduleView = new FilteredElementCollector(DocumentInstance.Instance).OfClass(typeof(PanelScheduleView)).Cast<PanelScheduleView>().FirstOrDefault(
                    view => view.GetPanel() == panelboard.Id);

            if (panelScheduleView == null) return;
            ElementId panelTemplateId = panelScheduleView.GetTemplate();
            PanelScheduleTemplate panelTemplate = DocumentInstance.Instance.GetElement(panelTemplateId) as PanelScheduleTemplate;

            //Color pink = new Color(255, 192, 203);

            Color pink = new Color(255, 255, 255);

            TableSectionData sectionData = panelScheduleView.GetSectionData(SectionType.Body);

            int rowIndex = -1;
            bool isOdd = int.Parse(circuitNumber.Split(',')[0]) % 2 != 0;
            
            int columnNumber = 0;

            if (!panelTemplate.IsSwitchboardSchedule)
            {
                columnNumber = isOdd ? sectionData.FirstColumnNumber : sectionData.LastColumnNumber;
            }
            else { columnNumber = sectionData.FirstColumnNumber; }

            
            for (int i = 1; i < sectionData.NumberOfRows; i++)
            {
                

                string paramValue = "";
                try { paramValue = panelScheduleView.GetParamValue(SectionType.Body, i, columnNumber); } catch { }
                
                IList<TableCellCombinedParameterData> tableCellCombinedParameterDatas = sectionData.GetCellCombinedParameters(i, 1);

                if (circuitNumber == paramValue && !string.IsNullOrEmpty(paramValue))
                {
                    rowIndex = i; break;
                }
                
            }

            int beginning = 0;
            int finishing = 0;

            if (isOdd) { beginning = sectionData.FirstColumnNumber; finishing = sectionData.LastColumnNumber; }
            else { beginning = sectionData.LastColumnNumber; finishing = sectionData.FirstColumnNumber; }

            if (rowIndex != -1)
            {
                TableCellStyle specificCellStyle = sectionData.GetTableCellStyle(rowIndex, sectionData.FirstColumnNumber);
                ElementId lineStyle = specificCellStyle.BorderBottomLineStyle;

                TableData tableData = panelScheduleView.GetTableData();
                TableCellStyle cellStyle = new TableCellStyle();
                cellStyle.BorderBottomLineStyle = lineStyle;
                cellStyle.BorderTopLineStyle = lineStyle;
                cellStyle.BorderLeftLineStyle = lineStyle;
                cellStyle.BorderRightLineStyle = lineStyle;

                Transaction t = new Transaction(DocumentInstance.Instance, "test");
                t.Start();

                for (int col = beginning; isOdd? col < finishing : col >= finishing;)
                {
                    string paramValue = "";
                    try { paramValue = panelScheduleView.GetParamValue(SectionType.Body, rowIndex, col); } catch { }

                    if (paramValue != "")
                    {
                         
                        string[] strings = paramValue.Split(' ');
                        
                        if (strings.Length == 2 && strings[1] == "A")
                        {
                            TableCellStyle overrideStyle = new TableCellStyle();
                            overrideStyle.ResetOverride();
                            cellStyle.BorderBottomLineStyle = lineStyle;
                            cellStyle.BorderTopLineStyle = lineStyle;
                            cellStyle.BorderRightLineStyle = lineStyle;
                            cellStyle.BorderLeftLineStyle = lineStyle;

                            ApplyStyles(e.PolesNumber, rowIndex, col, cellStyle, sectionData, isOverloaded, panelTemplate.IsSwitchboardSchedule);
                        }
                    }
                    if (isOdd) { col++; }
                    else { col--; }
                }
                t.Commit();
            }
            
        }

        private void ApplyStyles (int numberOfPoles, int row, int column, TableCellStyle cellStyle, TableSectionData sectionData, bool isOverloaded, bool isSwitchBoardSchedule)
        {
            if (isOverloaded)
            {
                cellStyle.BackgroundColor = new Color(255, 192, 203);
            }

            if (!isSwitchBoardSchedule)
            {
                for (int i = 0; i < numberOfPoles; i++)
                {
                    sectionData.SetCellStyle(row + i, column, cellStyle);
                }
            }
            else
            {
                sectionData.SetCellStyle(row, column, cellStyle);
            }
            
        }

    }
}
