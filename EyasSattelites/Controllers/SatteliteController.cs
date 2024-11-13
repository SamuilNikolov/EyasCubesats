using Microsoft.AspNetCore.Mvc;
using EyasSattelites.Services;
using System.Threading.Tasks;

namespace EyasSattelites.Controllers
{
    public class SatelliteController : Controller
    {
        private readonly SerialPortService _serialPortService;

        public SatelliteController(SerialPortService serialPortService)
        {
            _serialPortService = serialPortService;
        }

        // Action for sending a command
        [HttpPost]
        public async Task<IActionResult> SendCommand([FromBody] CommandModel commandModel) // Expect JSON body
        {
            if (commandModel == null || string.IsNullOrEmpty(commandModel.Command))
            {
                return Json(new { success = false, message = "Invalid command data received" });
            }

            try
            {
                var result = await _serialPortService.SendCommandAsync(commandModel.Command);

                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { success = true, message = "Command sent successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Return the main view for command and telemetry page
        public IActionResult Index()
        {
            return View();
        }
    }

    public class CommandModel
    {
        public string Command { get; set; } // This maps the incoming 'command' field from the JSON object
    }
}
