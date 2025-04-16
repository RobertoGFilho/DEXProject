using DexApi.Models;

namespace DexApi.Services
{
    public class DexParser : IDexParser
    {
        public (DexMeter, List<DexLaneMeter>) Parse(string fileContent, char machine)
        {
            var lines = fileContent.Split('*', StringSplitOptions.RemoveEmptyEntries);
            var meter = new DexMeter { Machine = machine };
            var lanes = new List<DexLaneMeter>();

            foreach (var line in lines)
            {
                if (line.StartsWith("ID101"))
                {
                    meter.MachineSerialNumber = line.Substring(5);
                }
                else if (line.StartsWith("VA101"))
                {
                    if (decimal.TryParse(line.Substring(5), out var value))
                        meter.ValueOfPaidVends = value;
                }
                else if (line.StartsWith("DA101"))
                {
                    if (DateTime.TryParseExact(line.Substring(5), "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                        meter.DEXDateTime = parsedDate;
                    else
                        meter.DEXDateTime = DateTime.UtcNow;
                }
                else if (line.StartsWith("PA101"))
                {
                    var lane = new DexLaneMeter
                    {
                        ProductIdentifier = line.Substring(5)
                    };
                    lanes.Add(lane);
                }
                else if (line.StartsWith("PA102"))
                {
                    if (lanes.Any() && decimal.TryParse(line.Substring(5), out var price))
                        lanes.Last().Price = price;
                }
                else if (line.StartsWith("PA201"))
                {
                    if (lanes.Any() && int.TryParse(line.Substring(5), out var vendCount))
                        lanes.Last().NumberOfVends = vendCount;
                }
                else if (line.StartsWith("PA202"))
                {
                    if (lanes.Any() && decimal.TryParse(line.Substring(5), out var value))
                        lanes.Last().ValueOfPaidSales = value;
                }
            }

            return (meter, lanes);
        }
    }
}