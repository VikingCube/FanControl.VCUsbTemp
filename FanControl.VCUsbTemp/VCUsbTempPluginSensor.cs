using FanControl.Plugins;
using System;

namespace FanControl.VCUsbTemp
{
    internal class VCUsbTempPluginSensor : IPluginSensor
    {
        private readonly VCUsbTempSensorConfig _config;
        private ITempSensorDriver _thermometer;
        private float _temp_measurement;

        public string Id => _config.device_type + _config.device_id;

        public string Name => _config.device_id + "/" + _config.sensor;

        public float? Value => _temp_measurement;

        public VCUsbTempPluginSensor(VCUsbTempSensorConfig config, VCUsbTempSerialPort serialport)
        {
            _config = config;

            switch(_config.device_type)
            {
                case "VCUsbTempSensor":
                    _thermometer = new VCUsbTempSensor(serialport, _config.sensor);
                    break;
                default:
                    throw new Exception("Invalid device type");
            }
        }

        public void Update()
        {
            _temp_measurement = _thermometer.Temperature();
        }
    }
}
