using Microsoft.AspNetCore.Mvc;
using DexApi.Models;
using DexApi.Services;
using DexApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DexApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DexController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDexParser _parser;
        private const string ValidUser = "vendsys";
        private const string ValidPass = "NFsZGmHAGWJSZ#RuvdiV";

        public DexController(ApplicationDbContext context, IDexParser parser)
        {
            _context = context;
            _parser = parser;
        }

        [HttpPost("vdi-dex")]
        public async Task<IActionResult> Post([FromBody] DexFileModel model)
        {
            if (!Request.Headers.ContainsKey("Authorization")) return Unauthorized();

            var authHeader = Request.Headers["Authorization"].ToString();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Replace("Basic ", ""))).Split(':');
            if (credentials.Length != 2 || credentials[0] != ValidUser || credentials[1] != ValidPass)
                return Unauthorized();

            var (dexMeter, laneMeters) = _parser.Parse(model.FileContent, model.Machine);

            if (dexMeter.DEXDateTime < new DateTime(1753, 1, 1))
                dexMeter.DEXDateTime = DateTime.UtcNow;

            int dexMeterId;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC SaveDEXMeter @Machine, @DEXDateTime, @Serial, @Value";
                command.CommandType = System.Data.CommandType.Text;

                var p1 = command.CreateParameter();
                p1.ParameterName = "@Machine";
                p1.Value = dexMeter.Machine;
                command.Parameters.Add(p1);

                var p2 = command.CreateParameter();
                p2.ParameterName = "@DEXDateTime";
                p2.Value = dexMeter.DEXDateTime;
                command.Parameters.Add(p2);

                var p3 = command.CreateParameter();
                p3.ParameterName = "@Serial";
                p3.Value = dexMeter.MachineSerialNumber;
                command.Parameters.Add(p3);

                var p4 = command.CreateParameter();
                p4.ParameterName = "@Value";
                p4.Value = dexMeter.ValueOfPaidVends;
                command.Parameters.Add(p4);

                await _context.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();
                dexMeterId = Convert.ToInt32(result);
            }

            foreach (var lane in laneMeters)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SaveDEXLaneMeter @p0, @p1, @p2, @p3, @p4",
                    dexMeterId, lane.ProductIdentifier, lane.Price, lane.NumberOfVends, lane.ValueOfPaidSales);
            }

            return Ok(new { message = "DEX processed successfully." });
        }
    }
}