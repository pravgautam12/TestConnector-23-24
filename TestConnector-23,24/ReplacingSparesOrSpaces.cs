using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace TestConnector2
{
    public class ReplacingSparesOrSpaces
    {
        public Double factor { get; set; }
        public Double voltage { get; set; }
        public FamilyInstance electricalPanel;
        public Document doc;

        public ReplacingSparesOrSpaces(double factor, double voltage, FamilyInstance electricalPanel, Document doc)
        {
            this.factor = factor;
            this.voltage = voltage;
            this.electricalPanel = electricalPanel;
            this.doc = doc;
        }



        public void CircuitWhenSparesOrSpaces(ElectricalSystem elecSys)
        {
            MEPModel x = electricalPanel.MEPModel;
            ISet<ElectricalSystem> ElectricalSystemSet = x.GetElectricalSystems();
            ElectricalSystem first = ElectricalSystemSet.FirstOrDefault();
            List<ElectricalSystem> sortedElectricalSystems = ElectricalSystemSet.Where(e => e.LoadName == "SPARE" || e.LoadName == "SPACE").OrderBy(e => e.StartSlot).ToList();

            if (factor == 1)
            {
                for (int i = 0; i < sortedElectricalSystems.Count; i++)
                {
                    if (i != sortedElectricalSystems.Count - 1)
                    {

                        if (voltage == 120 || voltage == 277)
                        {
                            doc.Delete(sortedElectricalSystems[i].Id);
                            try
                            {
                                elecSys.SelectPanel(electricalPanel);
                                break;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else if (voltage == 208 || voltage == 240 || voltage == 480)
                        {
                            if (sortedElectricalSystems[i + 2].StartSlot - sortedElectricalSystems[i].StartSlot == 2)
                            {
                                doc.Delete(sortedElectricalSystems[i].Id);
                                doc.Delete(sortedElectricalSystems[i + 2].Id);
                                try
                                {
                                    elecSys.SelectPanel(electricalPanel);
                                    break;
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }

            }
            //

            if (factor == Math.Sqrt(3))
            {
                for (int i = 0; i < sortedElectricalSystems.Count; i++)
                {
                    if (voltage == 240 || voltage == 208 || voltage == 480)
                    {
                        if (sortedElectricalSystems[i + 2].StartSlot - sortedElectricalSystems[i].StartSlot == 2 && sortedElectricalSystems[i + 4].StartSlot - sortedElectricalSystems[i + 2].StartSlot == 2)
                        {
                            doc.Delete(sortedElectricalSystems[i].Id);
                            doc.Delete(sortedElectricalSystems[i + 2].Id);
                            doc.Delete(sortedElectricalSystems[i + 4].Id);
                            try
                            {
                                elecSys.SelectPanel(electricalPanel);
                                break;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }


        }

    }
}

