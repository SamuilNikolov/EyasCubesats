using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EyasSattelites.Services;

namespace EyasSattelites.Controllers
{
    public class CommandController : Controller
    {
        private readonly SerialPortService _serialPortService;
        private bool useFakeTelemetry = true;  // Toggle between real and fake telemetry
        private const double ErrorRate = 0.25; // 5% error rate

        public CommandController(SerialPortService serialPortService)
        {
            _serialPortService = serialPortService;
        }

        [HttpPost]
        public async Task<IActionResult> SendCommand(string command)
        {
            var result = await _serialPortService.SendCommandAsync(command);
            return Json(new { success = true, message = result });
        }



        // Endpoint for real telemetry data
        [HttpGet("real-telemetry")]
        public IActionResult GetRealTelemetry()
        {
            try
            {
                _serialPortService.ReadTelemetry(); // Read and process real telemetry
                Console.WriteLine($"Real Telemetry Sent to Frontend: {_serialPortService.LatestTelemetry}");
                return Json(new { telemetry = _serialPortService.LatestTelemetry });
            }
            catch (Exception ex)
            {
                return Json(new { telemetry = $"Error reading telemetry: {ex.Message}" });
            }
        }
        [HttpGet("fake-telemetry")]
        public IActionResult GetFakeTelemetry()
        {
            try
            {
                var fakeTelemetry = GenerateFakeTelemetry();
                Console.WriteLine($"Fake Telemetry Sent to Frontend: {fakeTelemetry}");
                return Json(new { telemetry = fakeTelemetry });
            }
            catch (Exception ex)
            {
                return Json(new { telemetry = $"Error generating fake telemetry: {ex.Message}" });
            }
        }


        [HttpGet]
        public IActionResult GetTelemetry()
        {
            try
            {
                if (useFakeTelemetry)
                {
                    var fakeTelemetry = GenerateFakeTelemetry();
                    Console.WriteLine($"Fake Telemetry Sent to Frontend: {fakeTelemetry}");
                    return Json(new { telemetry = fakeTelemetry });
                }
                else
                {
                    _serialPortService.ReadTelemetry(); // Read and process real telemetry
                    Console.WriteLine($"Telemetry Sent to Frontend: {_serialPortService.LatestTelemetry}");
                    return Json(new { telemetry = _serialPortService.LatestTelemetry });
                }
            }
            catch (Exception ex)
            {
                return Json(new { telemetry = $"Error reading telemetry: {ex.Message}" });
            }
        }

        // Method to generate fake telemetry data
        // Method to generate fake telemetry data\

