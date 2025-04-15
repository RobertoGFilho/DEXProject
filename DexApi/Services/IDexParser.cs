using DexApi.Models;
using System.Collections.Generic;

namespace DexApi.Services
{
    public interface IDexParser
    {
        (DexMeter, List<DexLaneMeter>) Parse(string content, char machine);
    }
}