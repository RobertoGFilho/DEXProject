using DexApi.Models;
using System;
using System.Collections.Generic;

namespace DexApi.Services
{
    public class DexParser : IDexParser
    {
        public (DexMeter, List<DexLaneMeter>) Parse(string content, char machine)
        {
            var lines = content.Split('*');
            var dexMeter = new DexMeter { Machine = machine };
            var lanes = new List<DexLaneMeter>();

            foreach (var line in lines)
            {
                if (line.StartsWith("ID101"))
                    dexMeter.MachineSerialNumber = line.Substring(5);
                else if (line.StartsWith("VA101"))
                    dexMeter.ValueOfPaidVends = decimal.Parse(line.Substring(5));
                else if (line.StartsWith("EA3"))
                    dexMeter.DEXDateTime = DateTime.UtcNow.AddMilliseconds(-DateTime.UtcNow.Millisecond);
                else if (line.StartsWith("PA101"))
                {
                    var lane = new DexLaneMeter
                    {
                        ProductIdentifier = line.Substring(5)
                    };
                    lanes.Add(lane);
                }
                else if (line.StartsWith("PA102"))
                    lanes[^1].Price = decimal.Parse(line.Substring(5));
                else if (line.StartsWith("PA201"))
                    lanes[^1].NumberOfVends = int.Parse(line.Substring(5));
                else if (line.StartsWith("PA202"))
                    lanes[^1].ValueOfPaidSales = decimal.Parse(line.Substring(5));
            }

            return (dexMeter, lanes);
        }
    }
}