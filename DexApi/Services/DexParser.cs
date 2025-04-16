using DexApi.Models;
using System.Globalization;

namespace DexApi.Services
{
    public class DexParser : IDexParser
    {
        /// <summary>
        /// Parses DEX file content and extracts meter and lane information
        /// </summary>
        /// <param name="fileContent">Raw DEX file content</param>
        /// <param name="machine">Machine identifier ('A' or 'B')</param>
        /// <returns>Tuple containing DEX meter header and list of lane meters</returns>
        public (DexMeter, List<DexLaneMeter>) Parse(string fileContent, char machine)
        {
            // Initialize DEX meter with machine identifier and current timestamp
            var dexMeter = new DexMeter
            {
                Machine = machine,
                DEXDateTime = DateTime.UtcNow
            };

            // Split file content into lines and prepare lane collection
            var lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var lanes = new List<DexLaneMeter>();

            // Process each line of the DEX file
            foreach (var line in lines)
            {
                // Clean up line and replace asterisks with pipe symbols
                var clean = line.Trim().Replace("*", "|");

                // ID1 line contains machine serial number
                if (clean.StartsWith("ID1|"))
                {
                    dexMeter.MachineSerialNumber = clean.Split('|')[1];
                }
                // VA1 line contains total value of paid vends
                else if (clean.StartsWith("VA1|"))
                {
                    var parts = clean.Split('|');
                    if (parts.Length > 1 && decimal.TryParse(parts[1], out var total))
                        dexMeter.ValueOfPaidVends = total;
                }
                // PA7 lines contain product-level data (lanes)
                else if (clean.StartsWith("PA7|"))
                {
                    var parts = clean.Split('|');
                    if (parts.Length >= 7)
                    {
                        var productId = parts[1];
                        var price = parts[4];
                        var vendCount = parts[5];
                        var paidValue = parts[6];

                        // Create lane meter from extracted data
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