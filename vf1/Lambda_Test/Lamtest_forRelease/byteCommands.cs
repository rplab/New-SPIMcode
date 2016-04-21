using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lamtest
{
    class byteCommands
    {
        //Wheel Commands
        public byte byteSelectC = 252;
        //Special commands
        public byte byteCR = 13;
        public byte byteGoOnline = 238;
        public byte byteGoLocal = 239;
        //Shutter Commands
        //LB-SC Special commands
        public byte byteDisableTTL = 160;
        public byte byteSetTTLHigh = 161;
        public byte byteSetTTLLow = 162;
        public byte byteSetTTLToggleRising = 163;
        public byte byteSetTTLToggleFalling = 164;
        public byte byteDisableSync = 176;
        public byte byteSyncHighOpen = 177;
        public byte byteSyncLowOpen = 178;
        public byte byteResetDefault = 192;
        public byte byteSetNewDefault = 193;
        public byte byteRestorLast = 251;
        public byte byteStopFreeRun = 191;
        public byte byteSetCycles = 240;
        public byte byteSetFreeRunPowerOn = 241;
        public byte byteSetFreeRunTTLIn= 242;
        public byte byteFreeRunCommand= 243;
        public byte byteLBSC_Prefix = 250;
        //Shutter A
        public byte byteOpenA = 170;
        public byte byteOpenACond = 171;
        public byte byteCloseA = 172;
        //Shutter B
        public byte byteOpenB = 186;
        public byte byteOpenBCond = 187;
        public byte byteCloseB = 188;
        //Shutter C
        public byte byteOpenC = 235;
        public byte byteOpenCCond = 236;
        public byte byteCloseC = 237;
        //Shutter Special commands
        public byte byteSetFast = 220;
        public byte byteSetSoft = 221;
        public byte byteSetND = 222;
        //batch commands
        public byte byteLB103Batch = 189;
        public byte byteLB102Batch = 223;
        //Status / config commands
        public byte byteGetStatus = 204;
        public byte byteGetType = 252;
        public byte byteGetConfig = 253;
        //Versa Chrome Commands
        public byte bytePowerOn = 206;
        public byte bytePowerOff = 207;
        public byte byteSetWlength = 218;
        public byte byteGetWlength = 219;
        public byte byteSetUSteps = 222;//uStep_low + uStep_High
        public byte byteVFBatch = 223;//DF
        public byte byteVFError = 234;//EA (return byte) Error - Wavelength N/A
        public byte byteGetVFAll = 250;//IF 252 + 250
        public byte byteVFReset = 251;
        public byte byteVFpossition = 252;//IF 252 + 250
        public byte byteGetHomePos = 254;//Gets the uSteps LB-SC / VF-5  only!
    }
}
