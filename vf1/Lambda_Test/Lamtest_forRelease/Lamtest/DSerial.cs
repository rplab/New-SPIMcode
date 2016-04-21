//Serial port methods
//http://msdn.microsoft.com/en-us/library/system.io.ports.serialport_members.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Lamtest
{
    public class DSerial
    {
        // port with some basic settings
       SerialPort port = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
       public void openCom(string myPort)
        {
            if (port.IsOpen) port.Close();//The port must be cloased to assign a name to the port. 
            port.PortName = myPort;
            port.Open();
            clearBuffer();//discard CR
            port.ReadTimeout = 2000;
            clearBuffer();//discard CR
        }
       public void closeCom()
       {
           if (port.IsOpen) port.Close(); 
       }
       public bool isOpen()
       {
           bool status = true;
           if (port.IsOpen == false) status = false;
           return status;
       }
        public void writeByte(byte myByte)
        {
            port.Write(new byte[] {myByte}, 0, 1);
        }
        public byte readByte()
        {
            byte input = (byte)port.ReadByte();
            return input;
        }
        public string readString()
        {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
            string inputStr="";//can not properly read the <CR> from the lambda!
            inputStr = port.ReadTo(Convert.ToString(Convert.ToChar(13)));
            if(inputStr.Length < 2)
            {
                inputStr = port.ReadTo(Convert.ToString(Convert.ToChar(13)));
            }
            return inputStr;
        }
        public void clearBuffer()
        {
            port.DiscardInBuffer();
        }
    }
}
