using System;
using System.IO.Ports;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FanControl.VCUsbTemp
{

    internal class VCUsbTempSensor : ITempSensorDriver
    {
        private VCUsbTempSerialPort serial;
        private UInt16 sensor;

        public VCUsbTempSensor(VCUsbTempSerialPort _serial, UInt16 _sensor)
        {
            serial = _serial;
            sensor = _sensor;
        }

        public float Temperature()
        {
            return serial.get(sensor);
        }
    }
}
