using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;//FOR MESSAGE BOX

namespace Lamtest
{
    class lambdaVF
    {
        lambdaSerial lambdaCom = new lambdaSerial();
        byteCommands byteCom = new byteCommands();

        public void setMode(string mode)
        {
            lambdaCom.setMode(mode);
            lambdaCom.openCom(mode);
            return;
        }
        public string GetVFAll()
        {
            lambdaCom.clearBuffer();
            lambdaCom.writeByte(252);
            lambdaCom.writeByte(byteCom.byteGetVFAll);
            string filterPoss = lambdaCom.readString();
            return filterPoss;
        }
        public void setWaveLegnth(byte speed, int freqVal)
        {
            //http://stackoverflow.com/questions/1318933/c-int-to-byte
            byte[] intBytes = BitConverter.GetBytes(freqVal);
            byte mult = 64;
            //Array.Reverse(intBytes);
            intBytes[1] = (byte)(intBytes[1] + (byte)(speed * mult));//Add the speed component to the high position byte
            if (lambdaCom.isOpen())
            {
                lambdaCom.writeByte(218 /*byteCom.byteSetWlength*/);
                lambdaCom.writeByte(intBytes[0]);
                lambdaCom.writeByte(intBytes[1]);
            }
            return;
        }
        public UInt16 getWaveLegnth()//Need to implement
        {
            byte[] readBytes = new byte[2];
            UInt16 w;
            lambdaCom.clearBuffer();
            lambdaCom.writeByte(219 /*byteCom.byteGetWlength*/);
            readBytes[0] = lambdaCom.readByte();//Clear DB return
            readBytes[0] = lambdaCom.readByte();
            Thread.Sleep(10);
            readBytes[1] = lambdaCom.readByte();//txtBoxDialog.AppendText("base Posstion " + b + Environment.NewLine);
            w = BitConverter.ToUInt16(readBytes, 0);//w = int (w1 >> 8) + int (w2);
            return w;
        }
        public void btnMoveStepAngle(byte filterVal, byte moveByte, int stepIncrementLong)//Need to implement
        {
            //http://stackoverflow.com/questions/1318933/c-int-to-byte
            byte[] result = BitConverter.GetBytes(stepIncrementLong);
            if (lambdaCom.isOpen())
            {
                lambdaCom.writeByte(byteCom.byteVFBatch);
                lambdaCom.writeByte(moveByte);
                lambdaCom.writeByte(222 /*byteCom.byteSetUSteps*/);
                lambdaCom.writeByte(result[0]/*stepIncrementLow*/);
                lambdaCom.writeByte(result[1]/*stepIncrementHigh*/);
            }
            return;
        }
    }
}
