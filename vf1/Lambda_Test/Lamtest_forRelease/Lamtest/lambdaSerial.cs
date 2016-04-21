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
    public class lambdaSerial : lambdaUtils
    {
        // port with some basic settings
        SerialPort lambdaRS232;
        //lambdaUtils Util;
        FTDI_USB lambdaUSB;
        byteCommands byteCom;
        lambdaParallel myLPT;
        public vf5Functions vf5;//For creating object
        public lbXFunctions lbX;//For creating object
        private Mainform mf;//create mainform object
        public object stopProcess;//To access top level
        public CheckBox stop;//To access top level
        public TextBox txtBoxDialog;
        private string mode;
        private short LPTadress;
        private uint baudRate;//Makes no difference .SetBaudRate not working???
        public TextBox txtComOutput;

        public lambdaSerial(Mainform mf)//mainform object passed to lambddaSerial object
        {
            lambdaRS232 = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            //Util = new lambdaUtils();
            try { lambdaUSB = new FTDI_USB(); }
            catch { lambdaUSB = null; }
            byteCom = new byteCommands();
            myLPT = new lambdaParallel();
            this.mf = mf;//set reference of comport child to serial parrent
            vf5 = new vf5Functions(this);//pass the com port object to VF5 object to use parent methods
            lbX = new lbXFunctions(this);//pass the com port object to lbX object to use parent methods
            txtBoxDialog = mf.txtBoxDialog;
            txtComOutput = mf.txtCom;
            stop = mf.checkBoxStop;
            //App = mf.Application;
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
           txtComOutput.AppendText("Write:" + Environment.NewLine + "Sts.| Dec. | Hex. " + Environment.NewLine);
           string hexOutput = String.Format("{0:X}", myByte);
           string decOutput = String.Format("{0:g}", myByte);
           txtComOutput.AppendText(myByte.ToString() + "  " + decOutput + "    " + hexOutput + Environment.NewLine);
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
           };
           txtComOutput.AppendText("Write:" + Environment.NewLine + "Sts.| Dec. | Hex. " + Environment.NewLine);
           string hexOutput = String.Format("{0:X}", input);
           string decOutput = String.Format("{0:g}", input);
           txtComOutput.AppendText(input.ToString() + "  " + decOutput + "    " + hexOutput + Environment.NewLine);
           return input;
       }
       /*private void readPort()
       {
           string myMode = txtComPort.Text.ToString();
           byte loop;
           //txtCom.AppendText("R-");
           do
           {
               loop = lambdaCom.readByte();
               if (loop != 0 && lambdaCom.isOpen()) {}
               Application.DoEvents();
           } while (loop != 0);
           //txtCom.AppendText(Environment.NewLine);
       }*///Why??
       public string readStatus()
       {//returns status string
           clearBuffer();
           writeByte(byteCom.byteGetStatus);
           string status = readString();
           if (status.Length < 1 || status == null)
           {
               status = ("NA " + status.Length.ToString());
           }
           return status;
       }
       public string getStatus()
       {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
           string inputStr = readStatus();//can not properly read the <CR> from the lambda!
           string configStr = getConfig();
           string controller = "LB10-2";
           string outputStr = "";//can not properly read the <CR> from the lambda!
           string[] info = new string[10];
           char[] myChars;
           controller = getController(configStr);
           myChars = inputStr.ToCharArray(0, inputStr.Length);//The first character is the echo
           byte byteOne = (byte)myChars[1];
           byte byteTwo = (byte)myChars[2];
           byte byteThree = (byte)myChars[3];//4 = CR
           switch (controller)
           {
               case "10-3":
                   outputStr = ("LB10-3: ");
                   break;
               case "10-B":
                   info = getWheelInfo(byteOne);
                   outputStr = info[0] + "  " + info[1] + "  " + info[2] + Environment.NewLine;
                   info = getWheelInfo(byteTwo);
                   outputStr = outputStr + "Shutter: " + byteTwo + "State: " + byteThree;
                   break;
               case "SC":
                   outputStr = ("LB-SC: ");
                   break;
               default:
                   outputStr = ("Case failed: " + inputStr + "Cont:" + configStr);
                   break;
           }
           return outputStr;
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
               inputStr = "No config string returned";
           }
           if (inputStr.Length < 4 && mode=="COM")
           {
               try
               {
                   inputStr = lambdaRS232.ReadTo(Convert.ToString(Convert.ToChar(byteCom.byteCR)));
               }
               catch
               {
                   inputStr = "No config string returned";
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
           string controller = getController(status);
           return controller;
       }

    }
}
