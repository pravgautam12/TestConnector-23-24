using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConnector2
{
    public class CircuitInformation
    {
        public string amps_name { get; set; }
        public double current { get; set; }
        public string volts_name { get; set; }
        public double voltage { get; set; }
        public double factor { get; set; }
        public string phase_name { get; set; }
        public double BreakerRating { get; set; }

        public void CalculateCircuitParameters(string MOCP, string KW, string FLA, string MCA)
        {
            if (MOCP != "" && KW == "")
            {
                Double MOCP_Double = Double.Parse(MOCP);
                amps_name = GetAmpsName(MOCP_Double);

                if (!string.IsNullOrEmpty(FLA))
                {
                    current = double.Parse(FLA);
                }

                if (!string.IsNullOrEmpty(MCA) && string.IsNullOrEmpty(FLA))
                {
                    current = double.Parse(MCA) * 0.8;
                }

                BreakerRating = Double.Parse(MOCP);

            }

            if (MOCP == "" && KW != "")
            {
                current = (Double.Parse(KW) * 1000) / (factor * voltage);
                Double hundredTwentyFivePercent = current * 1.25;
                amps_name = GetAmpsName(hundredTwentyFivePercent);
                BreakerRating = GetBreakerSize(hundredTwentyFivePercent);
            }

        }


        public double GetBreakerSize(double x)
        {
            List<double> stdBrkrSizes = new List<double> { 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 110, 125, 150, 175, 200 };

            foreach (double size in stdBrkrSizes)
            {
                if (x <= size)
                {
                    return size;
                }
            }

            return 0.0;
        }
        public string GetAmpsName(double MOCP_Double)
        {
            var ampsMap = new Dictionary<double, string>
            {
                { 30, "30A" },
                { 60, "60A" },
                { 100, "100A" },
                { 200, "200A" },
                { 400, "400A" }
            };

            foreach (var key in ampsMap.Keys)
            {
                if (MOCP_Double <= key)
                {
                    return ampsMap[key];
                }
            }
            return null;
        }

        public void SetVoltageAndFactor(string voltageAndPhase)
        {
            String v = null;
            int volts = 0;
            try
            {
                v = voltageAndPhase.Split('/')[0];
                phase_name = voltageAndPhase.Split('/')[1];
            }
            catch
            {
                TaskDialog.Show("Format Error", "Please let the mechanical engineer know that voltage and phase parameter is not in the correct format");
            }

            try
            {
                volts = int.Parse(v);
            }
            catch
            {
                TaskDialog.Show("Parsing error", "Could not parse voltageAndPhase parameter");
            }

            var voltsMap = new Dictionary<double, double>
            {
                {130, 120 },
                {220, 208 },
                {260, 240 },
                {290, 277},
                {490, 480}
            };

            foreach (var key in voltsMap.Keys)
            {
                if (volts <= key)
                {
                    voltage = voltsMap[key];
                    volts_name = voltsMap[key].ToString();
                    break;
                }
            }

            if (phase_name == "1")
            {
                factor = 1;
            }
            else if (phase_name == "3")
            {
                factor = Math.Sqrt(3);
            }
        }

        public string GetWireSize(double Rating)
        {
            string unit = "";
            var WireSizeMap = new Dictionary<double, string>
            {
                {50, "#6, 1#10G, 3/4\"C"},
                {60, "#4, 1#10G, 1\"C"},
                {70, "#4, 1#8G, 1\"C" },
                {80, "#3, 1#8G, 1\"C" },
                {90, "#2, 1#8G, 1-1/4\"C" },
                {100, "#1, 1#8G, 1-1/4\"C" },
                {110, "#2, 1#6G, 1-1/4\"C" },
                {125, "#1, 1#6G, 1-1/4\"C" },
                {150, "#1/0, 1#6G, 1-1/2\"C" },
                {175, "#2/0, 1#6G, 1-1/2\"C"},
                {200, "#3/0, 1#6G, 2\"C" }
            };

            foreach (var key in WireSizeMap.Keys)
            {
                if (Rating <= key && Rating > 40)
                {
                    if (phase_name == "1")
                    {
                        if (voltage == 120 || voltage == 277)
                        {
                            return " 2" + WireSizeMap[key];
                        }

                        if (voltage == 208 || voltage == 240 || voltage == 480)
                        {
                            return "\n2" + WireSizeMap[key];
                        }
                    }

                    if (phase_name == "3")
                    {
                        return "\n3" + WireSizeMap[key];

                    }
                    break;
                }

            }

            return "";
        }
    }
}
