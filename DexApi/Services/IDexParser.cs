using DexApi.Models;

namespace DexApi.Services
{
    public interface IDexParser
    {
        (DexMeter, List<DexLaneMeter>) Parse(string fileContent, char machine);
    }
}