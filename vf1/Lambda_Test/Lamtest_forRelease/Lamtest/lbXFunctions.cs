using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lamtest
{
    public class lbXFunctions
    {
      byteCommands byteCom;
       private lambdaSerial com;//create comport object
       //private Mainform mf;
       public lbXFunctions(lambdaSerial com)//com object passed to VF5fuctions object
       {
           byteCom = new byteCommands();
           this.com = com;//set reference of comport child to serial parrent
       }
       public void moveMyWheel(string wheel, byte speedByte, byte filterByte)
       {
           byte moveByte = (byte)com.getMoveByte(speedByte, filterByte);
           moveMyWheel(wheel, moveByte);
           return;
       }
       public void moveMyWheel(string wheel, byte moveByte)
       {
           switch (wheel)
           {
               case "Wheel A":
                   moveWheelA(moveByte);
                   break;
               case "Wheel B":
                   moveWheelB(moveByte);
                   break;
               case "Wheel C":
                   moveWheelC(moveByte);
                   break;
               case "LB10-3 Batch":
                   moveBatch(moveByte, moveByte, moveByte);
                   break;
               case "LB10-2 Batch":
                   moveBatch2(moveByte, byteCom.byteCloseA, moveByte, byteCom.byteCloseB);//Close shutter by default
                   break;
           }
       }
           public void moveWheelA(byte myByte)
        {
            com.writeByte(myByte);
        }
        public void moveWheelB(byte myByte)
        {
            myByte = (byte)(myByte + 128);
            com.writeByte(myByte);
        }
        /*public void writeShutterA(byte myByte)
        {
            if (myByte <= byteCom.byteCloseA && myByte >= byteCom.byteOpenA)
            {
                com.writeByte(myByte);
            }
            else
            {
                com.writeByte(byteCom.byteOpenACond);
            }
        }*/
        public void selectA(string controller)
        {//Default is fast mode!
            if (controller == "10-3" || (controller == "IQ" && controller == "10-B")) { com.writeByte(1); } //For LB10-3 only OR LB10-B 2 shutter
        }
        public void openAdefault(string controller)
        {
            selectA(controller);
            com.writeByte(byteCom.byteOpenA);
        }
        public void openAdefault()
        {
            com.writeByte(byteCom.byteOpenA);
        }
        public void openA(string controller)
        {//Default is fast mode! 
            com.writeByte(byteCom.byteSetFast);
            selectA(controller);
            com.writeByte(byteCom.byteOpenA);
        }
        public void openA()
        {//Default is fast mode! ;
            com.writeByte(byteCom.byteSetFast);
            com.writeByte(byteCom.byteOpenA);
        }
        public void openA(string controller, byte ND)
        {//Only ND mode requires a byte setting
            com.writeByte(byteCom.byteSetND);
            selectA(controller); 
            com.writeByte(ND);
            com.writeByte(byteCom.byteOpenA);
        }
        public void openA(byte ND)
        {//Only ND mode requires a byte setting
            com.writeByte(byteCom.byteSetND);
            com.writeByte(ND);
            com.writeByte(byteCom.byteOpenA);
        }
        public void openACond(string controller)
        {//Shutter only opens when the wheel is stopped
            selectA(controller);
            com.writeByte(byteCom.byteOpenACond);
        }
        public void openACond()
        {//Shutter only opens when the wheel is stopped
            com.writeByte(byteCom.byteOpenACond);
        }
        public void setAfast(string controller)
        {
            selectA(controller);
            com.writeByte(byteCom.byteSetFast);
        }
        public void setAfast()
        {
            com.writeByte(byteCom.byteSetFast);
        }
        public void openASoft(string controller)
        {//soft mode
            com.writeByte(byteCom.byteSetSoft);
            selectA(controller); 
            com.writeByte(byteCom.byteOpenA);
        }
        public void openASoft()
        {//soft mode
            com.writeByte(byteCom.byteSetSoft);
            com.writeByte(byteCom.byteOpenA);
        }
        public void closeShutterA()
        {
            com.writeByte(byteCom.byteCloseA);
        }
        public void writeShutterB(byte myByte)
        {
            if (myByte <= byteCom.byteCloseB && myByte >= byteCom.byteOpenB)
            {
                com.writeByte(myByte);
            }
            else
            {
                com.writeByte(byteCom.byteOpenBCond);
            }
        }
        public void openANoMode()
        {//Shutter only opens when the wheel is stpped
            com.writeByte(byteCom.byteOpenA);
        }
        public void openBdefault()
        {//Use the current default!
            com.writeByte(byteCom.byteOpenB);
        }
        public void openB()
        {//Default is fast mode!
            com.writeByte(byteCom.byteSetFast);
            com.writeByte(2);
            com.writeByte(byteCom.byteOpenB);
        }
        public void openB(byte ND)
        {//Only ND mode requires a byte setting
            com.writeByte(byteCom.byteSetND);
            com.writeByte(2);
            com.writeByte(ND);
            com.writeByte(byteCom.byteOpenB);
        }
        public void openBSoft()
        {//soft mode
            com.writeByte(byteCom.byteSetSoft);
            com.writeByte(2);
            com.writeByte(byteCom.byteOpenB);
        }
        public void openBCond()
        {//Shutter only opens when the wheel is stopped
            com.writeByte(byteCom.byteOpenBCond);
        }
        public void openBNoMode()
        {//Shutter only opens when the wheel is stopped
            com.writeByte(byteCom.byteOpenB);
        }
        public void closeShutterB()
        {
            com.writeByte(byteCom.byteCloseB);
        }
        public void openCdefault()
        {//Use the current default!
            com.writeByte(byteCom.byteOpenC);
        }
        public void openC()
        {//Default is fast mode!//Use the current default!
            com.writeByte(byteCom.byteSetFast);
            com.writeByte(3);
            com.writeByte(byteCom.byteOpenC);
        }
        public void openC(byte ND)
        {//Only ND mode requires a byte setting
            com.writeByte(byteCom.byteSetND);
            com.writeByte(3);
            com.writeByte(ND);
            com.writeByte(byteCom.byteOpenC);
        }
        public void openCSoft()
        {//soft mode
            com.writeByte(byteCom.byteSetSoft);
            com.writeByte(3);
            com.writeByte(byteCom.byteOpenC);
        }
        public void openCCond()
        {//Shutter only opens when the wheel is stopped
            com.writeByte(byteCom.byteOpenCCond);
        }
        public void openCNoMode()
        {//Shutter only opens when the wheel is stopped
            com.writeByte(byteCom.byteOpenC);
        }
        public void closeShutterC()
        {
            com.writeByte(byteCom.byteCloseC);
        }
        public void moveWheelC(byte myByte)
        {
            com.writeByte(byteCom.byteSelectC);
            com.writeByte(myByte);
        }
        public void moveBatch(byte A, byte B, byte C)
        {
            
            com.writeByte(byteCom.byteLB103Batch);//start batch
            if (A != 255) { moveWheelA(A); }
            //writeByte(byteCom.byteCloseA);//for 3 move / shutter test
            if (B != 255) { moveWheelB(B); }
            if (C != 255) { moveWheelC(C); }
            com.writeByte(190);//end batch
            //writeByte(byteCom.byteOpenA);//Open shutter in batch
            
        }
        public void moveBatch2(byte A, byte AS, byte B, byte BS)
        {
            com.writeByte(byteCom.byteLB102Batch);//start batch
            moveWheelA(A);
            //closeA();
            moveWheelB(B);
            //closeB();
        }
       }
    }
