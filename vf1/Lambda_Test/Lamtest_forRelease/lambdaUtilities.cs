using System;

namespace Lamtest
{
    public class lambdaUtilities
    {
        public byte getFilterByte(string myString)
        {
            string filter = myString;
            switch (filter)
            {
                case "Filter 0":
                    filterByte = 0;
                    break;
                case "Filter 1":
                    filterByte = 1;
                    break;
                case "Filter 2":
                    filterByte = 2;
                    break;
                case "Filter 3":
                    filterByte = 3;
                    break;
                case "Filter 4":
                    filterByte = 4;
                    break;
                case "Filter 5":
                    filterByte = 5;
                    break;
                case "Filter 6":
                    filterByte = 6;
                    break;
                case "Filter 7":
                    filterByte = 7;
                    break;
                case "Filter 8":
                    filterByte = 8;
                    break;
                case "Filter 9":
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
                    speedByte = (1 * 16);
                    break;
                case "Speed 2":
                    speedByte = (2 * 16);
                    break;
                case "Speed 3":
                    speedByte = (3 * 16);
                    break;
                case "Speed 4":
                    speedByte = (4 * 16);
                    break;
                case "Speed 5":
                    speedByte = (5 * 16);
                    break;
                case "Speed 6":
                    speedByte = (6 * 16);
                    break;
                case "Speed 7":
                    speedByte = (7 * 16);
                    break;
            }
            return speedByte;
        }
    }
}