        private string GenerateFakeTelemetry()
        {
            var random = new Random();

            // Timestamp for the telemetry entry
            var timeStamp = DateTime.Now.ToString("HH:mm:ss");

            // Section I: General status telemetry
            int telemDelay = random.Next(1, 3); // 1 or 2
            int cmdTimeout = random.Next(0, 2); // 0 or 1
            int pwr = random.Next(0, 2); // 0 (bad) or 1 (nominal)
            int adcs = random.Next(0, 2); // 0 (bad) or 1 (nominal)
            int exp1 = random.Next(0, 2); // 0 or 1

            // Section T: Temperature telemetry
            double dh = Math.Round(random.NextDouble() * 30, 1); // DH value
            double exp = Math.Round(random.NextDouble() * -5, 1); // Exp temperature, negative values
            double referenceTemp = Math.Round(random.NextDouble() * 5 + 20, 1); // Reference temperature
            double panelA = Math.Round(random.NextDouble() * 5 + 20, 1); // Panel A temperature
            double panelB = Math.Round(random.NextDouble() * 5 + 20, 1); // Panel B temperature
            double baseTemp = Math.Round(random.NextDouble() * 5 + 20, 1); // Base temperature
            double topA = Math.Round(random.NextDouble() * 5 + 20, 1); // Top A temperature
            double topB = Math.Round(random.NextDouble() * 5 + 20, 1); // Top B temperature

            // Section P: Power subsystem telemetry
            double sep = random.Next(0, 2); // Sep 0 or 1
            double vBatt = Math.Round(random.NextDouble() * 0.5, 2); // Battery voltage
            double iBatt = Math.Round(random.NextDouble() * 20, 2); // Battery current
            double vSA = Math.Round(random.NextDouble() * 5, 2); // Solar array voltage
            double iSA = Math.Round(random.NextDouble() * -2, 2); // Solar array current, negative values
            double iMB = Math.Round(random.NextDouble() * 100, 2); // Main bus current
            double v5v = 5.03; // Constant 5V bus
            double i5v = Math.Round(random.NextDouble() * 200, 2); // 5V current
            double v3v = 3.31; // Constant 3.3V bus
            double i3v = Math.Round(random.NextDouble() * 5, 2); // 3.3V current
            int status = random.Next(0, 256); // Status as a hex-like value
            double tBatt = Math.Round(random.NextDouble() * 5 + 20, 2); // Battery temperature
            double tSA1 = Math.Round(random.NextDouble() * 5 + 20, 2); // Solar array 1 temperature
            double tSA2 = Math.Round(random.NextDouble() * -5, 2); // Solar array 2 temperature, negative

            // Section A: ADCS telemetry
            int s_T = random.Next(0, 300); // Sensor temperature
            int s_B = random.Next(0, 100); // Sensor B value
            int s0 = random.Next(0, 50); // Sensor 0
            int s90 = random.Next(0, 50); // Sensor 90
            int s180 = random.Next(50, 120); // Sensor 180
            int s270 = random.Next(0, 100); // Sensor 270
            double yaw = Math.Round(random.NextDouble() * 360, 1); // Yaw angle
            double sa = Math.Round(random.NextDouble(), 1); // Some sensor angle
            double mX = Math.Round(random.NextDouble(), 3); // Magnetometer X
            double mY = Math.Round(random.NextDouble(), 3); // Magnetometer Y
            double mZ = Math.Round(random.NextDouble(), 3); // Magnetometer Z
            double aX = Math.Round(random.NextDouble(), 3); // Accelerometer X
            double aY = Math.Round(random.NextDouble(), 3); // Accelerometer Y
            double aZ = Math.Round(random.NextDouble(), 3); // Accelerometer Z
            int x = random.Next(0, 2); // X control
            int y = random.Next(0, 2); // Y control
            double rpsOut = Math.Round(random.NextDouble(), 1); // Reaction wheel output
            double rpsCmd = Math.Round(random.NextDouble(), 1); // Reaction wheel command
            int pwmOut = random.Next(0, 255); // PWM output
            int alg = random.Next(0, 2); // Algorithm status
            double p = 5.0; // Proportional gain
            double i = 2.0; // Integral gain
            double d = 2.0; // Derivative gain
            int deltaT = random.Next(1, 3); // Delta time
            double db = 10.0; // Deadband
            double g = 14.0; // Gravity factor
            double offset = 15.0; // Offset value
            double e = 10.0; // Error value
            double hyst = 0.2; // Hysteresis

            // Constructing the formatted fake telemetry output
            string fakeTelemetry = $"ES0 {timeStamp} I: TelemDelay={telemDelay} CmdTimeOut={cmdTimeout} Pwr={pwr} ADCS={adcs} Exp1={exp1}\n" +
                                   $"ES0 {timeStamp} T: DH={dh} Exp={exp} Ref={referenceTemp} Panel_A={panelA} Panel_B={panelB} Base={baseTemp} Top_A={topA} Top_B={topB}\n" +
                                   $"ES0 {timeStamp} P: Sep={sep} , V_Batt={vBatt} , I_Batt={iBatt} , V_SA={vSA} , I_SA={iSA} , I_MB={iMB} , V_5v={v5v} , I_5v={i5v} , V_3v={v3v} , I_3v={i3v} , S=0x{status:X2} , T_Batt={tBatt} , T_SA1={tSA1} , T_SA2={tSA2}\n" +
                                   $"ES0 {timeStamp} A: s_T={s_T} , s_B={s_B} , s0={s0} , s90={s90} , s180={s180} , s270={s270} , yaw={yaw} , sa={sa} , M_X={mX} , M_Y={mY} , M_Z={mZ} , A_X={aX} , A_Y={aY} , A_Z={aZ} , X={x} , Y={y} , rps_out={rpsOut} , rps_cmd={rpsCmd} , PWM_out={pwmOut} , alg={alg} , P={p} , I={i} , D={d} , deltaT={deltaT} , db={db} , g={g} , offset={offset} , e={e} , hyst={hyst}\n" +
                                   $"ES0 {timeStamp} ----";

            return fakeTelemetry;
        }

    }
}
