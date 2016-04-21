//Origonal source
//http://msmvps.com/blogs/coad/archive/2005/03/23/SerialPort-_2800_RS_2D00_232-Serial-COM-Port_2900_-in-C_2300_-.NET.aspx
//C# namespace + Includes
//http://www.aspfree.com/c/a/C-Sharp/C-Sharp-Classes-Explained/
//Saving + printing the text box
//http://dotnetperls.com/textbox
//http://www.c-sharpcorner.com/UploadFile/mgold/NotepadDotNet07312005142055PM/NotepadDotNet.aspx
/*
**	FILENAME			lambdaSerial.cs
**
**	PURPOSE				This class is the fron end of the aplication.  LambdaSerial.cs
 *                      handels the communications, this inlcudes special labda
 *                      commands.  lambdautil handeld special function perculiar to 
 *                      the lambda imaging line.
**
**	CREATION DATE		10-01-2010
**	LAST MODIFICATION	10-04-2010
**
**	AUTHOR				Dan Carte
**
*/

/*To DO:
 * 5- harmonize feild names txtThisBox
 * 6- ADD LPT PORTS
 * 7- ADD USB PORTS?
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
//FTDI 
using FTD2XX_NET;

namespace Lamtest
{
    public partial class Mainform : Form
    {
        lambdaSerial lambdaCom = new lambdaCom();
        lambdaUtils lamUtil = new lambdaUtils();
        byteCommands byteCom = new byteCommands();
        Random random = new Random();
        //Stream needed for ring buffer
        StreamReader tr;
        StreamWriter tw;
        string[] filterValue = new string[50];
        //end stream stuff
        byte speedByte = 1;//0 is a bad speed for most moves
        byte filterByte = 0;
        byte moveByte = 0;
        string wheel, controller, wheelAConfig, wheelBConfig, wheelCConfig;
        bool stopProcess = false;

        public Mainform()
        {
            InitializeComponent();
            for (int i = 0; i <= 49; i++)
            {
                filterValue[i] = "eof";
            }
            //lambdaCom.openCom();
        }
        private void Mainform_Load(object sender, EventArgs e)
        {
            //txtCom.AppendText("devCount" + ftdiDeviceCount.ToString() + Environment.NewLine);
        }
        private void txtComPort_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("You can add custom values here if necessary." + Environment.NewLine);
        }
        private void btnOpenCom_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Opens the com port and reports the status of the unit." + Environment.NewLine);
        }
        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            //comPort.Text.ToString();
            string myMode = txtComPort.Text.ToString();
            lambdaCom.openCom(myMode);
            //lambdaParallel.Output(888, 1);
            uint test=1;
            string mySerial;
            byte[] txbuf= new byte[25];
            byte[] rxbuf= new byte[25];
           /* FTDI lambdaUSB = new FTDI();
            FTDI.FT_DEVICE_INFO_NODE[] deviceList = new FTDI.FT_DEVICE_INFO_NODE [10];
            lambdaUSB.Close();
            */
            if (myMode=="USB") 
            {         
  
            }
                /*while(lambdaUSB.IsOpen==false){
                lambdaUSB.ResetPort();
                lambdaUSB.SetTimeouts(300, 300);
                lambdaUSB.Purge(0);
                lambdaUSB.Purge(1);
                lambdaUSB.SetLatency(2);
                lambdaUSB.SetBaudRate(9600);
                lambdaUSB.InTransferSize(64);
                for (int i = 0; i < 5; i++)
                {
                    lambdaUSB.CyclePort();
                    txtCom.AppendText("Befor-" + lambdaUSB.IsOpen + Environment.NewLine);
                    //lambdaUSB.OpenByDescription("Sutter Instrument Lambda SC"); 
                    lambdaUSB.OpenByLocation(0); //DID NOT WORK
                    txtCom.AppendText("Location" + lambdaUSB.IsOpen + Environment.NewLine);
                    lambdaUSB.OpenByDescription("Sutter Instrument Lambda 10-3");
                    txtCom.AppendText("Desciption" + lambdaUSB.IsOpen + Environment.NewLine);
                    lambdaUSB.OpenByIndex(1);
                    txtCom.AppendText("Index" + lambdaUSB.IsOpen + Environment.NewLine);
                    lambdaUSB.OpenBySerialNumber("SIWTVRUF");
                    txtCom.AppendText("serial#" + lambdaUSB.IsOpen + Environment.NewLine);
                    txtCom.AppendText("END lOOP- " + i  + Environment.NewLine);
                }
                txbuf[0] = 238;
                lambdaUSB.Write(txbuf, 1, ref test);
                txtCom.AppendText("After-1 " + test + Environment.NewLine);// return 1
                txbuf[0] = 33;
                lambdaUSB.Write(txbuf, 1, ref test);
                txtCom.AppendText("After-2 " + test + Environment.NewLine);// return 1
                lambdaUSB.Read(rxbuf, 2, ref test);
                txtCom.AppendText("After-rxbuf[0] " + rxbuf[0] + Environment.NewLine);// return 1
                txtCom.AppendText("After-rxbuf[1] " + rxbuf[1] + Environment.NewLine);// return 1
                //txbuf[0] = 170;
                //lambdaUSB.Write(txbuf, 1, ref test);
    
                //for (int i = 0; i< 100; i++)
                //{
                lambdaUSB.GetNumberOfDevices(ref test); //It works =1;
                txtCom.AppendText("After-3 " + test + Environment.NewLine);// return 1
                //lambdaUSB.GetSerialNumber(out mySerial);// Blank string ?/ ""/
                //txtCom.AppendText("After-2 " + mySerial + Environment.NewLine);// return 1
                lambdaUSB.GetDeviceList(deviceList);
                //for (int i = 0; i < 9; i++)
                //{
                txtCom.AppendText("After-3 " + deviceList[0] + Environment.NewLine);//Only zero on this ssytem
                //}
                lambdaUSB.OpenByIndex(1);
                //lambdaUSB.GetDescription(out mySerial);//no Return
                //txtCom.AppendText("After-4 " + mySerial + Environment.NewLine);// return 1
                lambdaUSB.GetDriverVersion(ref test); //Does nothing
                txtCom.AppendText("After-5 " + test + Environment.NewLine);// return 1
                lambdaUSB.GetDeviceID(ref test);//No return
                txtCom.AppendText("After-6 " + test + Environment.NewLine);// return 1
                    //
                //txtCom.AppendText("deviceList.ToString()-" + deviceList.ToString() + Environment.NewLine);
                //txtCom.AppendText("mySerial.ToString()-" + test + Environment.NewLine);
               //}
            }*/
            lambdaCom.clearBuffer();
            lambdaCom.writeByte(byteCom.byteGoOnline);
            txtCom.AppendText("W-238" + Environment.NewLine);
            readPort();
            string controller= lambdaCom.getController();
            txtCom.AppendText("R-" + controller + Environment.NewLine);
            if (lambdaCom.isOpen13() || controller == "LB10-2")
            {
                txtBoxDialog.AppendText(txtComPort.Text.ToString() + " :  is Open" + Environment.NewLine);
            }
            else
            {
                txtBoxDialog.AppendText("Could not open: " + txtComPort.Text.ToString() + Environment.NewLine);
            }
            lambdaCom.clearBuffer();
            writeConfig();
            controller = lambdaCom.getController();
            lambdaCom.clearBuffer();
            txtBoxDialog.AppendText("End" + Environment.NewLine);
        }
        private void btnClose_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Closes the com port and clears dialog box." + Environment.NewLine);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            txtBoxDialog.Text = "";
            lambdaCom.closeCom();
            txtCom.AppendText("W-Close Port" + Environment.NewLine);
            readPort();
        }
        //**************************************************************************************************************
        //Works with a DG-4 as well.
        private void radioFilter0_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 0." + Environment.NewLine);
        }
        private void filterButton0_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter0.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter0.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte+ Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter1_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 1." + Environment.NewLine);
        }
        private void filterButton1_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter1.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter1.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter2_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 2." + Environment.NewLine);
        }
        private void filterButton2_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter2.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter2.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter3_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 3." + Environment.NewLine);
        }
        private void filterButton3_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter3.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter3.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter4_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 4." + Environment.NewLine);
        }
        private void filterButton4_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter4.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter4.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter5_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 5." + Environment.NewLine);
        }
        private void filterButton5_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter5.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter5.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter6_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 6." + Environment.NewLine);
        }
        private void filterButton6_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter6.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter6.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter7_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 7." + Environment.NewLine);
        }
        private void filterButton7_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter7.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter7.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter8_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 8." + Environment.NewLine);
        }
        private void filterButton8_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter8.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter8.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void radioFilter9_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("On click moves to posstion 9." + Environment.NewLine);
        }
        private void filterButton9_Click(object sender, EventArgs e)
        {
            if (lambdaCom.isOpen() && radioFilter9.Checked)
            {
                speedByte = (byte)(lamUtil.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lamUtil.getFilterByte(radioFilter9.Text.ToString());
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                wheel = txtWheelBox.Text.ToString();
                moveMyWheel(wheel, moveByte);
                txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + moveByte.ToString() + Environment.NewLine);
            }
            return;
        }

        private void txtSpeedBox_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Set the movement speed." + Environment.NewLine);
            txtHelp.AppendText("Speed 0 is reserved for the high speed 4 possition wheel." + Environment.NewLine);
        }

        private void txtWheelBox_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Set the active wheel." + Environment.NewLine);
        }
        //Send Command**************************************************************************************************
        private void btnSendCommand_MouseClick(object sender, MouseEventArgs e)
        {
            byte myCommand = (byte)decCommand.Value;
            lambdaCom.writeByte(myCommand);
            byte returnString= lambdaCom.readByte();
            txtCom.AppendText("R1-" + returnString + Environment.NewLine);
            returnString = lambdaCom.readByte();
            txtCom.AppendText("R2-" + returnString + Environment.NewLine);
        }
        //**************************************************************************************************************
        //Shutter test pannel.
        private void txtShutterSelect_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Select the acive shutter." + Environment.NewLine);
        }
        private void radioOpenA_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Opens shutter A." + Environment.NewLine);
            txtHelp.AppendText("The mode is set by 'Shutter mode'." + Environment.NewLine);
        }
        private void txtModeA_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText(" Shutter mode: Default mode open in the last configuration." + Environment.NewLine);
        }

        private void decNDA_Enter(object sender, EventArgs e)
        {
            txtHelp.AppendText("For ND mode. Useful values 16-128" + Environment.NewLine);
        }
        private void openAButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioOpenA.Checked)
            {
                string shutterMode = txtModeA.Text.ToString();
                byte ndSetting = (byte)decNDA.Value;
                switch (shutterMode)
                {
                    case "Fast":
                        lambdaCom.openA();
                        txtCom.AppendText("W-Open A fast" + Environment.NewLine);
                        readPort();
                        break;
                    case "Soft":
                        lambdaCom.openASoft();
                        txtCom.AppendText("W-Open A soft" + Environment.NewLine);
                        readPort();
                        break;
                    case "ND Mode":
                        lambdaCom.openA(ndSetting);
                        txtCom.AppendText("W-Open A ND" + ndSetting + Environment.NewLine);
                        readPort();
                        break;
                    case "Defualt Mode":
                        lambdaCom.openAdefault();
                        txtCom.AppendText("W-Open A Default" + ndSetting + Environment.NewLine);
                        readPort();
                        break;
                    default:
                        lambdaCom.openAdefault();
                        txtCom.AppendText("W-Open A Default" + Environment.NewLine);
                        readPort();
                        break;
                }
                txtBoxDialog.AppendText(" Shutter A: Opened " + shutterMode + "mode " + Environment.NewLine);
            }
            return;
        }

        private void radioCloseA_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Close shutter A." + Environment.NewLine);
        }
        private void closeAButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioCloseA.Checked)
            {
                lambdaCom.closeShutterA();
                txtCom.AppendText("W-Close Shutter A"  + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(" Shutter A: Closed " + Environment.NewLine);
            }
            return;
        }

        private void radioOpenB_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Opens shutter B." + Environment.NewLine);
            txtHelp.AppendText("The mode is set by 'Shutter mode'." + Environment.NewLine);
        }
        private void txtModeB_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText(" Shutter mode: Default mode open in the last configuration." + Environment.NewLine);
        }
        private void decNDB_Enter(object sender, EventArgs e)
        {
            txtHelp.AppendText("For ND mode. Useful values 16-128" + Environment.NewLine);
        }
        private void openBButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioOpenB.Checked)
            {
                string shutterMode = txtModeB.Text.ToString();
                byte ndSetting = (byte)decNDB.Value;
                switch (shutterMode)
                {
                    case "Fast":
                        lambdaCom.openB();
                        txtCom.AppendText("W-Open B fast" + Environment.NewLine);
                        readPort();
                        break;
                    case "Soft":
                        lambdaCom.openBSoft();
                        txtCom.AppendText("W-Open B soft" + Environment.NewLine);
                        readPort();
                        break;
                    case "ND Mode":
                        lambdaCom.openB(ndSetting);
                        txtCom.AppendText("W-Open B ND" + ndSetting + Environment.NewLine);
                        readPort();
                        break;
                    case "Defualt Mode":
                        lambdaCom.openBdefault();
                        txtCom.AppendText("W-Open B Default" + Environment.NewLine);
                        readPort();
                        break;
                    default:
                        lambdaCom.openBdefault();
                        txtCom.AppendText("W-Open B Default" + Environment.NewLine);
                        readPort();
                        break;
                }
                txtBoxDialog.AppendText(" Shutter B: Opened " + shutterMode + "mode " + Environment.NewLine);
            }
            return;
        }

        private void radioCloseB_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Close shutter B." + Environment.NewLine);
        }
        private void closeBButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioCloseB.Checked)
            {
                lambdaCom.closeShutterB();
                txtCom.AppendText("W-Close Shutter B" + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(" Shutter B: Closed " + Environment.NewLine);
            }
            return;
        }

        private void radioOpenC_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Opens shutter C." + Environment.NewLine);
            txtHelp.AppendText("The mode is set by 'Shutter mode'." + Environment.NewLine);
            txtHelp.AppendText("The jumper MUST be set for a shutter on port C" + Environment.NewLine);
        }
        private void txtModeC_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText(" Shutter mode: Default mode open in the last configuration." + Environment.NewLine);
        }
        private void decNDC_Enter(object sender, EventArgs e)
        {
            txtHelp.AppendText("For ND mode. Useful values 16-128" + Environment.NewLine);
        }
        private void openCButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioOpenC.Checked)
            {
                string shutterMode = txtModeC.Text.ToString();
                byte ndSetting = (byte)decNDC.Value;
                switch (shutterMode)
                {
                    case "Fast":
                        lambdaCom.openC();
                        txtCom.AppendText("W-Open C Fast" + Environment.NewLine);
                        readPort();
                        break;
                    case "Soft":
                        lambdaCom.openCSoft();
                        txtCom.AppendText("W-Open C Soft" + Environment.NewLine);
                        readPort();
                        break;
                    case "ND Mode":
                        lambdaCom.openC(ndSetting);
                        txtCom.AppendText("W-Open C ND" + ndSetting  + Environment.NewLine);
                        readPort();
                        break;
                    case "Defualt Mode":
                        lambdaCom.openCdefault();
                        txtCom.AppendText("W-Open C Default"  + Environment.NewLine);
                        readPort();
                        break;
                    default:
                        lambdaCom.openCdefault();
                        readPort();
                        break;
                }
                txtBoxDialog.AppendText(" Shutter C: Opened " + shutterMode + "mode " + Environment.NewLine);
            }
            return;
        }

        private void radioCloseC_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Close shutter C." + Environment.NewLine);
        }
        private void closeCButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioCloseC.Checked)
            {
                lambdaCom.closeShutterC();
                txtCom.AppendText("W-Close Shutter C" + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(" Shutter C: Closed " + Environment.NewLine);
            }
            return;
        }

        private void decHertz_Enter(object sender, EventArgs e)
        {
            txtHelp.AppendText("Choose the desired frequency for the test." + Environment.NewLine);
            txtHelp.AppendText("Frequencies above 18hz can be unstable." + Environment.NewLine);
            txtHelp.AppendText("Frequencies from 28-42hz are extreemly unstable." + Environment.NewLine);
            txtHelp.AppendText("42hz for the  25mm and 38hz for the 35mm can be quite good." + Environment.NewLine);
        }

        private void btnShutterTest_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Test the designated shutter at the designated frequency." + Environment.NewLine);
        }
        private void shutterTestbutton_Click(object sender, EventArgs e)
        {
            stopProcess = false;
            string shutter = txtShutterSelect.Text.ToString();
            int hertz = (int)decHertz.Value;
            hertz = lamUtil.getHertz(hertz);
            byte loop;
            do
            {
                switch (shutter)
                {
                    case "A":
                        lambdaCom.openA();
                        txtCom.AppendText("W-Open A" + Environment.NewLine);
                        readPort();
                        Thread.Sleep(hertz);
                        lambdaCom.closeShutterA();
                        txtCom.AppendText("W-Open A" + Environment.NewLine);
                        readPort();
                        break;
                    case "B":
                        lambdaCom.openB();
                        Thread.Sleep(hertz);
                        lambdaCom.closeShutterB();
                        break;
                    case "C":
                        lambdaCom.openC();
                        Thread.Sleep(hertz);
                        lambdaCom.closeShutterC();
                        break;
                }
                Application.DoEvents();
                Thread.Sleep(hertz);
            } while (lambdaCom.isOpen() || stopProcess == false);
            return;
        }
        //************************************************************************************
        //Wheel test pannel
        private void chkBoxAddDel0_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 0." + Environment.NewLine);
            txtHelp.AppendText("Speed 0 should only be used with a high speed wheel." + Environment.NewLine);
        }
        private void chkBoxAddDel1_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 1." + Environment.NewLine);
            txtHelp.AppendText("Speed 1 is best for a light to marginaly loaded wheel" + Environment.NewLine);
        }
        private void chkBoxAddDel2_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 2." + Environment.NewLine);
            txtHelp.AppendText("Speed 2 tends to be rough unlees the wheel is loaded." + Environment.NewLine); 
            txtHelp.AppendText("So speed 2 requires additional delay." + Environment.NewLine);
        }
        private void chkBoxAddDel3_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 3." + Environment.NewLine);
            txtHelp.AppendText("Speed 3 tends to be rough unlees the wheel is loaded." + Environment.NewLine);
            txtHelp.AppendText("So speed 3 requires additional delay." + Environment.NewLine);
        }
        private void chkBoxAddDel4_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 4." + Environment.NewLine);
            txtHelp.AppendText("Speed 4 tends to be rough unlees the wheel is loaded." + Environment.NewLine);
            txtHelp.AppendText("So speed 4 might require additional delay." + Environment.NewLine);
        }
        private void chkBoxAddDel5_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 5." + Environment.NewLine);
            txtHelp.AppendText("Speed 5 tends to be rough unlees the wheel is loaded." + Environment.NewLine);
            txtHelp.AppendText("Speed 5 should be reliable without delay." + Environment.NewLine);
        }
        private void chkBoxAddDel6_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 6." + Environment.NewLine);
            txtHelp.AppendText("Speed 6 tends to be rough unlees the wheel is loaded." + Environment.NewLine);
            txtHelp.AppendText("Speed 6 should be reliable without delay." + Environment.NewLine);
        }
        private void chkBoxAddDel7_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Increased delay between moves for speed 7." + Environment.NewLine);
            txtHelp.AppendText("Speed 7 tends to be rough unlees the wheel is loaded." + Environment.NewLine);
            txtHelp.AppendText("Speed 7 should be reliable without delay." + Environment.NewLine);
        }
        private void DecDelayMultiplier_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Multiplies the standard delay by 1-5." + Environment.NewLine);
            txtHelp.AppendText("For cheched boxes only." + Environment.NewLine);
        }
        private void txtDelay_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("The delay in mili-seconds to be used in the test." + Environment.NewLine);
        }
        private void txtNumSteps_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("The number of steps to be used in the test." + Environment.NewLine);
        }
        private void txtTopSpeed_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("The fastest speed to be used in the test." + Environment.NewLine);
        }
        private void txtLastSpeed_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("The slowest speed to be used in the test." + Environment.NewLine);
        }
        private void txtTestMode_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("This box determines the wheels to be tested." + Environment.NewLine);
            txtHelp.AppendText("LB10-3 batch mode tests all 3 wheel." + Environment.NewLine);
            txtHelp.AppendText("LB10-2 batch mode wheels A && B only." + Environment.NewLine);
        }
        private void btnTestWheel_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Performs a test moving the wheels to a random possition from 0-9 using the specified paramters." + Environment.NewLine);
        }
        private void testButton_Click(object sender, EventArgs e)
        {
            if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")
            {
                txtBoxDialog.AppendText("The error detection does not work on the parallel port!" + Environment.NewLine);
            }
            testWheel();
        }

        private void btnRandomTest_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Performs a test of the wheels to a random possition from 0-9 using a random delay." + Environment.NewLine);
            txtHelp.AppendText("This test us useful for findind a good speed / delay combination for the current load of the wheel." + Environment.NewLine);
        }
        private void randomButton_Click(object sender, EventArgs e)
        {
            if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")
            {
                txtBoxDialog.AppendText("The error detection does not work on the parallel port!" + Environment.NewLine);
            }
            randomTest();
        }
        private void filter1UpDown_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Determines the first posstion to test in the fixed test." + Environment.NewLine);
        }
        private void filter2UpDown_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Determines the second posstion to test in the fixed test." + Environment.NewLine);
        }

        private void btnFixTest_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Performs a test of the wheels to two possitions using a random delay." + Environment.NewLine);
            txtHelp.AppendText("This test us useful for findind a good speed / delay combination." + Environment.NewLine);
            txtHelp.AppendText("For the current load of the wheel between 2 predetermined posstions." + Environment.NewLine);
        }
        private void fixTestButton_MouseClick(object sender, MouseEventArgs e)
        {
            stopProcess = false;
            bool Exit= false;
            int errors, topSpeed, lastSpeed, cycleCount, delay, filter1, filter2;
            errors = 0;
            delay = int.Parse(txtDelay.Text.ToString());
            speedByte = 0;
            topSpeed = (int)lamUtil.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lamUtil.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = 100;
            filter1 = (int)filter1UpDown.Value;
            filter2 = (int)filter2UpDown.Value;
            wheel = txtTestMode.Text.ToString();
            controller = lambdaCom.getController();
            if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")
            {
                txtBoxDialog.AppendText("The error detection does not work on the parallel port!" + Environment.NewLine);
            }
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA();
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB();
                wheelCConfig = lambdaCom.getWheelC();
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }//escape
                //char sCheck = Convert.ToChar(s);//toString(s);
               // do
               // {
                    //myRandom does the actual loop.  The do - while statment determins the delay
                    errors = myRandom(cycleCount, delay, s, filter1, filter2, "fixedDelay");
                   /* if (errors > 0 && delay < 200)
                    {
                        delay = delay + 10;
                        errors = 0;
                    }
                    else
                    {
                        Exit= true;//should exit out when there are no errors
                    }

                } while (delay < 200 && stopProcess != true && Exit != true);*/
                if (errors > 0)
                {
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " is not a good speed at delay-" + delay.ToString()+ " . " + Environment.NewLine);
                }
                else
                {
                    txtBoxDialog.AppendText("A goog speed is:" + Environment.NewLine);
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " at  delay " + delay.ToString() + Environment.NewLine);
                }
                //delay = 10;
                errors = 0;
                stopProcess = false;
                Exit = false;
            }
        }
        private void HS4TestButton_Click(object sender, EventArgs e)
        {
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay, filter1, filter2;
            errors = 0;
            delay = int.Parse(txtDelay.Text.ToString());
            speedByte = 0;
            topSpeed = (int)lamUtil.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lamUtil.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse(txtNumSteps.Text.ToString());
            filter1 = (int)filter1UpDown.Value;
            filter2 = (int)filter2UpDown.Value;
            wheel = txtTestMode.Text.ToString();
            controller = lambdaCom.getController();
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA();
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB();
                wheelCConfig = lambdaCom.getWheelC();
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }//escape
                //char sCheck = Convert.ToChar(s);//toString(s);
                errors = myRandom(cycleCount, delay, s, filter1, filter2, "fixedDelay");
                if (errors > 0)
                {
                    txtBoxDialog.AppendText(errors + " Errors at speed " + s.ToString() + ". At Delay " + delay + Environment.NewLine);
                }
                else
                {
                    txtBoxDialog.AppendText("A goog speed is:" + Environment.NewLine);
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " at  delay " + delay.ToString() + Environment.NewLine);
                }
                errors = 0;
            }
        }
        //************************************************************************************************
        //Wheel test methods
        private void testWheel()
        {
            string testType = "fixedTest";
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay;
            speedByte = 1;
            delay = int.Parse(txtDelay.Text.ToString());
            topSpeed = (int)lamUtil.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lamUtil.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse(txtNumSteps.Text.ToString());
            wheel = txtTestMode.Text.ToString();
            if (wheel == "LB10-3 Batch") { controller = lambdaCom.getController(); }
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA();
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB();
                wheelCConfig = lambdaCom.getWheelC();
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }//escape
                //char sCheck = Convert.ToChar(s);//toString(s);
                delay = int.Parse(txtDelay.Text.ToString());
                delay = getDelay(s, delay);
                errors = myRandom(cycleCount, delay, s, testType);
                txtBoxDialog.AppendText(errors + " Errors at Speed " + s.ToString() + Environment.NewLine);
                //Branch for LB10-3 batch errors
                if (errors > 0 && wheel == "LB10-3 Batch")
                {
                    if (wheelAConfig != "N.A." && wheelAConfig != "WA-NC")
                    {
                        wheel = "Wheel A";
                        txtBoxDialog.AppendText("Errors on batch mode.  Testing wheel A. " + Environment.NewLine);
                        errors = myRandom(cycleCount, delay, s, testType);
                        txtBoxDialog.AppendText(wheel + "  " + errors + " Errors at Speed " + s.ToString() + Environment.NewLine);
                    }
                    if (wheelBConfig != "N.A." && wheelAConfig != "WB-NC")
                    {
                        wheel = "Wheel B";
                        txtBoxDialog.AppendText("Errors on batch mode.  Testing wheel B. " + Environment.NewLine);
                        errors = myRandom(cycleCount, delay, s, testType);
                        txtBoxDialog.AppendText(wheel + "  " + errors + " Errors at Speed " + s.ToString() + Environment.NewLine);
                    }
                    if (wheelCConfig != "N.A." && wheelAConfig != "WC-NC")
                    {
                        wheel = "Wheel C";
                        txtBoxDialog.AppendText("Errors on batch mode.  Testing wheel C. " + Environment.NewLine);
                        errors = myRandom(cycleCount, delay, s, testType);
                        txtBoxDialog.AppendText(wheel + "  " + errors + " Errors at Speed " + s.ToString() + Environment.NewLine);
                    }
                    wheel = "LB10-3 Batch";
                }
                if (errors > 0 && wheel == "LB10-2 Batch")
                {
                    wheel = "Wheel A";
                    txtBoxDialog.AppendText("Errors on batch mode.  Testing wheel A. " + Environment.NewLine);
                    errors = myRandom(cycleCount, delay, s, testType);
                    txtBoxDialog.AppendText(wheel + "  " + errors + " Errors at Speed " + s.ToString() + Environment.NewLine);
                    wheel = "Wheel B";
                    txtBoxDialog.AppendText("Errors on batch mode.  Testing wheel B. " + Environment.NewLine);
                    errors = myRandom(cycleCount, delay, s, testType);
                    txtBoxDialog.AppendText(wheel + "  " + errors + " Errors at Speed " + s.ToString() + Environment.NewLine);
                    wheel = "LB10-2 Batch";
                }
            }
        }
        private void randomTest()
        {
            string testType = "randomDelay";
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay;
            errors = 0;
            delay = 10;
            speedByte = 0;
            topSpeed = (int)lamUtil.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lamUtil.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = 100;
            wheel = txtTestMode.Text.ToString();
            controller = lambdaCom.getController();
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA();
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB();
                wheelCConfig = lambdaCom.getWheelC();
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }//escape
                //char sCheck = Convert.ToChar(s);//toString(s);
                do
                {
                    errors = myRandom(cycleCount, delay, s, testType);
                    if (errors > 0)
                    {
                        delay = delay + 10;
                    }
                } while (errors > 0 && delay < 200 || stopProcess == false);
                if (errors > 0)
                {
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " is not a good speed. " + Environment.NewLine);
                }
                else
                {
                    txtBoxDialog.AppendText("A goog speed is:" + Environment.NewLine);
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " at  delay " + delay.ToString() + Environment.NewLine);
                }
                delay = 10;
                errors = 0;
            }
        }

        private int myRandom(int cycleCount, int delay, int s, string testType)
        {
            byte loop = 1;
            int beginTime, endTime, totalTime;//used to calculate an error.
            int errors = 0;
            int oldErrors = 0;
            int counter = 0;
            int lastMove = 0;
            speedByte = (byte)lamUtil.getByte(s);
            for (int i = 0; i < cycleCount; i++)
            {
                do//You do NOT want to send the same command out twice  
                {//especially to the LB10-2
                    filterByte = (byte)random.Next(0, 9);
                } while (filterByte == lastMove);

                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                if (i == 0) { counter = 0; }
                moveMyWheel(wheel, moveByte);
                beginTime = Environment.TickCount;//returns time in ms
                loop = lambdaCom.readByte();
                while (loop != byteCom.byteCR && lambdaCom.isOpen())
                {
                    //lambdaCom.clearBuffer();
                    loop = lambdaCom.readByte();
                    if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")//to deal with LPT
                    {
                        if (loop == lambdaCom.readByte()) { loop = byteCom.byteCR; }
                    }
                    Application.DoEvents();
                }
                lambdaCom.clearBuffer();//You might read the CR twice if you do not do this!
                lambdaCom.clearBuffer();
                counter++;
                endTime = Environment.TickCount;
                totalTime = endTime - beginTime;
                errors = lamUtil.getErrors(controller, totalTime, s, errors);
                if (errors > oldErrors && testType == "fixedDelay")
                {
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " from " + lastMove.ToString());
                    txtBoxDialog.AppendText(" to " + filterByte.ToString() + Environment.NewLine);
                    oldErrors = errors;
                }
                if (errors > 0 && testType == "randomDelay")
                {
                    i = cycleCount;
                    //break;
                }
                txtMoves.Text = counter.ToString();
                lastMove = filterByte;
                Thread.Sleep(delay);//One way to add a delay
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
            }
            return errors;
        }

        private int myRandom(int cycleCount, int delay, int s, int filter1, int filter2, string testType)
        {
            byte loop = 1;
            int beginTime, endTime, totalTime;//used to calculate an error.
            int errors = 0;
            int oldErrors = 0;
            int counter = 0;
            int lastMove = 0;
            speedByte = (byte)lamUtil.getByte(s);
            for (int i = 1; i <= cycleCount; i++)
            {
                if ((i % 2) == 0)
                {
                    filterByte = (byte)filter1;
                }
                else
                {
                    filterByte = (byte)filter2;
                }
                moveByte = (byte)lamUtil.getMoveByte(speedByte, filterByte);
                if (i == 1) { counter = 0; }
                moveMyWheel(wheel, moveByte);
                beginTime = Environment.TickCount;//returns time in ms
                loop = lambdaCom.readByte();
                while (loop != byteCom.byteCR && lambdaCom.isOpen())
                {
                    loop = lambdaCom.readByte();
                    Application.DoEvents();
                }
                lambdaCom.clearBuffer();//You might read the CR twice if you do not do this!
                lambdaCom.clearBuffer();
                counter++;
                endTime = Environment.TickCount;
                totalTime = endTime - beginTime;
                errors = lamUtil.getErrors(controller, totalTime, s, errors);
                if (errors > oldErrors && testType == "fixedDelay")
                {
                    txtBoxDialog.AppendText(errors + " Errors at speed " + s.ToString() + " from " + lastMove.ToString());
                    txtBoxDialog.AppendText(" to " + filterByte.ToString() + Environment.NewLine);
                    oldErrors = errors;
                }
                if (errors > 0 && testType == "randomDelay")
                {
                    i = cycleCount;
                    break;
                }
                txtMoves.Text = counter.ToString();
                lastMove = filterByte;
                Thread.Sleep(delay);//One way to add a delay
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
            }
            return errors;
        }
        private void moveMyWheel(string wheel, byte moveByte)
        {
            switch (wheel)
            {
                case "Wheel A":
                    lambdaCom.moveWheelA(moveByte);
                    break;
                case "Wheel B":
                    lambdaCom.moveWheelB(moveByte);
                    break;
                case "Wheel C":
                    lambdaCom.moveWheelC(moveByte);
                    break;
                case "LB10-3 Batch":
                    lambdaCom.moveBatch(moveByte, moveByte, moveByte);
                    break;
                case "LB10-2 Batch":
                    lambdaCom.moveBatch2(moveByte, byteCom.byteCloseA, moveByte, byteCom.byteCloseB);//Close shutter by default
                    break;
            }
        }
        private int getDelay(int s, int delay)
        {
            //txtBoxDialog.AppendText("Delay Method" + delay + Environment.NewLine);
            switch (s)
            {
                case 0:
                    if (chkBoxAddDel0.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 1:
                    if (chkBoxAddDel1.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 2:
                    if (chkBoxAddDel2.Checked == true)
                    {
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 3:
                    if (chkBoxAddDel3.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 4:
                    if (chkBoxAddDel4.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 5:
                    if (chkBoxAddDel5.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 6:
                    if (chkBoxAddDel6.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
                case 7:
                    if (chkBoxAddDel7.Checked == true) { 
                        delay = delay * (int)DecDelayMultiplier.Value;
                        txtBoxDialog.AppendText("Delay Added. New Delay is: " + delay + "Ms" + Environment.NewLine);
                    }
                    break;
            }
            return delay;
        }

        //******************************************************************************************************
        //LB-SC pannel

        private void txtTrigger_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Sets the TTL trigger mode." + Environment.NewLine);
            txtHelp.AppendText("Default: open on TTL high." + Environment.NewLine);
            txtHelp.AppendText("Toggle mode triggers on the rising edge." + Environment.NewLine);
        }
        private void btnSetTTL_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Save defaults tyo the LB-SC controller." + Environment.NewLine);
        }
        private void txtSync_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Detemines the TTL Sync mode." + Environment.NewLine);
            txtHelp.AppendText("Default: TTL high on shutter open." + Environment.NewLine);
        }
        private void setTTL_Click(object sender, EventArgs e)
        {
            string ttlIn, ttlOut;
            ttlIn = txtTrigger.Text;
            ttlOut = txtSync.Text;
            switch (ttlIn)
            {
                case "TTL High":
                    lambdaCom.setTTLHigh();
                    break;
                case "TTL Low":
                    lambdaCom.setTTLLow();
                    break;
                case "Rising Edge":
                    lambdaCom.setTTLToggleRisingEdge();
                    break;
                case "Falling Edge":
                    lambdaCom.setTTLToggleFallingEdge();
                    break;
                case "Disabled":
                    lambdaCom.setTTLDisabled();
                    break;
            }
            switch (ttlOut)
            {
                case "High on Open":
                    lambdaCom.setSyncHighOpen();
                    break;
                case "Low on Open":
                    lambdaCom.setSyncLowOpen();
                    break;
                case "Disabled":
                    lambdaCom.setSyncDisabled();
                    break;
            }
            return;
        }
        //The LB-SC has a unique protocal.  The minutes, seconds,mili-Seconds and micro-seconds
        //are all encoded separatly.
        private void decMin_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Set the minuts." + Environment.NewLine);
        }
        private void decSec_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Set the seconds." + Environment.NewLine);
        }
        private void decMs_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Set the mili-seconds." + Environment.NewLine);
        }
        private void decUs_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Set the micro-seconds." + Environment.NewLine);
        }
        private void btnExposure_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Uses the timer values to set the exposure time." + Environment.NewLine);
        }
        private void exposureButton_Click(object sender, EventArgs e)
        {
            uint usTime = (uint)decUs.Value;
            uint msTime = (uint)decMs.Value;
            uint secTime = (uint)decSec.Value;
            uint minTime = (uint)decMin.Value;
            lambdaCom.setExposureTimer(minTime, secTime, msTime, usTime);
            lambdaCom.setSingleShot();
            decUs.Value = 0;
            decMs.Value = 0;
            decSec.Value = 0;
            decMin.Value = 0;
        }
        private void btnDelay_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Uses the timer values to set the delay time." + Environment.NewLine);
        }
        private void delayButton_Click(object sender, EventArgs e)
        {
            uint usTime = (uint)decUs.Value;
            uint msTime = (uint)decMs.Value;
            uint secTime = (uint)decSec.Value;
            uint minTime = (uint)decMin.Value;
            lambdaCom.setDelayTimer(minTime, secTime, msTime, usTime);
            lambdaCom.setSingleShot();
            decUs.Value = 0;
            decMs.Value = 0;
            decSec.Value = 0;
            decMin.Value = 0;
        }
        private void btnRun_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Start free run." + Environment.NewLine);
        }
        private void decCycles_MouseClick(object sender, MouseEventArgs e)
        {
            txtHelp.AppendText("Sets the free run Cycles." + Environment.NewLine);
            txtHelp.AppendText("Sets the value to 1 to use the dealy on the first move." + Environment.NewLine);
        }
        private void btnRestoreSC_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Restore factory default to LB-SC." + Environment.NewLine);
        }
        private void restoreButton_Click(object sender, EventArgs e)
        {
            lambdaCom.stopFreeRun();
            lambdaCom.restoreDefaults();
        }
        private void btnResetSC_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Reset LB-SC to last configuration." + Environment.NewLine);
            txtHelp.AppendText("Not Implemented." + Environment.NewLine);
        }
        private void btnResetSC_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void btnStopAutoRun_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Stop free run." + Environment.NewLine);
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            lambdaCom.stopFreeRun();
        }

        //****************************************************************************************************** 
        //dialog box methods
        private void getPortsButton_Click(object sender, EventArgs e)
        {
            string writePorts = lambdaCom.getPorts();
            txtBoxDialog.AppendText("The following ports are avaialble on your PC:" + "\r\n");
            txtBoxDialog.AppendText(writePorts);
        }

        private void statusButton_Click(object sender, EventArgs e)
        {
            writeConfig();
            txtBoxDialog.AppendText("End" + Environment.NewLine);
        }

        private void writeConfig()
        {
            string controller, wheelA, wheelB, wheelC, shutterA, shutterB, shutterC, status, controllerString;
            status = lambdaCom.getConfig();
            controller = lambdaCom.getController();
            // for test
            //dialogBox.AppendText("STATUS" + status + Environment.NewLine);
            switch (controller)
            {
                case "10-3":
                    wheelA = lambdaCom.getWheelA();
                    wheelB = lambdaCom.getWheelB();
                    wheelC = lambdaCom.getWheelC();
                    shutterA = lambdaCom.getShutterA();
                    shutterB = lambdaCom.getShutterB();//Note case statments change the scope of the dialog box!
                    shutterC = lambdaCom.getShutterC();
                    controllerString = lamUtil.getConfigString(wheelA, wheelB, wheelC, shutterA, shutterB, shutterC);
                    //dialogBox.AppendText("Lambda 10-3" + Environment.NewLine);
                    txtBoxDialog.AppendText(controllerString);
                    break;
                case "10-B":
                    wheelA = lambdaCom.getWheelA();
                    shutterA = lambdaCom.getShutterA();
                    shutterB = lambdaCom.getShutterB();
                    controllerString = lamUtil.getConfigString(wheelA, shutterA, shutterB);
                    //dialogBox.AppendText("Lambda 10-B" + Environment.NewLine);
                    txtBoxDialog.AppendText(controllerString);
                    break;
                case "LB10-2":
                    //dialogBox.AppendText("Lambda 10-2" + Environment.NewLine);
                    controllerString = lamUtil.getConfigString();
                    txtBoxDialog.AppendText(controllerString);
                    break;
                case "SC":
                    shutterA = lambdaCom.getShutterA();
                    //dialogBox.AppendText("Lambda SC" + Environment.NewLine);
                    controllerString = lamUtil.getConfigString(shutterA);
                    txtBoxDialog.AppendText(controllerString);
                    break;
            }
            lambdaCom.clearBuffer();
            return;
        }

        //*********************************************************************************************************
        //DG-4 panel


        private void radioTurboOn_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Turns on turbo-blanking." + Environment.NewLine);
            txtHelp.AppendText("Turbo-blanking prevents light leakage for non adjacent filter moves.." + Environment.NewLine);
        }
        private void radioTurboOn_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.writeByte(byteCom.byteOpenB);
        }
        private void radioTurboOff_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Turns on turbo-blanking." + Environment.NewLine);
            txtHelp.AppendText("Turbo-blanking prevents light leakage for non adjacent filter moves.." + Environment.NewLine);
        }
        private void radioTurboOff_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.writeByte(byteCom.byteCloseB);
        }
        private void shutterOpenButton_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Opens the shutter / move to possition." + Environment.NewLine);
        }
        private void shutterOpenButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && shutterOpenButton.Checked)
            {
                lambdaCom.openA();
                txtBoxDialog.AppendText(" Shutter: Opened " + Environment.NewLine);
            }
            return;
        }
        private void shutterCloseButton_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Close the shutter / move to possition 0." + Environment.NewLine);
        }
        private void shutterCloseButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && shutterCloseButton.Checked)
            {
                lambdaCom.closeShutterA();
                txtBoxDialog.AppendText(" Shutter: Closed " + Environment.NewLine);
            }
            return;
        }

        private void radioMove1_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 1." + Environment.NewLine);
        }
        private void move1RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove1.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(1);
                    txtBoxDialog.AppendText(" Moved Possition 1 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(17);
                }
            }
            return;
        }

        private void radioMove2_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 2." + Environment.NewLine);
        }
        private void move2RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove2.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(2);
                    txtBoxDialog.AppendText(" Posstion 2 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(18);
                }
            }
            return;
        }
        private void radioMove3_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 3." + Environment.NewLine);
        }
        private void move3RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove3.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(3);
                    txtBoxDialog.AppendText(" Posstion 3 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(19);
                }
            }
            return;
        }
        private void radioMove4_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 4." + Environment.NewLine);
        }
        private void move4RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove4.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(4);
                    txtBoxDialog.AppendText(" Posstion 4 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(20);
                }
            }
            return;
        }
        private void radioMove5_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 5 on the DG-5." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 1 66% power." + Environment.NewLine);
        }
        private void move5RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove5.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(5);
                    txtBoxDialog.AppendText(" Posstion 5 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(21);
                }
            }
            return;
        }
        private void radioMove6_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 6." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 2 66% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 1 66% power." + Environment.NewLine);
        }
        private void move6RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove6.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(6);
                    txtBoxDialog.AppendText(" Posstion 6 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(22);
                }
            }
            return;
        }
        private void radioMove7_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 7." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 3 66% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 2 66% power." + Environment.NewLine);
        }
        private void move7RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove7.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(7);
                    txtBoxDialog.AppendText(" Posstion 7 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(23);
                }
            }
            return;
        }
        private void radioMove8_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 8." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 4 66% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 3 66% power." + Environment.NewLine);
        }
        private void move8RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove8.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(8);
                    txtBoxDialog.AppendText(" Posstion 8 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(24);
                }
            }
            return;
        }
        private void radioMove9_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 9." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 1 33% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 4 66% power." + Environment.NewLine);
        }
        private void move9RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove9.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(9);
                    txtBoxDialog.AppendText(" Posstion 9 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(25);
                }
            }
            return;
        }
        private void radioMove10_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 10." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 2 33% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 5 66% power." + Environment.NewLine);
        }
        private void move10RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove10.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(10);
                    txtBoxDialog.AppendText(" Posstion 10 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(26);
                }
            }
            return;
        }
        private void radioMove11_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 11." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 3 33% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 1 33% power." + Environment.NewLine);
        }
        private void move11RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove11.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(11);
                    txtBoxDialog.AppendText(" Posstion 11 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(27);
                }
            }
            return;
        }
        private void radioMove12_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 11." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 4 33% power." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 2 33% power." + Environment.NewLine);
        }
        private void move12RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove12.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(12);
                    txtBoxDialog.AppendText(" Posstion 12 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(28);
                }
            }
            return;
        }
        private void radioMove13_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 12." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 0 / shutter." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 3 33% power." + Environment.NewLine);
        }
        private void move13RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove13.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(13);
                    txtBoxDialog.AppendText(" Posstion 13 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(29);
                }
            }
            return;
        }
        private void radioMove14_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 12." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 0 / shutter." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 4 33% power." + Environment.NewLine);
        }
        private void move14RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove14.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(14);
                    txtBoxDialog.AppendText(" Posstion 14 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(30);
                }
            }
            return;
        }
        private void radioMove15_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Move to possition 12." + Environment.NewLine);
            txtHelp.AppendText("DG-4: equals possition 0 / shutter." + Environment.NewLine);
            txtHelp.AppendText("DG-5: equals possition 5 33% power." + Environment.NewLine);
        }
        private void move15RadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioMove15.Checked)
            {
                if (chkTriggerd.Checked == false)
                {
                    lambdaCom.writeByte(15);
                    txtBoxDialog.AppendText(" Posstion 15 " + Environment.NewLine);
                }
                else
                {
                    lambdaCom.writeByte(31);
                }
            }
            return;
        }

        //***************************************************************************************
        //Ring buffer methods

        private void radioStrobeOff_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Deactivete TTL / strobe." + Environment.NewLine);
        }
        private void ttlOffButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioStrobeOff.Checked)
            {
                lambdaCom.writeByte(203);
                txtBoxDialog.AppendText("TTL's Disabled " + Environment.NewLine);
            }
            return;
        }

        private void radioStrobeOn_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Activete TTL / strobe." + Environment.NewLine);
            txtHelp.AppendText("Must be activly driven." + Environment.NewLine);
        }

        private void strobeButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lambdaCom.isOpen() && radioStrobeOn.Checked)
            {
                lambdaCom.writeByte(202);
                txtBoxDialog.AppendText("Triggered by Strobe Pulse " + Environment.NewLine);
            }
            return;
        }

        private void decFilterNum_Enter_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Specifies the filter number in the sequence." + Environment.NewLine);
        }
        private void decSeqNum_Enter_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Specifies the sequence number." + Environment.NewLine);
        }
        private void btnAddVal_MouseHover_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Adds the filter number at the sequence number." + Environment.NewLine);
        }
        private void btnAddValue_MouseClick(object sender, MouseEventArgs e)
        {
            string filter = decFilterNum.Value.ToString();
            int sequence = (int)decSeqNum.Value;
            filterValue[sequence] = filter;
            decSeqNum.Value++;
            if (filterValue[sequence + 1] != "eof" && filterValue[sequence + 1] != null)
            {
                decFilterNum.Value = decimal.Parse(filterValue[sequence + 1]);
            }
            txtBoxDialog.AppendText("Current value" + filterValue[sequence + 1] + Environment.NewLine);
            txtBoxDialog.AppendText("Ring buffer filter# " + filter + " in sequence " + sequence + Environment.NewLine);
        }
        private void btnGetFile_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Gets a specific file and stores it in memory." + Environment.NewLine);
        }
        private void btnGetFile_MouseClick(object sender, MouseEventArgs e)
        {
            string file = txtFileName.Text;
            getFile(file);
        }
        private void btnSaveFile_MouseClick(object sender, MouseEventArgs e)
        {
            string file = txtFileName.Text;
            saveFile(file);
            txtBoxDialog.AppendText(file + ".txt Saved" + Environment.NewLine);
        }

        private void btnLoadBuffer_MouseHover_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Loads the file in memory into the DG-4." + Environment.NewLine);
        }

        private void btnClearRingBuffer_MouseClick(object sender, MouseEventArgs e)
        {
            decSeqNum.Value = 0;
            decFilterNum.Value = 0;
        }
        private void btnClearRingBuffer_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Resets filter number and sequence number to 0." + Environment.NewLine);
        }
        private void btnLoadBuffer_MouseClick(object sender, MouseEventArgs e)
        {
            int i = 0;
            lambdaCom.writeByte(byteCom.byteLB102Batch);//Start loading buffer
            if (filterValue[i] == "eof") { lambdaCom.writeByte(0); }
            while (filterValue[i] != "eof")
            {
                lambdaCom.writeByte(byte.Parse(filterValue[i]));//Load value
                i++;
            }
            lambdaCom.writeByte(byteCom.byteSetCycles);//End loading buffer
        }
        private void btnRingEnable_MouseHover_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Sets the DG-4 into ring buffer mode." + Environment.NewLine);
        }
        private void btnRingEnable_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.writeByte(byteCom.byteSetFreeRunPowerOn);
            lambdaCom.writeByte(202);
        }
        
        private void btnRingDisable_MouseHover_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Disables the ring buffer mode." + Environment.NewLine);
        }
        private void btnRingDisable_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.writeByte(byteCom.byteSetFreeRunTTLIn);
            lambdaCom.writeByte(203);
        }
        //******************************************************************************************
        //      File methods 
        //need load buffer methods.
        //need to initialize buffer file
        //**********************************************************************************
        private void txtFileName_MouseHover_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("This name is used to both retreive saved files as well as to save the file.." + Environment.NewLine);
        }
        public void getFile(string fileName)
        {// create reader & open file
            tr = new StreamReader(fileName + ".txt");
            for (int i = 0; i <= 49; i++)
            {
                if (filterValue[i] == "eof" || filterValue[i] == null)
                {
                    filterValue[i] = "eof";
                }
                else
                {
                    filterValue[i] = i.ToString();// tr.ReadLine();
                }
            }
            decSeqNum.Value = 0;
            tr.Close();
        }
        private void btnSaveFile_MouseHover_1(object sender, EventArgs e)
        {
            txtHelp.AppendText("Saves the file using the name in the text box." + Environment.NewLine);
        }

        public void saveFile(string fileName)
        {// create reader & open file
            int fileLength = (int.Parse(decSeqNum.Value.ToString()) - 1);
            tw = new StreamWriter(fileName + ".txt");
            for (int i = 0; i <= fileLength; i++)
            {
                //dialogBox.AppendText("i" + Environment.NewLine);
                tw.WriteLine(filterValue[i]);
            }
            decSeqNum.Value = 0;
            decFilterNum.Value = decimal.Parse(filterValue[0]);
            tw.Close();
        }

        private void txtMoves_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("This is used to track the number of moves once in test mode." + Environment.NewLine);
        }

        private void btnClearTxt_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Clears the text in the text box.." + Environment.NewLine);
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            txtBoxDialog.Text = "";
        }
        private void btnStop_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Stops all running tests." + Environment.NewLine);
        }
        private void btnStop_MouseClick(object sender, MouseEventArgs e)
        {
            stopProcess = true;
        }
        private void btnGetStatus_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("returns the configuration information of the controller." + Environment.NewLine);
        }
        private void btnGetPorts_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("returns the com ports available for use." + Environment.NewLine);
        }

        private void readPort(){
            string myMode = txtComPort.Text.ToString();
            byte loop = lambdaCom.readByte();
            while (loop != byteCom.byteCR && myMode != "LPT1" && myMode != "LPT2")
            {
                if (loop != 0 && lambdaCom.isOpen()) { txtCom.AppendText("R-" + loop + Environment.NewLine); }
                loop = lambdaCom.readByte();
                Application.DoEvents();
            }
        }

        /* This function is not reliable.  Most likly on the DG-4 side?
        private void btnAdjust_MouseClick(object sender, MouseEventArgs e)
        {
            byte filter = (byte)decFilterToAdjust.Value;
            int steps = (int)decAdjustSteps.Value;
            lambdaCom.writeByte(filter);
            txtBoxDialog.AppendText(" Posstion " + filter + Environment.NewLine);
            Thread.Sleep(10);//One way to add a delay
            lambdaCom.writeByte(234);//Start nuetral desity adjustment
            Thread.Sleep(10);//One way to add a delay
            for (int i = 0; i < steps; i++)
            {
                if (radioInc.Checked)
                { //Increment by one}
                    lambdaCom.writeByte(byteCom.byteOpenC);
                }
                else
                { //Decrament by one}
                    lambdaCom.writeByte(236);
                }
                Thread.Sleep(10);//One way to add a delay
                lambdaCom.writeByte(byteCom.byteOpenA);
            }
            Thread.Sleep(10);//One way to add a delay
            lambdaCom.writeByte(byteCom.byteCloseC);//End nuetral desity adjustment
            if (radioInc.Checked)
            { //Increment by one}
                txtBoxDialog.AppendText(" Posstion " + filter + " incremented by: " + steps +Environment.NewLine);
            }
            else
            { //Decrament by one}
                txtBoxDialog.AppendText(" Posstion " + filter + " decremented by: " + steps + Environment.NewLine);
            }
            lambdaCom.writeByte(219);
        }*/
    }
}
