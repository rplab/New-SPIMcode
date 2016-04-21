//Serial port methods
//http://msdn.microsoft.com/en-us/library/system.io.ports.serialport_members.aspx
//Reverse; http://weblogs.sqlteam.com/mladenp/archive/2006/03/19/9350.aspx 
//http://www.codeproject.com/Articles/15020/Reading-from-Parallel-Port-using-Inpout32-dll
/*
**	FILENAME			lambdaSerial.cs
**
**	PURPOSE				This class is designed to handle communications from
 *                      all lambda Imaging products.  In addtion there are many 
 *                      methods specific to this product line.  Threading has 
 *                      been employed for both the addtion of a timer function
 *                      as well as to allow safe termination of this program.
**
**	CREATION DATE		10-01-2010
**	LAST MODIFICATION	10-04-2010
**
**	AUTHOR				Dan Carte
**
*/
//To Do: Implement and test parallel port.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;//FOR MESSAGE BOX

namespace Lamtest
{
    public class lambdaSerial
    {
        // port with some basic settings
        SerialPort lambdaRS232;
        lambdaUtils Util;
        FTDI_USB lambdaUSB;
        byteCommands byteCom;
        lambdaParallel myLPT;
        private string mode;
        private short LPTadress;
        private uint baudRate;//Makes no difference .SetBaudRate not working???
        public TextBox comOutput;

        public lambdaSerial(TextBox comString)
        {
            lambdaRS232 = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
            Util = new lambdaUtils();
            try { lambdaUSB = new FTDI_USB(); }
            catch { lambdaUSB = null; }
            byteCom = new byteCommands();
            myLPT = new lambdaParallel();
            mode = "USB";
            LPTadress = 888;
            baudRate = 128000;
            comOutput = comString;
        }
        public lambdaSerial()
        {
            lambdaRS232 = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
            Util = new lambdaUtils();
            try { lambdaUSB = new FTDI_USB(); }
            catch { lambdaUSB = null; }
            byteCom = new byteCommands();
            myLPT = new lambdaParallel();
            mode = "USB";
            LPTadress = 888;
            baudRate = 128000;
        }
        ~lambdaSerial()
        {
            lambdaRS232.Close();
            try { lambdaUSB.ClearBuffer(); }
            catch { lambdaUSB = null; }
        }

