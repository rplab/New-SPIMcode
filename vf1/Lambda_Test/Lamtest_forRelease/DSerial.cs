using System;
using System.IO.Ports;

namespace DSerial
{
    public class DSerial
    {
        public void openCom()
        {
            // port with some basic settings
            SerialPort port = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
            // Open the port for communications
            port.Open();
            // Write a set of bytes
            port.Write(new byte[] { 0xEE, 0xE2, 0xFF }, 0, 3);
            // Close the port
            port.Close();
        }
    }
}
