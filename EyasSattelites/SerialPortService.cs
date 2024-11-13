using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EyasSattelites.Services
{
    public class SerialPortService : IDisposable
    {
        private readonly SerialPort _serialPort;
        private StringBuilder _telemetryBuffer = new StringBuilder();
        private const string EndMarker = "----";
        private readonly Timer _connectionTimer;

        public string LatestTelemetry { get; private set; } = "No telemetry data yet.";

        public SerialPortService()
        {
            _serialPort = new SerialPort("COM4", 19200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 1000,
                WriteTimeout = 100
            };

            _connectionTimer = new Timer(CheckAndReconnect, null, 0, 5000); // Check every 5 seconds
            OpenPort();
        }

        private void OpenPort()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                LatestTelemetry = "COM PORT DISCONNECTED";
            }
        }

        public async Task<string> SendCommandAsync(string command)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    Console.WriteLine("COMMAND -> "+command);
                    _serialPort.DiscardInBuffer();
                    _serialPort.WriteLine(command);
                    return ""; // Success
                }
                return "Serial port is not open.";
            }
            catch (Exception ex)
            {
                return $"Error sending command: {ex.Message}";
            }
        }

        private void CheckAndReconnect(object state)
        {
            if (IsPortDisconnected())
            {
                LatestTelemetry = "COM PORT DISCONNECTED";
                OpenPort();
            }
        }

        private bool IsPortDisconnected()
        {
            try
            {
                if (!_serialPort.IsOpen) return true;
                _serialPort.ReadTimeout = 500;
                _serialPort.ReadByte(); // Check for disconnection
                return false;
            }
            catch (TimeoutException)
            {
                return false; // Timeout is expected
            }
            catch (Exception)
            {
                return true;
            }
        }

        public string ReadTelemetry()
        {
            if (IsPortDisconnected())
            {
                LatestTelemetry = "COM PORT DISCONNECTED";
                return LatestTelemetry;
            }

            try
            {
                if (_serialPort.IsOpen)
                {
                    string incomingData;
                    while ((incomingData = _serialPort.ReadExisting()) != string.Empty)
                    {
                        _telemetryBuffer.Append(incomingData);
                        ProcessBuffer();
                    }
                }
            }
            catch (TimeoutException)
            {
                LatestTelemetry = "Timeout occurred while reading telemetry.";
            }
            catch (Exception ex)
            {
                LatestTelemetry = $"Error reading telemetry: {ex.Message}";
            }

            return LatestTelemetry;
        }



        private void ProcessBuffer()
        {
            string bufferContent = _telemetryBuffer.ToString();

            while (bufferContent.Contains(EndMarker))
            {
                // Find the index of the end marker
                int endMarkerIndex = bufferContent.IndexOf(EndMarker);

                // Extract the complete telemetry message
                string completeTelemetry = bufferContent.Substring(0, endMarkerIndex).Trim();

                // Store the complete telemetry in LatestTelemetry
                LatestTelemetry = completeTelemetry;
                Console.WriteLine("Processed Telemetry: " + LatestTelemetry);

                // Remove the processed part from the buffer
                _telemetryBuffer.Remove(0, endMarkerIndex + EndMarker.Length);

                // Refresh bufferContent to reflect the updated buffer
                bufferContent = _telemetryBuffer.ToString();
            }
        }


        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
            _connectionTimer?.Dispose();
        }
    }
}