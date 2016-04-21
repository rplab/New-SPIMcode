using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lamtest
{
    class byteCommands
    {
        //Wheel Commands
        public byte byteSelectC = 0xFC;//252;
        //Special commands
        public byte byteCR = 0x0D;//13;
        public byte byteGoOnline = 0xEE;//238;
        public byte byteGoLocal = 0xEF;//239;
        //Shutter Commands
        //LB-SC Special commands
        public byte byteDisableTTL = 0xA0;//160;
        public byte byteSetTTLHigh = 0xA1;//161;
        public byte byteSetTTLLow = 0xA2;//162;
        public byte byteSetTTLToggleRising = 0xA3;//163;
        public byte byteSetTTLToggleFalling = 0xA4;//164;
        public byte byteDisableSync = 0xB0;//176;
        public byte byteSyncHighOpen = 0xB1;//177;
        public byte byteSyncLowOpen = 0xB2;//178;
        public byte byteResetDefault = 0xC0;//192;
        public byte byteSetNewDefault = 0xC1;//193;
        public byte byteRestorLast = 0xFB;//251;
        public byte byteStopFreeRun = 0xBF;//191;
        public byte byteSetCycles = 0xF0;//240;
        public byte byteSetFreeRunPowerOn = 0xF1;//241;
        public byte byteSetFreeRunTTLIn = 0xF2;//242;
        public byte byteFreeRunCommand = 0xF3;//243;
        public byte byteLBSC_Prefix = 0xFA;//250;
        //Shutter A
        public byte byteOpenA = 0xAA;//170;
        public byte byteOpenACond = 0xAB;//171;
        public byte byteCloseA = 0xAC;//172;
        //Shutter B
        public byte byteOpenB = 0xBA;//186;
        public byte byteOpenBCond = 0xBB;//187;
        public byte byteCloseB = 0xBC;//188;
        //Shutter C
        public byte byteOpenC = 0xEB;//235;
        public byte byteOpenCCond = 0xEC;//236;
        public byte byteCloseC = 0xED;//237;
        //Shutter Special commands
        public byte byteSetFast = 0xDC;//220;
        public byte byteSetSoft = 0xDD;//221;
        public byte byteSetND = 0xDE;//222;
        //batch commands
        public byte byteLB103Batch = 0xBD;//189;
        public byte byteLB102Batch = 0xDF;//223;
        //Status / config commands
        public byte byteGetStatus = 0xCC;//204;
        public byte byteGetType = 0xFC;//252;
        public byte byteGetConfig = 0xFD;//253;
        //Versa Chrome Commands
        public byte bytePowerOn = 0xCE;//206;
        public byte bytePowerOff = 0xCF;//207;
        public byte byteSetWlength = 0xDA;//218;
        public byte byteGetWlength = 0xDB;//219;
        public byte byteSetUSteps = 0xDE;//222;//uStep_low + uStep_High
        public byte byteVFBatch = 0xDF;//223;//DF
        public byte byteVFError = 0xEA;//234;//EA (return byte) Error - Wavelength N/A
        public byte byteGetVFAll = 0xFA;//250;//IF 252 + 250
        public byte byteVFReset = 0xFB;//251;
        public byte byteVFpossition = 0xFC;//252;//IF 252 + 250
        public byte byteGetHomePos = 0xFE;//254;//Gets the uSteps LB-SC / VF-5  only!
    }
}
