using DexApi.Models;
using System.Globalization;

namespace DexApi.Services
{
    public class DexParser : IDexParser
    {
        public (DexMeter, List<DexLaneMeter>) Parse(string fileContent, char machine)
        {
            var dexMeter = new DexMeter
            {
                Machine = machine,
                DEXDateTime = DateTime.UtcNow
            };

            var lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var lanes = new List<DexLaneMeter>();

            foreach (var line in lines)
            {
                var clean = line.Trim().Replace("*", "|");

                if (clean.StartsWith("ID1|"))
                {
                    dexMeter.MachineSerialNumber = clean.Split('|')[1];
                }
                else if (clean.StartsWith("VA1|"))
                {
                    var parts = clean.Split('|');
                    if (parts.Length > 1 && decimal.TryParse(parts[1], out var total))
                        dexMeter.ValueOfPaidVends = total;
                }
                else if (clean.StartsWith("PA7|"))
                {
                    var parts = clean.Split('|');
                    if (parts.Length >= 7)
                    {
                        var productId = parts[1];
                        var price = parts[4];
                        var vendCount = parts[5];
                        var paidValue = parts[6];

                        var lane = new DexLaneMeter
                        {
                            ProductIdentifier = productId,
                            Price = decimal.TryParse(price, out var p) ? p : 0,
                            NumberOfVends = int.TryParse(vendCount, out var v) ? v : 0,
                            ValueOfPaidSales = decimal.TryParse(paidValue, out var val) ? val : 0
                        };

                        lanes.Add(lane);
                    }
                }
            }

            return (dexMeter, lanes);
        }
    }
}