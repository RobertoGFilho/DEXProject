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
        // Database context for database operations
        private readonly ApplicationDbContext _context;
        // Service for parsing DEX file content
        private readonly IDexParser _parser;
        // Configuration for accessing app settings
        private readonly IConfiguration _config;
        // Logger for error handling and tracing
        private readonly ILogger<DexController> _logger;

        public DexController(ApplicationDbContext context, IDexParser parser, IConfiguration config, ILogger<DexController> logger)
        {
            _context = context;
            _parser = parser;
            _config = config;
            _logger = logger;
        }

        // Endpoint to receive and process DEX files
        // Validates authentication, parses DEX content, and saves to database
        [HttpPost("vdi-dex")]
        public async Task<IActionResult> Post([FromBody] DexFileModel model)
        {
            // Basic authentication validation
            if (!Request.Headers.ContainsKey("Authorization")) return Unauthorized();
            var authHeader = Request.Headers["Authorization"].ToString();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Replace("Basic ", ""))).Split(':');

            var validUser = _config["DexApiAuth:Username"];
            var validPass = _config["DexApiAuth:Password"];

            if (credentials.Length != 2 || credentials[0] != validUser || credentials[1] != validPass)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(model.FileContent))
                return BadRequest("DEX content is empty.");

            try
            {
                // Parse the DEX file content
                var (dexMeter, laneMeters) = _parser.Parse(model.FileContent, model.Machine);

                // Ensure the date is valid for SQL Server (must be >= 1753-01-01)
                if (dexMeter.DEXDateTime < new DateTime(1753, 1, 1))
                    dexMeter.DEXDateTime = DateTime.UtcNow;

                int dexMeterId;
                // Save DEX meter information using stored procedure
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

                // Save lane meter details for each product lane
                foreach (var lane in laneMeters)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC SaveDEXLaneMeter @p0, @p1, @p2, @p3, @p4",
                        dexMeterId, lane.ProductIdentifier, lane.Price, lane.NumberOfVends, lane.ValueOfPaidSales);
                }

                return Ok(new { message = "DEX processed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DEX file.");
                return StatusCode(500, "Internal error processing DEX file.");
            }
        }
    }
}