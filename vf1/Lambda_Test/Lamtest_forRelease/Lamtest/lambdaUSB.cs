//FTD2XX_NET.DLL=>
/*http://www.ftdichip.com/Support/SoftwareExamples/CodeExamples/CSharp.htm
Examples 3 and 4 below both show how to use the FTD2XX_NET interface DLL.  
 * A reference should be added to the FTD2XX_NET.DLL file in the Solution Explorer.  
 * Simply right-click on the References item in the Solution Explorer in your 
 * Visual Studio project, select "Add Reference" then "Browse" and locate the DLL.  
 * The XML file should be placed in the same location.
 * 
 * PS- I was unable to get the .net wrapper to function so i suspect it
 * is non-functional. The FTD2XX_NET.DLL worked flawlessly.

 * ************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using FTD2XX_NET;

namespace Lamtest
{
    class FTDI_USB
    {
        FTDI myFtdiDevice = new FTDI();
        byte[] testArray = new byte[5];
        FTDI.FT_STATUS ftStatus;
        // Create new instance of the FTDI device class
        UInt32 ftdiDeviceCount;//Must be static to be USED!
        UInt32 defaultBaud;
        UInt32 test;
        public FTDI_USB()
        {
            ftStatus = FTDI.FT_STATUS.FT_OK;
            ftdiDeviceCount = 0;
            defaultBaud = 128000;
            test = 0;
            //OpenDevice();//It will crash if there is no FTI device present.
        }
        ~FTDI_USB()
        {
        }
        public void OpenDevice()
        {
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
                ftStatus = myFtdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber.ToString());
                ftStatus = myFtdiDevice.SetLatency(2);// SetLatencyTimer(2);
                ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
                ftStatus = myFtdiDevice.SetTimeouts(10, 0);
        }
        public void OpenBySerialNum(string s)
        {//Get the SerialNum of the specified device// Not tested
            ftStatus = myFtdiDevice.OpenBySerialNumber(s);
            ftStatus = myFtdiDevice.SetLatency(2);// SetLatencyTimer(2);
            ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            ftStatus = myFtdiDevice.SetTimeouts(10, 0);
        }
        public void OpenByDescription(string s)
        {//Get the SerialNum of the specified device// Not tested
            ftStatus = myFtdiDevice.OpenByDescription(s); //OpenBySerialNumber(s);
            ftStatus = myFtdiDevice.SetLatency(2);// SetLatencyTimer(2);
            ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            ftStatus = myFtdiDevice.SetTimeouts(10, 0);
        }
        public void OpenDeviceByIndex(uint i)
        {
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
                ftStatus = myFtdiDevice.OpenByIndex(i);
                ftStatus = myFtdiDevice.SetLatency(2);// SetLatencyTimer(2);
                ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);;
                ftStatus = myFtdiDevice.SetTimeouts(10, 0);
        }
        public void ClearBuffer()
        {
            ftStatus = myFtdiDevice.Purge(0);
            ftStatus = myFtdiDevice.Purge(1);
        }
        public void WriteByte(byte b)
        {
            byte[] myByte = new byte[] { b };
            UInt32 test = 0;
            ftStatus = myFtdiDevice.Write(myByte, 1, ref test);
        }
        public byte ReadByte()
        {
            byte[] testArray = new byte[5];
            ftStatus = myFtdiDevice.Read(testArray,1,ref test);
            return testArray[0];
        }
        public string Readto(uint n)
        {
            string testArray;
            myFtdiDevice.Read(out testArray,n,ref test);
            return testArray;
        }
        public void SetBaudrate(uint r)
        {
            ftStatus = myFtdiDevice.SetBaudRate(r);
        }
        public void SetTimeouts(uint a, uint b)
        {
            ftStatus = myFtdiDevice.SetTimeouts(a, b);
        }
        public bool IsOpen()
        {
            bool isOpen = true;
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                isOpen = false;
            }
            return isOpen;
        }
        public UInt32 GetDeviceCount()
        {
            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ftdiDeviceCount = 0;
            }
            return ftdiDeviceCount;
        }

        public string GetDescription()
        {//Get the Description of the first device// Not tested
            // Check status
            string myDescription;
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
            try
            {
                myDescription = ftdiDeviceList[0].Description.ToString();
            }
            catch
            {
                myDescription = "No device";
            }
        return myDescription;
        }
        public string GetDescription(UInt32 i)
        {//Get the Description of the specified device// Not tested
            // Check status
            string myDescription;
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
            myDescription = ftdiDeviceList[i].Description.ToString();
        return myDescription;
        }
        public string GetSerialNum()
        {//Get the SerialNum of the first device// Not tested
            // Check status
            string mySerialNum;
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
            try
            {
                mySerialNum = ftdiDeviceList[0].SerialNumber.ToString();
            }
            catch 
            {
                mySerialNum ="No Device";
            }
        return mySerialNum;
        }
        public string GetSerialNum(UInt32 i)
        {//Get the SerialNum of the specified device// Not tested
            // Check status
            string mySerialNum;
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
            try
            {
                mySerialNum = ftdiDeviceList[i].SerialNumber.ToString();
            }
            catch
            {
                mySerialNum = "No Device";
            }
        return mySerialNum;
        }
    }
}
/***************************************************
           
                   /*
        public string getFlags()
        {//Get the flags of the first device// Not tested
            // Check status
            string myFlags;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myFlags=( String.Format("{0:x}", ftdiDeviceList[0].Flags));
            }
            return myFlags;
        }
        public string getFlags(int i)
        {//Get the flags of the specified device// Not tested
            // Check status
            string myFlags;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myFlags=( String.Format("{0:x}", ftdiDeviceList[i].Flags));
            }
            return myFlags;
        }
        public string getType()
        {//Get the type of the first device// Not tested
            // Check status
            string myType;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myType=ftdiDeviceList[0].Type.ToString();
            }
            return myType;
        }
        public string getType(int i)
        {//Get the type of the specified device// Not tested
            // Check status
            string myType;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myType = ftdiDeviceList[i].Type.ToString();
            }
            return myType;
        }
        public string getID()
        {//Get the ID of the first device// Not tested
            // Check status
            string myID;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myID = String.Format("{0:x}", ftdiDeviceList[0].ID);
            }
            return myID;
        }
        public string getID(int i)
        {//Get the ID of the specified device// Not tested
            // Check status
            string myID;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myID = String.Format("{0:x}", ftdiDeviceList[i].ID);
            }
            return myID;
        }
        public string getLocID()
        {//Get the LocationID of the first device// Not tested
            // Check status
            string myID;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myID = String.Format("{0:x}", ftdiDeviceList[0].LocId);
            }
            return myID;
        }
        public string getLocID(int i)
        {//Get the LocationID of the specified device// Not tested
            // Check status
            string myID;
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                myID = String.Format("{0:x}", ftdiDeviceList[i].LocId));
            }
            return myID;
        */