        public void setMode(string newMode)
        {
            mode = newMode;
            //MessageBox.Show("Set mode" + mode);
        }
        public void setAdress(int newAdress)
        {
            LPTadress = (short) newAdress;
        }
        //USB Methods
        public UInt32 GetDeviceCount()
        {
            UInt32 count = lambdaUSB.GetDeviceCount();
            return count;
        }
        public string GetDescription()
        {//Get the Description of the first device// Not tested
            string myDescription= lambdaUSB.GetDescription();
            return myDescription;
        }
        public string GetDescription(UInt32 i)
        {//Get the Description of the specified device// Not tested
            string myDescription = lambdaUSB.GetDescription(i);
            return myDescription;
        }
        public string GetSerialNum()
        {//Get the SerialNum of the first device// Not tested
            string mySerialNum= lambdaUSB.GetSerialNum();
            return mySerialNum;
        }
        public string GetSerialNum(UInt32 i)
        {//Get the SerialNum of the specified device// Not tested
            string mySerialNum = lambdaUSB.GetSerialNum(i);
            return mySerialNum;
        }
        public void OpenByDescription(string d)
        {//Get the Description of the first device// Not tested
            lambdaUSB.OpenByDescription(d);
            setMode("USB");
            lambdaUSB.WriteByte(238);
            return;
        }
        public void OpenBySerialNum(string s)
        {//Get the SerialNum of the specified device// Not tested
            lambdaUSB.OpenBySerialNum(s);
            lambdaUSB.SetBaudrate(baudRate);
            setMode("USB");
            //MessageBox.Show("Case USB");
            if (lambdaUSB.IsOpen()==false){MessageBox.Show("USB Failed to open"); }
            return;
        }
        public void openByIndex(string myPort, UInt32 index)
        {
            setMode(myPort);
            //if (lambdaRS232.IsOpen) lambdaRS232.Close();//The port must be cloased to assign a name to the port. 
            //lambdaParallel.Output(LPTadress, 0);
            lambdaUSB.OpenDeviceByIndex(index);//Need to use a list later
            lambdaUSB.SetBaudrate(baudRate);
            setMode("USB");
            lambdaUSB.WriteByte(238);
        }
        public void openByIndex(UInt32 index)
        {
            //if (lambdaRS232.IsOpen) lambdaRS232.Close();//The port must be cloased to assign a name to the port. 
            //lambdaParallel.Output(LPTadress, 0);
            lambdaUSB.OpenDeviceByIndex(index);//Need to use a list later
            lambdaUSB.SetBaudrate(baudRate);
            setMode("USB");
            lambdaUSB.WriteByte(238);
        }
        public void SetUSBBaudrate(uint r)
        {
            lambdaUSB.SetBaudrate(r);
        }
        public void SetRS232Baudrate(int r)
        {
            lambdaRS232.BaudRate = r;
        }
        public void SetTimeouts(uint a, uint b)
        {
            lambdaUSB.SetTimeouts(a,b);
        }
        //End USB methods
       public void openCom(string myPort)
        {
           setMode(myPort);
           //MessageBox.Show("Case ???" + myPort); 
           //if (lambdaRS232.IsOpen) lambdaRS232.Close();//The port must be cloased to assign a name to the port. 
           //lambdaParallel.Output(LPTadress, 0);
           switch (myPort)
           {
            case "LPT1":
               setAdress(888);
               setMode("LPT");
               writeByte(238);
               break;
            case "LPT2":
               setAdress(632);
               setMode("LPT");
               writeByte(238);
               break;
            case "COM1":
               lambdaRS232.Close();
               lambdaRS232.PortName = myPort;
               lambdaRS232.Open();
               lambdaRS232.ReadTimeout = 100;
               setMode("COM");
               break;
            case "COM2":
               lambdaRS232.Close();
               lambdaRS232.PortName = myPort;
               lambdaRS232.Open();
               lambdaRS232.ReadTimeout = 100;
               setMode("COM");
               break;
            case "COM3":
               lambdaRS232.Close();
               lambdaRS232.PortName = myPort;
               lambdaRS232.Open();
               lambdaRS232.ReadTimeout = 100;
               setMode("COM");
               break;
            case "COM4":
               lambdaRS232.Close();
               lambdaRS232.PortName = myPort;
               lambdaRS232.Open();
               lambdaRS232.ReadTimeout = 100;
               setMode("COM");
               break;
           case "USB":
               setMode("USB");
               lambdaUSB.OpenBySerialNum(lambdaUSB.GetSerialNum());//Need to use a list later
               lambdaUSB.SetBaudrate(baudRate);
               lambdaUSB.WriteByte(238);
               //MessageBox.Show("Case USB"); 
               break;
           default:
               lambdaRS232.Close();
               lambdaRS232.PortName = myPort;
               lambdaRS232.Open();
               lambdaRS232.ReadTimeout = 100;
               setMode("COM");
                break;
            }
        }
       public void closeCom()
       {
           if (lambdaRS232.IsOpen) lambdaRS232.Close(); 
       }
       public bool isOpen13()
       {
           bool status = false;
           if (lambdaRS232.IsOpen == false) status = false;
           byte loop = readByte();
           int i = 0;
           while (loop != byteCom.byteCR)
           {
               loop = readByte();
               Thread.Sleep(1);
               i++;
              if (i >= 5) { break; }
           }
           if (loop == byteCom.byteCR) { status = true; } else { status = false; }
           return status;
       }
       public bool isOpen()
       {
           bool status = true;
           switch (mode)
           {
               case "COM":
                   if (lambdaRS232.IsOpen == false && mode == "COM") status = false;
                   break;
               case "USB":
                   if (lambdaUSB.IsOpen() == false && mode == "USB") status = false;
                   break;
           }
           return status;
       }
       public void clearBuffer()//Clears transmit and recieve buffers.
       {
           switch (mode)
           {
               case "COM":
               lambdaRS232.DiscardInBuffer();
               lambdaRS232.DiscardOutBuffer();
           break;
               case "USB":
               lambdaUSB.ClearBuffer();
           break;
           }

       }
       public string getPorts()
       {
           string myPorts = "";
           // Get a list of serial port names.
           string[] ports = SerialPort.GetPortNames();

           // Display each port name to the console.
           foreach (string port in ports)
           {
               myPorts = myPorts + port + "\r\n";
           }
           return myPorts;
       }

// Read && write methods here_______________________________________________________
       public void writeByte(byte myByte)
       {
           switch (mode)
           {
               case "COM":
                   lambdaRS232.Write(new byte[] { myByte }, 0, 1);
                   //MessageBox.Show("Write");
                   break;
               case "LPT":
                   lambdaParallel.Output(LPTadress, myByte);
                   break;
               case "USB":
                    lambdaUSB.WriteByte(myByte);
               break;
               default:
               lambdaRS232.Write(new byte[] { myByte }, 0, 1);
               break;
           }
       }
       public byte readByte()
       {
           byte input = 0;
           switch (mode)
           {
               case "COM":
                   if (lambdaRS232.IsOpen && lambdaRS232.BytesToRead > 0)
                   {
                       input = (byte)lambdaRS232.ReadByte();
                   }
                   break;
               case "LPT":
                   input=13;
                   break;
               case "USB":
                   input = lambdaUSB.ReadByte();
                   break;
               default :
                   input = (byte)123;
                   break;
           }
           return input;
       }
       public string readConfigString()
       {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
           string inputStr = "";//can not properly read the <CR> from the lambda!
           try
           {
               switch (mode)
               {
                   case "COM":
                       inputStr = lambdaRS232.ReadTo(Convert.ToString(Convert.ToChar(byteCom.byteCR)));
                       break;
                   case "LPT":
                       inputStr = lambdaRS232.ReadTo(Convert.ToString(Convert.ToChar(lambdaParallel.Input(LPTadress)))); 
                       break;
                   case "USB":
                       inputStr = lambdaUSB.Readto(32);
                       lambdaUSB.ClearBuffer();
                       break;
               }
           }
         catch
           {
               inputStr = "LB10-2";
           }
           if (inputStr.Length < 4 && mode=="COM")
           {
               try
               {
                   inputStr = lambdaRS232.ReadTo(Convert.ToString(Convert.ToChar(byteCom.byteCR)));
               }
               catch
               {
                   inputStr = "LB10-2";
               }
           }
           return inputStr;
       }
       public string readString()
       {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
           string inputStr = "";//can not properly read the <CR> from the lambda!
           try
           {
               switch (mode)
               {
                   case "COM":
                       inputStr = lambdaRS232.ReadTo(Convert.ToString(Convert.ToChar(byteCom.byteCR)));
                       break;
                   case "LPT":
                       inputStr = lambdaRS232.ReadTo(Convert.ToString(Convert.ToChar(lambdaParallel.Input(LPTadress))));
                       break;
                   case "USB":
                       inputStr = lambdaUSB.Readto(30);
                       lambdaUSB.ClearBuffer();
                       break;
               }
           }
           catch
           {
               inputStr = "Blank";
           }
           return inputStr;
       }
       public string getConfig()
       {//returns status string
           clearBuffer();
           writeByte(byteCom.byteGetConfig);
           string status = readConfigString();
           if (status.Length < 2 || status == null)
           {
               status = "NA";
           }
           return status;
       }
       public string getController()
       {
           string status = getConfig();
           string controller = Util. 
               getController(status);
           return controller;
       }
    }
}
