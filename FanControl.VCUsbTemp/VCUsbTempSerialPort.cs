using System;
using System.IO.Ports;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FanControl.VCUsbTemp
{

    internal class VCUsbTempSerialPort
    {
        private SerialPort serial_port;
        private JObject sensordata;

        public VCUsbTempSerialPort(string device_id)
        {
            serial_port = new SerialPort(device_id);

            serial_port.BaudRate = 9600;
            serial_port.Parity = Parity.None;
            serial_port.StopBits = StopBits.One;
            serial_port.DataBits = 6;
            serial_port.Handshake = Handshake.None;
            serial_port.RtsEnable = false;

            serial_port.ReadTimeout = 500;
            serial_port.WriteTimeout = 500;
        }

        public void Open()
        {
            
            serial_port.Open();
        }

        public void Close()
        {
            serial_port.Close();
        }

        public float get(int channel)
        {
            try
            {
                string data = serial_port.ReadLine();
                sensordata = JObject.Parse(data);
                return (float)sensordata["temp"][channel];
            }
            catch (JsonException ex)
            {
                return -1;
            }
            catch (System.TimeoutException)
            {
                return -2;
            }

        }
    }
}
