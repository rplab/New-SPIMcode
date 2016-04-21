using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lamtest
{
    public class vf5Functions 

    {
       byteCommands byteCom;
       private lambdaSerial com;//create comport object
       //private Mainform mf;
       public vf5Functions(lambdaSerial com)//com object passed to VF5fuctions object
       {
           byteCom = new byteCommands();
           this.com = com;//set reference of comport child to serial parrent
       }
       public void moveNM(byte speed, int freqency)
        {
            setWaveLegnth(speed, freqency);
            return;
        }
       public void moveNM(int freqency)
        {
            byte speed = 1;
            setWaveLegnth(speed, freqency);
            return;
        }
       public void setWaveLegnth(byte speed, int freqVal)
        {
            //http://stackoverflow.com/questions/1318933/c-int-to-byte
            byte[] intBytes = BitConverter.GetBytes(freqVal);
            byte mult = 64;
            //Array.Reverse(intBytes);
            byte[] result = intBytes;
            result[1] = (byte)(result[1] + (byte)(speed * mult));//Add the speed component to the high position byte
               com.writeByte(byteCom.byteSetWlength);
               com.writeByte(result[0]);
               com.writeByte(result[1]);
               com.txtBoxDialog.AppendText("New Possition: " + freqVal + Environment.NewLine); //**can no longer access mainform
            return;
        }
        //Utility
       //VF-5 methods_________________________________________________
       public int getRange(int wl)
       {
           int range = 60;
           switch (wl)
           {
               case 380:
                   range = 42;
                   break;
               case 440:
                   range = 52;
                   break;
               case 490:
                   range = 60;
                   break;
               case 550:
                   range = 64;
                   break;
               case 620:
                   range = 73;
                   break;
               case 700:
                   range = 84;
                   break;
               case 800:
                   range = 101;
                   break;
               default:
                   range = 60;
                   break;
           }
           return range;
       }
       public int[] getBaseFilters()
       {
           byte loop = 0;
           int[] baseVal = new int[5];
           int filter_0 = 0;
           int filter_1 = 0;
           int filter_2 = 0;
           int filter_3 = 0;
           int filter_4 = 0;
           com.writeByte(byteCom.byteVFpossition);
           com.clearBuffer();
           com.writeByte(byteCom.byteGetVFAll);
           while (loop != byteCom.byteCR)
           {
               Thread.Sleep(1);
               System.Windows.Forms.Application.DoEvents();
               loop = com.readByte();
               switch (loop)
               {
                   case 240:
                       filter_0 = com.readByte();
                       Thread.Sleep(1);
                       Thread.Sleep(1);
                       baseVal[0] = filter_0 + (256 * com.readByte());
                       break;
                   case 242:
                       Thread.Sleep(1);
                       filter_1 = com.readByte();
                       Thread.Sleep(1);
                       baseVal[1] = filter_1 + (256 * com.readByte());
                       break;
                   case 244:
                       Thread.Sleep(1);
                       filter_2 = com.readByte();
                       Thread.Sleep(1);
                       baseVal[2] = filter_2 + (256 * com.readByte());
                       break;
                   case 246:
                       Thread.Sleep(1);
                       filter_3 = com.readByte();
                       Thread.Sleep(1);
                       baseVal[3] = filter_3 + (256 * com.readByte());
                       break;
                   case 248:
                       //Thread.Sleep(1);
                       Thread.Sleep(1);
                       filter_4 = com.readByte();
                       Thread.Sleep(1);
                       baseVal[4] = filter_4 + (256 * com.readByte());
                       break;
               }
           }
           return baseVal;
       }
       //VF 5 Commands
      public string GetVFAll()
       {
           com.clearBuffer();
           com.writeByte(252);
           com.writeByte(byteCom.byteGetVFAll);
           string filterPoss = com.readString();
           return filterPoss;
       }
       public UInt16 getWaveLegnth()//Need to implement
       {
           byte[] readBytes = new byte[2];
           UInt16 w;
           com.clearBuffer();
           com.writeByte(byteCom.byteGetWlength);
           readBytes[0] = com.readByte();//Clear DB return
           readBytes[0] = com.readByte();
           Thread.Sleep(10);
           readBytes[1] = com.readByte();//txtBoxDialog.AppendText("base Posstion " + b + Environment.NewLine);
           w = BitConverter.ToUInt16(readBytes, 0);//w = int (w1 >> 8) + int (w2);
           return w;
       }
       public void setStepAngle(byte filterVal, byte moveByte, int stepIncrementLong)//Need to implement
       {
           //http://stackoverflow.com/questions/1318933/c-int-to-byte
           byte[] result = BitConverter.GetBytes(stepIncrementLong);
           if (com.isOpen())
           {
               com.writeByte(byteCom.byteVFBatch);
               com.writeByte(moveByte);
               com.writeByte(byteCom.byteSetUSteps);
               com.writeByte(result[0]);
               com.writeByte(result[1]);
           }
           return;
       }

//Sweep methods
        public void sweep380(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 380; i > 337; i--)//Filter 1
             {
                 if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false;  break; }//stop process, reset check box
                 com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                 System.Windows.Forms.Application.DoEvents(); 
                 moveNM(speed, i);
                 while (loop != byteCom.byteCR)
                 {
                     Thread.Sleep(1);
                     System.Windows.Forms.Application.DoEvents();
                     loop = com.readByte();
                     //com.txtBoxDialog.AppendText("Return" + loop + " ");
                 }
                 loop = 0;
                 com.txtBoxDialog.AppendText(Environment.NewLine);
                 Thread.Sleep(1000);
             }
             Thread.Sleep(1000);
        }
        public void sweep440(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 440; i > 387; i--)//Filter 2
            {
                if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                System.Windows.Forms.Application.DoEvents();
                moveNM(speed, i);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                    //com.txtBoxDialog.AppendText("Return" + loop + " ");
                }
                loop = 0;
                com.txtBoxDialog.AppendText(Environment.NewLine);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
        }
        public void sweep490(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 490; i > 429; i--)//Filter 3
            {
                if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                System.Windows.Forms.Application.DoEvents();
                moveNM(speed, i);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                    //com.txtBoxDialog.AppendText("Return" + loop + " ");
                }
                loop = 0;
                com.txtBoxDialog.AppendText(Environment.NewLine);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
        }
        public void sweep550(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 550; i > 486; i--)//Filter 4
            {
                if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                System.Windows.Forms.Application.DoEvents();
                moveNM(speed, i);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                    //com.txtBoxDialog.AppendText("Return" + loop + " ");
                }
                loop = 0;
                com.txtBoxDialog.AppendText(Environment.NewLine);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
        }
        public void sweep620(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 620; i > 547; i--)//Filter 5
            {
                if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                System.Windows.Forms.Application.DoEvents();
                moveNM(speed, i);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                    //com.txtBoxDialog.AppendText("Return" + loop + " ");
                }
                loop = 0;
                com.txtBoxDialog.AppendText(Environment.NewLine);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
        }
        public void sweep700(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 700; i > 621; i--)//Filter 5
            {
                if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                System.Windows.Forms.Application.DoEvents();
                moveNM(speed, i);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                    //com.txtBoxDialog.AppendText("Return" + loop + " ");
                }
                loop = 0;
                com.txtBoxDialog.AppendText(Environment.NewLine);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
        }
        public void sweep800(byte speed)
        {
            byte loop = 0;
            com.txtBoxDialog.AppendText("Sweep started:" + Environment.NewLine); //**can no longer access mainform
            for (int i = 800; i > 699; i--)//Filter 5
            {
                if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                System.Windows.Forms.Application.DoEvents();
                moveNM(speed, i);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                    //com.txtBoxDialog.AppendText("Return" + loop + " ");
                }
                loop = 0;
                com.txtBoxDialog.AppendText(Environment.NewLine);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
        }
        public void Step5()
        {
            Thread.Sleep(10);
            //byte[] intBytes;
            int[] baseFilter = new int[5];
            byte loop = 0;
            byte speed = 3;
            baseFilter = getBaseFilters();
            for (int i = 0; i < 5; i++)
            {
                com.txtBoxDialog.AppendText(" Posstion: " + baseFilter[i] + Environment.NewLine);
                moveNM(speed, baseFilter[i]);
                Thread.Sleep(10);
                while (loop != byteCom.byteCR)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                    loop = com.readByte();
                }
                loop = 0;
                Thread.Sleep(1000);
                System.Windows.Forms.Application.DoEvents();
                if (com.stop.Checked) { i = 5; }
            }
        }
        public void sweepAll()
        {
            byte[] intBytes = new byte[2];
            byte loop = 0;
            byte speed = 3;
            int[] baseFilter = new int[5];
            baseFilter = getBaseFilters();
            com.stop.Checked = false;
            for (int i = 0; i < 5; i++)
            {
                int range = getRange(baseFilter[i]);
                for (int j = baseFilter[i]; j > (baseFilter[i] - range); j--)//Filter 1
                {
                    if (com.isOpen() == false || com.stop.Checked) { com.stop.Checked = false; break; }//stop process, reset check box
                    com.txtBoxDialog.AppendText("New Possition" + i + " nM" + Environment.NewLine); //**can no longer access mainform
                    System.Windows.Forms.Application.DoEvents();
                    moveNM(speed, j);
                    while (loop != byteCom.byteCR)
                    {
                        Thread.Sleep(1);
                        System.Windows.Forms.Application.DoEvents();
                        loop = com.readByte();
                        //com.txtBoxDialog.AppendText("Return" + loop + " ");
                    }
                    loop = 0;
                    com.txtBoxDialog.AppendText(Environment.NewLine);
                    Thread.Sleep(1000);
                }

            }
        }
    }
}
/* http://stackoverflow.com/questions/1940165/how-to-access-to-the-parent-object-in-c-sharp
Question
--I have a "meter" class. One property of "meter" is another class called "production". 
-- I need to access to a property of meter class (power rating) from production class by reference. 
--The powerRating is not known at the instantiation of Meter.


public class lambdaSerial//Meter
{
   --private vf5Functions vf5;/Production _production;

   public lambdaSerial();//Meter()
   {
      vf5 = new vf5Functions();//_production = new Production();
   }
}
Answer
public class vf5Functions{//Production {
  //The other members, properties etc...
 
  private lambdaSerial com;// Meter m;

  Production(lambdaSerial com){ //Meter m) {
    this.com = com;
  }
 public class lambdaSerial//Meter
{
   private vf5Functions vf5;//Production _production;

   public lambdaSerial()//Meter()
   {
      vf5 = new vf5Functions(this);//_production = new Production(this);
   }
}
}
*/