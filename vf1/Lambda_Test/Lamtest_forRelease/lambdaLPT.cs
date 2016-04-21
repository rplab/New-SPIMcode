//Serial port methods
//http://msdn.microsoft.com/en-us/library/system.io.ports.serialport_members.aspx
//Reverse; http://weblogs.sqlteam.com/mladenp/archive/2006/03/19/9350.aspx 
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
//http://sandeep-aparajit.blogspot.com/2008/08/io-how-to-program-readwrite-parallel.html
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Lamtest
{
    public class lambdaSerial
    {
        // port with some basic settings
        SerialPort port = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
        i
        ~lambdaSerial() { }//generic destructor
        //Import byteCommands
        byteCommands byteCom = new byteCommands();
        private string mode="COM2";
        private int LPTaddress = 0378;
        public void setMode(string newMode)
        {
            mode = newMode;
        }
        public void setAddress(int newAddress)
        {
            LPTaddress = newAddress;
        }

       public void openCom(string myPort)
        {
           setMode(myPort);
           if (port.IsOpen) port.Close();//The port must be cloased to assign a name to the port. 
           switch (myPort)
           {
               case "LPT1":
                   setAddress(0378);
                   setMode("LPT");
                   lambdaParallel.Output(LPTaddress,238);
               break;
               default:
                    port.PortName = myPort;
                    port.Open();
                    port.ReadTimeout = 1000;
                    setMode("COM");
                break;
            }
        }
       public void closeCom()
       {
           if (port.IsOpen) port.Close(); 
       }
       public bool isOpen13()
       {
           bool status = false;
           if (port.IsOpen == false) status = false;
           byte loop = readByte();
           int i = 0;
           while (loop != byteCom.byteCR && port.IsOpen)
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
           if (port.IsOpen == false) status = false;
           return status;
       }
       public void clearBuffer()
       {
           switch (mode)
           {
               case "COM":
               port.DiscardInBuffer();
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
                   port.Write(new byte[] { myByte }, 0, 1);
                   break;
               case "LPT":
                   lambdaParallel.Output(LPTaddress, myByte);
                   break;
           }
       }
       public byte readByte()
       {
           byte input = 0;
           switch (mode)
           {
               case "COM":
                   if (port.IsOpen && port.BytesToRead > 0)
                   {
                       input = (byte)port.ReadByte();
                   }
                   break;
               case "LPT":
                   input=lambdaParallel.Input(LPTaddress);
                   break;
           }
           return input;
       }
       private string readString()
       {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
           string inputStr = "";//can not properly read the <CR> from the lambda!
           try
           {
               inputStr = port.ReadTo(Convert.ToString(Convert.ToChar(byteCom.byteCR)));
           }
           catch
           {
               inputStr = "LB10-2";
           }
           if (inputStr.Length < 4)
           {
               try
               {
                   inputStr = port.ReadTo(Convert.ToString(Convert.ToChar(byteCom.byteCR)));
               }
               catch
               {
                   inputStr = "LB10-2";
               }
           }
           return inputStr;
       }
    // Lambda specific methods_____________________________________________________
       //Get Config mthods_________________________________________________________ 
       public string getConfig()
       {//returns staus string
           clearBuffer();
           writeByte(byteCom.byteGetConfig);
           string status = readString();
           return status;
       }
       public string getController()
       {
           string controller;
           string status = getConfig();
           controller = status.Substring(1, 4);
           if (controller != "10-3" && controller != "10-B") 
           {
               controller = controller.Substring(0, 2);
               if (controller != "SC")
               {
                   controller = status.Substring(0, 6);
               }
           }
           return controller;
       }
       public string getController(string controller)
       {
           controller = controller.Substring(1, 4);
           if (controller != "10-3" && controller != "10-B") 
           {
               controller = controller.Substring(0, 2);
               if (controller != "SC")
               {
                   controller = controller.Substring(0, 6);
               }
           }
           return controller;
       }
       public string getWheelA()
       {
           string Wheel, controller;
           string status = getConfig();
           controller = getController(status);
           switch (controller)
           {
               case "10-3":
                   Wheel = status.Substring(5, 5);
               break;
               case "10-B":
                    Wheel = status.Substring(5, 4);
               break;
               default:
                    Wheel = "N.A.";
               break;
           }
           return Wheel;
       }
       public string getWheelB()
       {
           string Wheel, controller;
           string status = getConfig();
           controller = getController(status);
           switch (controller)
           {
               case "10-3":
                   Wheel = status.Substring(10, 5);
                   break;
               default:
                   Wheel = "N.A.";
                   break;
           }
           return Wheel;
       }
       public string getWheelC()
       {
           string Wheel, controller;
           string status = getConfig();
           controller = getController(status);
           switch (controller)
           {
               case "10-3":
                   Wheel = status.Substring(15, 5);
                   break;
               default:
                   Wheel = "N.A.";
                   break;
           }
           return Wheel;
       }
       public string getShutterA()
       {
           string Wheel, controller;
           string status = getConfig();
           controller = getController(status);
           switch (controller)
           {
               case "10-3":
                   Wheel = status.Substring(20, 5);
                   break;
               case "10-B":
                   Wheel = status.Substring(9, 4);
                   break;
               case "SC":
                   Wheel = status.Substring(9, 4);
                   break;
               default:
                   Wheel = "N.A.";
                   break;
           }
           return Wheel;
       }
       public string getShutterB()
       {//need to deal with ND on BOTH A and B
           string Wheel, controller;
           string status = getConfig();
           controller = getController(status);
           switch (controller)
           {
               case "10-3":
                   Wheel = status.Substring(25, 5);
                   break;
               case "10-B":
                   if (status.Substring(5, 4) == "W-NC" && status.Length > 12)
                   {
                       Wheel = status.Substring(12, 4);
                   }
                   else
                   {
                       Wheel = "N.A.";
                   }
                   break;
               default:
                   Wheel = "N.A.";
                   break;
           }
           return Wheel;
       }

       public string getShutterC()
       {//need to deal with ND on BOTH A and B
           string Wheel, controller;
           string status = getConfig();
           controller = getController(status);
           if (status.Length > 30)
           {
               Wheel = status.Substring(30, 5);
           }
           else
           {
               Wheel = "N.A.";
           }
           return Wheel;
       }
//Move commands_______________________________________________________________________

       public void moveWheelA(byte myByte)
       {
            port.Write(new byte[] { myByte }, 0, 1);
       }

       public void moveWheelB(byte myByte)
       {
           myByte = (byte)(myByte + 128);
           port.Write(new byte[] { myByte }, 0, 1);
       }
       public void writeShutterB(byte myByte)
       {
           if (myByte <= byteCom.byteCloseB&& myByte >= byteCom.byteOpenB)
           {
               port.Write(new byte[] { myByte }, 0, 1);
           }
           else
           {
               port.Write(new byte[] { byteCom.byteOpenBCond }, 0, 1);//Open conditional shutter B
           }
       }
       public void moveWheelC(byte myByte)
       {
           port.Write(new byte[] {byteCom.byteSelectC }, 0, 1);
           port.Write(new byte[] { myByte }, 0, 1);
       }
       public void moveBatch(byte A,byte B,byte C)
       {
           writeByte(byteCom.byteLB103Batch);//start batch
           if (A != 255) { moveWheelA(A); }
           if (B != 255) { moveWheelB(B); }
           if (C != 255) { moveWheelC(C); }
           writeByte(190);//end batch
       }
       public void moveBatch2(byte A, byte AS, byte B, byte BS)
       {
           writeByte(byteCom.byteLB102Batch);//start batch
           moveWheelA(A);
           writeShutterA(AS);
           moveWheelB(B);
           writeShutterB(BS);
       }
//Shutter specific methods here___________________________________________________

       public void writeShutterA(byte myByte)
       {
           if (myByte <= byteCom.byteCloseA && myByte >= byteCom.byteOpenA)
           {
               port.Write(new byte[] { myByte }, 0, 1);
           }
           else
           {
               port.Write(new byte[] {byteCom.byteOpenACond}, 0, 1);//Open conditional shutter A
           }
       }

       public void openAdefault()
       {//Use the current default!
           port.Write(new byte[] { byteCom.byteOpenA }, 0, 1);
       }
       public void openA()
       {//Default is fast mode!
           port.Write(new byte[] { byteCom.byteSetFast }, 0, 1);
           port.Write(new byte[] { 1 }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenA }, 0, 1);
       }
       public void openA(byte ND)
       {//Only ND mode requires a byte setting
           port.Write(new byte[] { byteCom.byteSetND }, 0, 1);
           port.Write(new byte[] { 1 }, 0, 1);
           port.Write(new byte[] { ND }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenA }, 0, 1);
       }
       public void openASoft()
       {//soft mode
           port.Write(new byte[] { byteCom.byteSetSoft }, 0, 1);
           port.Write(new byte[] { 1 }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenA }, 0, 1);
       }
       public void openACond()
       {//Shutter only opens when the wheel is stpped
           port.Write(new byte[] { byteCom.byteOpenA }, 0, 1);
       }
       public void openANoMode()
       {//Shutter only opens when the wheel is stpped
           port.Write(new byte[] { byteCom.byteOpenA }, 0, 1);
       }
       public void closeShutterA()
       {
           port.Write(new byte[] { byteCom.byteCloseA}, 0, 1);
       }
       public void openBdefault()
       {//Use the current default!
           port.Write(new byte[] { byteCom.byteOpenB }, 0, 1);
       }
       public void openB()
       {//Default is fast mode!
           port.Write(new byte[] { byteCom.byteSetFast }, 0, 1);
           port.Write(new byte[] { 2 }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenB }, 0, 1);
       }
       public void openB(byte ND)
       {//Only ND mode requires a byte setting
           port.Write(new byte[] { byteCom.byteSetND }, 0, 1);
           port.Write(new byte[] { 2 }, 0, 1);
           port.Write(new byte[] { ND }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenB }, 0, 1);
       }
       public void openBSoft()
       {//soft mode
           port.Write(new byte[] { byteCom.byteSetSoft }, 0, 1);
           port.Write(new byte[] { 2 }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenB }, 0, 1);
       }
       public void openBCond()
       {//Shutter only opens when the wheel is stpped
           port.Write(new byte[] { byteCom.byteOpenBCond }, 0, 1);
       }
       public void openBNoMode()
       {//Shutter only opens when the wheel is stpped
           port.Write(new byte[] { byteCom.byteOpenB }, 0, 1);
       }
       public void closeShutterB()
       {
           port.Write(new byte[] { byteCom.byteCloseB }, 0, 1);
       }
       public void openCdefault()
       {//Use the current default!
           port.Write(new byte[] { byteCom.byteOpenC }, 0, 1);
       }
       public void openC()
       {//Default is fast mode!//Use the current default!
           port.Write(new byte[] { byteCom.byteSetFast }, 0, 1);
           port.Write(new byte[] { 3 }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenC }, 0, 1);
       }
       public void openC(byte ND)
       {//Only ND mode requires a byte setting
           port.Write(new byte[] { byteCom.byteSetND }, 0, 1);
           port.Write(new byte[] { 3 }, 0, 1);
           port.Write(new byte[] { ND }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenC }, 0, 1);
       }
       public void openCSoft()
       {//soft mode
           port.Write(new byte[] { byteCom.byteSetSoft }, 0, 1);
           port.Write(new byte[] { 3 }, 0, 1);
           port.Write(new byte[] { byteCom.byteOpenC }, 0, 1);
       }
       public void openCCond()
       {//Shutter only opens when the wheel is stpped
           port.Write(new byte[] { byteCom.byteOpenCCond }, 0, 1);
       }
       public void openCNoMode()
       {//Shutter only opens when the wheel is stpped
           port.Write(new byte[] { byteCom.byteOpenC }, 0, 1);
       }
       public void closeShutterC()
       {
           port.Write(new byte[] { byteCom.byteCloseC }, 0, 1);
       }
