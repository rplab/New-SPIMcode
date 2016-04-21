/*
**	FILENAME			lambdaUtils.cs
**
**	PURPOSE				This class is designed to handle utiltity mehtods for all 
 *                      lambda Filter Switchers.  There are utilties to combine 
 *                      filter and speed vlaues to an apropriate move byte. In addtion 
 *                      there are routines for error handleing as well as interpreting
 *                      the status return string.
**
**	CREATION DATE		10-01-2010
**	LAST MODIFICATION	10-04-2010
**
**	AUTHOR				Dan Carte
**
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lamtest
{
    class lambdaUtils
    {
        byteCommands byteCom = new byteCommands();
        byte speedByte = 1;//0 is a bad speed for most moves
        byte filterByte = 0;
        //Shutter methods_________________________________________________
        public int getHertz(int cycles)
        {
            int hertz = 22;
            if (cycles > 0 && cycles < 45)
            {
                hertz = 500 / (cycles);
            }
            else
            {
                hertz = 22;
            }

            return hertz;
        }
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
        //Generic methods_________________________________________________
        public int getByte(int myInt)
        {
            int speed = myInt;
            byte speedByte=1;
            switch (speed)
            {
                case 0:
                    speedByte = 0;
                    break;
                case 1:
                    speedByte = 1;
                    break;
                case 2:
                    speedByte = 2;
                    break;
                case 3:
                    speedByte = 3;
                    break;
                case 4:
                    speedByte = 4;
                    break;
                case 5:
                    speedByte = 5;
                    break;
                case 6:
                    speedByte = 6;
                    break;
                case 7:
                    speedByte = 7;
                    break;
            }
            return speedByte;
        }
        public byte getMoveByte(byte speed, byte filter)
        {
            byte moveByte=(byte)((speed*16) + filter);
            return moveByte;
        }
        public int getErrors(string controller,int totalTime, int s, int errors)
        {
            //12-14-2013 DELAY TIMES FOR ERRORS INCREASED BY 100MS AT SPEEDS 4-7? No idea why the time went up?
            switch (s)//calculate an error.
            {
                case 0:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 130)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 230)
                        {
                            errors++;
                        }
                    }
                    break;
                case 1:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 320)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 190)//190 or 220
                        {
                            errors++;
                        }
                    }
                    break;
                case 2:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 430)//220
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 300)//300 or 390
                        {
                            errors++;
                        }
                    }
                    break;
                case 3:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 280)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 350)//350 OR 410
                        {
                            errors++;
                        }
                    }
                    break;
                case 4:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 410)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 490)//490 OR 520
                        {
                            errors++;
                        }
                    }
                    break;
                case 5:
                    if (totalTime > 320)
                        if (controller != "LB10-2")
                        {
                            if (totalTime > 500)
                            {
                                errors++;
                            }
                        }
                        else
                        {
                            if (totalTime > 720)//720 OR 1000
                            {
                                errors++;
                            }
                        }
                    break;
                case 6:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 760)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 1350)//1300 or 1550
                        {
                            errors++;
                        }
                    }
                    break;
                case 7:
                    if (controller != "LB10-2")
                    {
                        if (totalTime > 1400)
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        if (totalTime > 2200)//2200 or 2700
                        {
                            errors++;
                        }
                    }
                    break;
            }
            return errors;
        }
        //Cofig methods____________________________________________________________________________
        public string getConfigString(string wheelA, string wheelB, string wheelC, string shutterA, string shutterB, string shutterC)
        {
            string config = "";
            switch (wheelA)
            {
                case "WA-25":
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-NWIQ =>  25mm 10 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-NWX => 25mm 10 poss. wheel without shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "WA-HS":
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-WHS4IQ =>  25mm High Speed 4 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-WHS4X => 25mm High Speed 4 poss.without shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "WA-32":
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-W32IQ =>  32mm 10 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-W32 => 32mm 10 poss. without a shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "WA-BD":
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-W4 + IQXX-SA=>  25mm 4 poss. belt drive wheel with a stand alone SmartShutter else where." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : LB10-W4 => 32mm 25mm 4 poss. belt drive wheel." + Environment.NewLine);
                    }
                    break;
                case "WA-NC":
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : Not Connected =>Uniblitz port active." + Environment.NewLine);
                    }
                    break;
                default:
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : Not Connected =>Uniblitz port active." + Environment.NewLine);
                    }
                break;
            }
            switch (wheelB)
            {
                case "WB-25":
                    if (shutterB == "SB-IQ")
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-NWIQ =>  25mm 10 poss. wheel with a SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-NWX => 25mm 10 poss.wheel without shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "WB-HS":
                    if (shutterB == "SB-IQ")
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-WHS4IQ =>  25mm High Speed 4 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-WHS4X => 25mm High Speed 4 poss. wheel without a shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "WB-32":
                    if (shutterB == "SB-IQ")
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-W32IQ =>  32mm 10 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-W32 => 32mm 10 possition without shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "WB-BD":
                    if (shutterB == "SB-IQ")
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :  LB10-W4 + IQXX-SA=>  25mm 4 poss. belt drive wheel with a stand alone SmartShutter else where." + Environment.NewLine);
                    }
                    else
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " : LB10-W4 => 32mm 25mm 4 poss. belt drive wheel." + Environment.NewLine);
                    }
                    break;
                case "WB-NC":
                    if (shutterB == "SB-IQ")
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config=config + ("Wheel: " + wheelB + " " + "Shutter: " + shutterB + " : Not Connected =>Uniblitz port active." + Environment.NewLine);
                    }
                    break;
                default:
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : Not Connected =>Uniblitz port active." + Environment.NewLine);
                    }
                    break;
            }
            switch (wheelC)
            {//Still have to deal with shutter C
                case "WC-25":
                    config=config + ("Wheel: " + wheelC + " :  LB10-NWX => 25mm 10 poss. wheel without shutter." + Environment.NewLine);
                    break;
                case "WC-HS":
                    config=config + ("Wheel: " + wheelC + " :  LB10-WHS4IQ =>  25mm High Speed 4 poss. wheel without shutter." + Environment.NewLine);
                    break;
                case "WC-32":
                    config=config + ("Wheel: " + wheelC + " :  LB10-W32 => 32mm 10 poss. without a shutter." + Environment.NewLine);
                    break;
                case "WC-BD":
                    config=config + ("Wheel: " + wheelC + ": LB10-W4 => 32mm 25mm 4 poss. belt drive wheel." + Environment.NewLine);
                    break;
                case "WC-NC":
                    if (shutterC == "SC-IQ")
                    {
                        config=config + ("Shutter: " + shutterC + " : SmartShutter on Port C." + Environment.NewLine);
                    }
                    else
                    {
                        config=config + ("Wheel: " + wheelC + " : Not Connected." + Environment.NewLine);
                    }
                    break;
            }
            return config;
        }
        public string getConfigString(string wheelA, string shutterA, string shutterB)
        {
            string config = "";
            switch (wheelA)
            {
                case "W-25":
                    if (shutterA == "S-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-NWIQ =>  25mm 10 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-NWX => 25mm 10 poss. wheel without shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "W-HS":
                    if (shutterA == "S-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-WHS4IQ =>  25mm High Speed 4 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-WHS4X => 25mm High Speed 4 poss.without shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "W-32":
                    if (shutterA == "S-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-W32IQ =>  32mm 10 poss. wheel with SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-W32 => 32mm 10 poss. without a shutter OR with a Uniblitz shutter." + Environment.NewLine);
                    }
                    break;
                case "W-BD":
                    if (shutterA == "S-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :  LB10-W4 + IQXX-SA=>  25mm 4 poss. belt drive wheel with a stand alone SmartShutter else where." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : LB10-W4 => 32mm 25mm 4 poss. belt drive wheel." + Environment.NewLine);
                    }
                    break;
                case "W-NC":
                    if (shutterA == "S-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : Not Connected =>Uniblitz port active." + Environment.NewLine);
                    }
                    if (shutterB == "SB-IQ")
                    {
                        config = ("Shutter: " + shutterB + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    break;
                default:
                    if (shutterA == "SA-IQ")
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " :IQXX-SA=>  Stand alone SmartShutter." + Environment.NewLine);
                    }
                    else
                    {
                        config = ("Wheel: " + wheelA + " " + "Shutter: " + shutterA + " : Not Connected =>Uniblitz port active." + Environment.NewLine);
                    }
                    break;
            }
            return config;
        }
        public string getConfigString()
        {
            string config = ("LB10-2: Lambda 10-2 or Lambda DG-4" + Environment.NewLine + "LB10-2 & DG-4 do not provide status for the filter wheels or shutters." + Environment.NewLine);
            return config;
        }
        public string getConfigString(string shutter)
        {
            string config = ("SC : LB-SC - Smart shutter controller." + Environment.NewLine);
            if (shutter == "S-IQ")
            {
                config = config +(shutter + ": Smart Shutter Connected." + Environment.NewLine);
            }
            else
            {
                config = config + (shutter + ": Smart Shutter NOT Connected." + Environment.NewLine);
            }
            
            return config;
        }
        public string getController(string status)
        {
            string controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                controller = "NA";
                return controller;
            }
            try
            {
                controller = status.Substring(1, 2);
                if (controller != "SC" && controller != "NA")
                {
                    controller = status.Substring(1, 4);
                    if (controller != "10-3" && controller != "10-B")
                    {
                        controller = status.Substring(0, 6);
                    }
                }
                return controller;
            }
            catch
            {
                controller = "NA";
                return controller;
            }
        }
        public string[] getWheelInfo(byte wheelByte)
        {
            byte wheelNum, speedInfo, posInfo;
            byte wheelByte1 = wheelByte;
            byte wheelByte2 = wheelByte;
            string[] wheelInfo = new string[3];
            wheelNum = (byte)(wheelByte1 << 7);
            byte wheelNum1= wheelNum;
            if (wheelNum == 1)
            {
                wheelInfo[0] = "Wheel B";
            }
            else
            {
                wheelInfo[0] = "Wheel A";
            }
            speedInfo = (byte)((wheelByte2 >> 4) - (wheelNum << 3));
            if (speedInfo > 7)
            {
                wheelInfo[1] = "Spd NA";
            }
            else
            {
                wheelInfo[1] = ("Spd: " + speedInfo.ToString());
            }
            posInfo = (byte)(wheelByte - ((speedInfo << 4 )+ (wheelNum1 << 7)));
            if (posInfo > 9)
            {
                wheelInfo[2] = "Pos: NA";
            }
            else
            {
                wheelInfo[2] = ("Pos: " + posInfo.ToString());
            }
            return wheelInfo;
        }
        public string getWheelA(string status)
        {
            string Wheel, controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                Wheel = "NA";
                return Wheel;
            }
            controller = getController(status);
            switch (controller)
            {
                case "10-3":
                    try { Wheel = status.Substring(5, 5); }
                    catch (Exception) // catches without assigning to a variable
                    {
                        Wheel = "N.A.";
                    }
                    break;
                case "10-B":
                    try { Wheel = status.Substring(5, 4); }
                    catch (Exception) // catches without assigning to a variable
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
        public string getWheelB(string status)
        {
            string Wheel, controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                Wheel = "NA";
                return Wheel;
            }
            controller = getController(status);
            switch (controller)
            {
                case "10-3":
                    try { Wheel = status.Substring(10, 5); }
                    catch (Exception) // catches without assigning to a variable
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
        public string getWheelC(string status)
        {
            string Wheel, controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                Wheel = "NA";
                return Wheel;
            }
            controller = getController(status);
            switch (controller)
            {
                case "10-3":
                    try { Wheel = status.Substring(15, 5); }
                    catch (Exception) // catches without assigning to a variable
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
        public string getShutterA(string status)
        {
            string Wheel, controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                Wheel = "NA";
                return Wheel;
            }
            controller = getController(status);
            switch (controller)
            {
                case "10-3":
                    try { Wheel = status.Substring(20, 5); }
                    catch (Exception) // catches without assigning to a variable
                    {
                        Wheel = "N.A.";
                    }
                    break;
                case "10-B":
                    try { Wheel = status.Substring(9, 4); }
                    catch (Exception) // catches without assigning to a variable
                    {
                        Wheel = "N.A.";
                    }
                    break;
                case "SC":
                    try { Wheel = status.Substring(9, 4); }
                    catch (Exception) // catches without assigning to a variable
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
        public string getShutterB(string status)
        {//need to deal with ND on BOTH A and B
            string Wheel, controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                Wheel = "NA";
                return Wheel;
            }
            controller = getController(status);
            switch (controller)
            {
                case "10-3":
                    try { Wheel = status.Substring(25, 5); }
                    catch (Exception) // catches without assigning to a variable
                    {
                        Wheel = "N.A.";
                    }
                    break;
                case "10-B":
                    if (status.Substring(5, 4) == "W-NC" && status.Length > 12)
                    {
                        try { Wheel = status.Substring(12, 4); }
                        catch (Exception) // catches without assigning to a variable
                        {
                            Wheel = "N.A.";
                        }
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
        public string getShutterC(string status)
        {//need to deal with ND on BOTH A and B
            string Wheel, controller;
            //string status = getConfig();
            if (status.Length < 2 || status == null || status == "NA")
            {
                Wheel = "NA";
                return Wheel;
            }
            controller = getController(status);
            if (status.Length > 30)
            {
                try { Wheel = status.Substring(30, 5); }
                catch (Exception) // catches without assigning to a variable
                {
                    Wheel = "N.A.";
                }
            }
            else
            {
                Wheel = "N.A.";
            }
            return Wheel;
        }
        //Form specific methods_________________________________________________
        public byte getFilterByte(string myString)
        {
            string filter = myString;
            switch (filter)
            {
                case "0":
                    filterByte = 0;
                    break;
                case "1":
                    filterByte = 1;
                    break;
                case "2":
                    filterByte = 2;
                    break;
                case "3":
                    filterByte = 3;
                    break;
                case "4":
                    filterByte = 4;
                    break;
                case "5":
                    filterByte = 5;
                    break;
                case "6":
                    filterByte = 6;
                    break;
                case "7":
                    filterByte = 7;
                    break;
                case "8":
                    filterByte = 8;
                    break;
                case "9":
                    filterByte = 9;
                    break;
            }
            return filterByte;
        }
        public byte getSpeedByte(string myString)
        {
            string speed = myString;
            switch (speed)
            {
                case "Speed 0":
                    speedByte = 0;
                    break;
                case "Speed 1":
                    speedByte = 1;
                    break;
                case "Speed 2":
                    speedByte = 2;
                    break;
                case "Speed 3":
                    speedByte = 3;
                    break;
                case "Speed 4":
                    speedByte = 4;
                    break;
                case "Speed 5":
                    speedByte = 5;
                    break;
                case "Speed 6":
                    speedByte = 6;
                    break;
                case "Speed 7":
                    speedByte = 7;
                    break;
            }
            return speedByte;
        }
    }
}
