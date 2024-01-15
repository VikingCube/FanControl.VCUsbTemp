using FanControl.Plugins;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Diagnostics.Eventing.Reader;

namespace FanControl.VCUsbTemp
{
    internal class VCUsbTempPlugin : IPlugin
    {
        public static readonly string LOG_PREFIX = "[FanControl.VCUsbTemp] ";
        public string Name => "VC USB Temp";

        private readonly IPluginLogger _logger;
        private readonly IPluginDialog _dialog;
        private readonly List<VCUsbTempPluginSensor> _sensors;
        private readonly Dictionary<string, VCUsbTempSerialPort> _serialports;
        
        public VCUsbTempPlugin(IPluginLogger logger, IPluginDialog dialog)
        {
            _logger = logger;
            _dialog = dialog;
            _sensors = new List<VCUsbTempPluginSensor>();
            _serialports = new Dictionary<string, VCUsbTempSerialPort>();
        }

        public void Initialize()
        {
            string cwd_path = Directory.GetCurrentDirectory();
            var config_directory_path = Path.Combine(cwd_path, "Configurations");
            var config_json_path = Path.Combine(config_directory_path, "FanControl.VCUsbTemp.json");

            try
            {
                string tempSensorConfigRaw = File.ReadAllText(config_json_path);
                var tempSensorConfig = JsonConvert.DeserializeObject<IList<VCUsbTempSensorConfig>>(tempSensorConfigRaw);

                foreach (VCUsbTempSensorConfig sensor_port in tempSensorConfig)
                {
                    if (!_serialports.ContainsKey(sensor_port.device_id))
                    {
                        _serialports[sensor_port.device_id] = new VCUsbTempSerialPort(sensor_port.device_id);
                    }
                    _sensors.Add(new VCUsbTempPluginSensor(sensor_port, _serialports[sensor_port.device_id]));
                }
            }
            catch (Exception e)
            {
                var error_message = $"Error: Could not parse configuration for the FanControl.VCUsbTemp plugin at {config_json_path}. Please edit it to be valid and restart FanControl.";
                _logger.Log(LOG_PREFIX + error_message);
                _logger.Log(LOG_PREFIX + "Not loading any FanControl.VCUsbTemp sensors.");
                _logger.Log(LOG_PREFIX + $"Parsing exception hint: {e.Message}" + Environment.NewLine + e.StackTrace);
                _dialog.ShowMessageDialog(error_message);
            }
        }

        public void Close()
        {
            foreach (VCUsbTempSerialPort port in _serialports.Values)
            {
                port.Close();
            }
        }
        
        public void Load(IPluginSensorsContainer container)
        {
            foreach (VCUsbTempSerialPort port in _serialports.Values)
            {
                port.Open();
            }
            foreach (VCUsbTempPluginSensor sensor_port in _sensors)
            {
                container.TempSensors.Add(sensor_port);
            }
        }
    }
}