//TTL methods________________________________________________________________________
       public void setTTLDisabled()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteDisableTTL);
           return;
       }
       public void setTTLHigh()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteSetTTLHigh);
           return;
       }
       public void setTTLLow()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteSetTTLLow);
           return;
       }
       public void setTTLToggleRisingEdge()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteSetTTLToggleRising);
           return;
       }
       public void setTTLToggleFallingEdge()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteSetTTLToggleFalling);
           return;
       }
       public void setSyncDisabled()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteDisableSync);
           return;
       }
       public void setSyncHighOpen()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteSyncHighOpen);
           return;
       }
       public void setSyncLowOpen()
       {
           writeByte(byteCom.byteLBSC_Prefix);
           writeByte(byteCom.byteSyncLowOpen);
           return;
       }
//Timer methods_____________________________________________________________________________________

        public void setDelayTimer(uint min, uint sec, uint ms, uint us)
        // The code for setting the exposure and dealy is almost identical!
        // The sole diference is in the second nibble of byte 4.
        {
            int msOnes, msTens, msHundreds;
            string msString, onesMs, tensMs, hundredsMs;
            byte byte1, byte2, byte3, byte4;
            if (us > 9) { us = 0; }
            if (ms > 999) { ms = 0; }
            if (sec > 59) { sec = 0; }
            if (min > 59) { min = 0; }
            //The time on the LB-SC has a unique encoding scheam
            //byte one
            msString = ms.ToString();
            onesMs = msString.Substring(0, 1);
            msOnes = int.Parse(onesMs);
            byte1 = (byte)((msOnes >> 4) + us);
            //byte two
            try { tensMs = msString.Substring(1, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                tensMs = "0";
            }
            msTens = int.Parse(tensMs);
            try { hundredsMs = msString.Substring(2, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                hundredsMs = "0";
            }
            msHundreds = int.Parse(hundredsMs);
            byte2 = (byte)((msHundreds >> 4) + msTens);
            byte3 = (byte)(sec);
            byte4 = (byte)(min);
            //Set timeer
            writeByte(byteCom.byteLBSC_Prefix);
            writeByte(16);//set delay
            writeByte(byte4);
            writeByte(byte3);
            writeByte(byte2);
            writeByte(byte1);
            return;
        }
        public void setExposureTimer(uint min, uint sec, uint ms, uint us)
        // The code for setting the exposure and dealy is almost identical!
        // The sole diference is in the second nibble of byte 4.
        {
            int msOnes, msTens, msHundreds;
            string msString, onesMs, tensMs, hundredsMs;
            byte byte1, byte2, byte3, byte4;
            if (us > 9) { us = 0; }
            if (ms > 999) { ms = 0; }
            if (sec > 59) { sec = 0; }
            if (min > 59) { min = 0; }
            //The time on the LB-SC has a unique encoding scheam
            //byte one
            msString = ms.ToString();
            onesMs = msString.Substring(0,1);
            msOnes = int.Parse(onesMs);
            byte1 = (byte)((msOnes >> 4) + us);
            //byte two
            try { tensMs = msString.Substring(1, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                tensMs = "0";
            }
            msTens = int.Parse(tensMs);
            try { hundredsMs = msString.Substring(2, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                hundredsMs = "0";
            }
            msHundreds = int.Parse(hundredsMs);
            byte2 = (byte)((msHundreds >> 4) + msTens);
            byte3 = (byte)(sec);
            byte4 = (byte)(min);
            //Set timeer
            writeByte(byteCom.byteLBSC_Prefix);
            writeByte(32);//set exposure
            writeByte(byte4);
            writeByte(byte3);
            writeByte(byte2);
            writeByte(byte1);
            return;
        }

        public void setFreeRunTTL(int cycles)
        {
            byte byte1, byte2;
            byte2 = (byte)(cycles >> 8); // second byte
            byte1 = (byte)(cycles);
            writeByte(byteCom.byteLBSC_Prefix);
            writeByte(byteCom.byteSetCycles);
            writeByte(0);//writeByte(byte3);
            writeByte(1);//writeByte(byte1);
            writeByte(byteCom.byteLBSC_Prefix);
            writeByte(byteCom.byteSetFreeRunTTLIn);//242 ttl
        }
        public void setSingleShot()
        // The code for setting the exposure and dealy is almost identical!
        // The sole diference is in the second nibble of byte 4.
        {
            setTTLToggleFallingEdge();
            setFreeRunTTL(1);
        }
 //Restore defaults!__________________________________________________________________
      
        public void restoreDefaults()
        {
            stopFreeRun();
            writeByte(byteCom.byteLBSC_Prefix);
            writeByte(byteCom.byteResetDefault);
            return;
        }
        public void restoreLast()
        {
            stopFreeRun();
            writeByte(byteCom.byteRestorLast);
            return;
        }
        public void stopFreeRun()
        {
            writeByte(byteCom.byteStopFreeRun);
            return;
        }
    }
}
