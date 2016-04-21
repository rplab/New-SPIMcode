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
 * 1-Add get home possition for LB-SC.
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
        //TextBox txtCom = new TextBox();
        lambdaSerial lambdaCom;
        //lambdaUtils lamUtil;
        byteCommands byteCom;
        Random random;
        //Stream needed for ring buffer
        StreamReader tr;
        StreamWriter tw;
        string[] filterValue = new string[50];
        //end stream stuff
        byte speedByte = 1;//0 is a bad speed for most moves
        byte filterByte = 0;
        byte moveByte = 0;
        string [] deviceList = new string[128];
        string wheel, controller, wheelAConfig, wheelBConfig, wheelCConfig, LB10B_SB;
        bool stopProcess = false;

        public Mainform()
        {
            InitializeComponent();
            lambdaCom = new lambdaSerial(this);
            //lamUtil = new lambdaUtils();
            byteCom = new byteCommands();
            random = new Random();
            uint ui=0;
            do
            {
                try
                {
                    if (lambdaCom.GetSerialNum(ui) != null)
                    {
                        txtUSBList.Items.Add((lambdaCom.GetSerialNum(ui) + ": " + lambdaCom.GetDescription(ui)));
                        deviceList[ui] = lambdaCom.GetSerialNum(ui);
                    }
                }
                catch
                {

                }
                ui++;
            } while (lambdaCom.GetSerialNum((ui - 1)) != "No Device" && ui < 129);
            try
            {
                if (lambdaCom.GetSerialNum(0) != null)
                {
                    txtUSBList.SetSelected(0, true);
                }
            }
            catch
            {

            }
            for (int i = 0; i <= 49; i++)
            {
                filterValue[i] = "eof";
            }
            //lambdaCom.openCom();
        }
        ~Mainform()
        {
            lambdaCom.clearBuffer();
            lambdaCom.closeCom();
        }
        public void UpdateTxtCom(string text)
        {
            this.txtCom.AppendText(text);
        }
        private void Mainform_Load(object sender, EventArgs e)
        {
            //txtCom.AppendText("devCount" + ftdiDeviceCount.ToString() + Environment.NewLine);
        }
        /*private void txtComPort_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("You can add custom values here if necessary." + Environment.NewLine);
        }*/
        private void btnOpenCom_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Opens the com port and reports the status of the unit." + Environment.NewLine);
        }
        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            //UInt32 SelectedIndex = 1;
            string myMode = txtComPort.Text.ToString();
            int usbMode = txtOpenBy.SelectedIndex;
            uint usbBuad = uint.Parse( txtBaudSelect.Text.ToString());
            int rs232Buad = int.Parse(txtBaudSelect.Text.ToString());

            switch (myMode)//if (myMode == "USB")
            {
                case "USB":
                    switch (usbMode)
                    {
                        case 0:
                            lambdaCom.OpenBySerialNum(lambdaCom.GetSerialNum());
                            txtHelp.AppendText("Case 0." + Environment.NewLine);
                            break;
                        case 1:
                            lambdaCom.openByIndex(0);
                            txtHelp.AppendText("Case 1." + Environment.NewLine);
                            break;
                        /*case 2:
                            lambdaCom.OpenByDescription(lambdaCom.GetDescription());
                            break; Does not work properly
                         * */
                        default:
                            lambdaCom.openCom(myMode);
                            txtHelp.AppendText("Case D." + Environment.NewLine);
                            //MessageBox.Show("Default" + usbMode); 
                            break;
                    }
                    lambdaCom.SetUSBBaudrate(usbBuad);
                    //MessageBox.Show("Baud set" + usbBuad.ToString()); 
                    break;
                case "LPT1":
                    lambdaCom.openCom(myMode);
                    lambdaCom.writeByte(238);
                    break;
                case "LPT2":
                    lambdaCom.openCom(myMode);
                    lambdaCom.writeByte(238);
                    break;
                default:
                    lambdaCom.openCom(myMode);
                    lambdaCom.SetRS232Baudrate(rs232Buad); 
                    break;
            } 
            byte[] txbuf = new byte[25];
            byte[] rxbuf= new byte[25];
            lambdaCom.clearBuffer();
            lambdaCom.writeByte(byteCom.byteGoOnline);
            //if (myMode!="USB"){ readPort();}//messes up usb!
            controller= lambdaCom.getController();
            //txtCom.AppendText("R-" + controller + Environment.NewLine);
            if (lambdaCom.isOpen() || controller == "LB10-2")
            {
                txtBoxDialog.AppendText(txtComPort.Text.ToString() + " :  is Open" + Environment.NewLine);
            }
            else
            {
                txtBoxDialog.AppendText("Could not open: " + txtComPort.Text.ToString() + Environment.NewLine);
            }
            lambdaCom.clearBuffer();
            txtBoxDialog.AppendText("Config String: " + lambdaCom.getConfig() + Environment.NewLine);
            //writeConfig();
            controller = lambdaCom.getController();
            LB10B_SB = lambdaCom.getConfig();
            try
            {
                LB10B_SB = LB10B_SB.Substring(13, 2);
            }
            catch { LB10B_SB = "NA"; }
            lambdaCom.clearBuffer();
            txtBoxDialog.AppendText("End" + Environment.NewLine);
            //txtBoxDialog.AppendText("LB10B_SB: " + LB10B_SB + Environment.NewLine);
        }
        private void btnClose_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Closes the com port and clears dialog box." + Environment.NewLine);
        }
        private void txtComPort_ValueMemberChanged(object sender, EventArgs e)
        {

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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter0.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte, filterByte);
                //txtCom.AppendText("W-" + moveByte+ Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter1.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter2.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                //txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter3.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter4.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter5.Text.ToString());;
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter6.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter7.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter8.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
                speedByte = (byte)(lambdaCom.getSpeedByte(txtSpeedBox.Text.ToString()));
                filterByte = lambdaCom.getFilterByte(radioFilter9.Text.ToString());
                wheel = txtWheelBox.Text.ToString();
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                //txtCom.AppendText("W-" + moveByte + Environment.NewLine);
                readPort();
                txtBoxDialog.AppendText(wheel + " Moved to: " + filterByte.ToString() + Environment.NewLine);
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
            byte returnString = lambdaCom.readByte();
            //txtCom.AppendText("R1-" + returnString + " : " + (char)returnString + Environment.NewLine);
            returnString = lambdaCom.readByte();
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
            //lambdaCom.writeByte(byteCom.bytePowerOn);
            //Thread.Sleep(2);
            if (lambdaCom.isOpen() && radioOpenA.Checked)
            {
                string shutterMode = txtModeA.Text.ToString();
                byte ndSetting = (byte)decNDA.Value;
                switch (shutterMode)
                {
                    case "Fast":
                        lambdaCom.lbX.openA(controller);
                        readPort();
                        break;
                    case "Soft":
                        lambdaCom.lbX.openASoft(controller);
                        //txtCom.AppendText("W-Open A soft" + Environment.NewLine);
                        readPort();
                        break;
                    case "ND Mode":
                        lambdaCom.lbX.openA(controller,ndSetting);
                        //txtCom.AppendText("W-Open A ND" + ndSetting + Environment.NewLine);
                        readPort();
                        break;
                    case "Defualt Mode":
                        lambdaCom.lbX.openAdefault(controller);
                        //txtCom.AppendText("W-Open A Default" + ndSetting + Environment.NewLine);
                        readPort();
                        break;
                    default:
                        lambdaCom.lbX.openAdefault(controller);
                        //txtCom.AppendText("W-Open A Default" + Environment.NewLine);
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
            //lambdaCom.writeByte(byteCom.bytePowerOn);
            //Thread.Sleep(2);
            if (lambdaCom.isOpen() && radioCloseA.Checked)
            {
                lambdaCom.lbX.closeShutterA();
                //txtCom.AppendText("W-Close Shutter A"  + Environment.NewLine);
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
                        lambdaCom.lbX.openB();
                        //txtCom.AppendText("W-Open B fast" + Environment.NewLine);
                        readPort();
                        break;
                    case "Soft":
                        lambdaCom.lbX.openBSoft();
                        //txtCom.AppendText("W-Open B soft" + Environment.NewLine);
                        readPort();
                        break;
                    case "ND Mode":
                        lambdaCom.lbX.openB(ndSetting);
                        //txtCom.AppendText("W-Open B ND" + ndSetting + Environment.NewLine);
                        readPort();
                        break;
                    case "Defualt Mode":
                        lambdaCom.lbX.openBdefault();
                        //txtCom.AppendText("W-Open B Default" + Environment.NewLine);
                        readPort();
                        break;
                    default:
                        lambdaCom.lbX.openBdefault();
                        //txtCom.AppendText("W-Open B Default" + Environment.NewLine);
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
                lambdaCom.lbX.closeShutterB();
                //txtCom.AppendText("W-Close Shutter B" + Environment.NewLine);
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
                        lambdaCom.lbX.openC();
                        //txtCom.AppendText("W-Open C Fast" + Environment.NewLine);
                        readPort();
                        break;
                    case "Soft":
                        lambdaCom.lbX.openCSoft();
                        //txtCom.AppendText("W-Open C Soft" + Environment.NewLine);
                        readPort();
                        break;
                    case "ND Mode":
                        lambdaCom.lbX.openC(ndSetting);
                        //txtCom.AppendText("W-Open C ND" + ndSetting  + Environment.NewLine);
                        readPort();
                        break;
                    case "Defualt Mode":
                        lambdaCom.lbX.openCdefault();
                        //txtCom.AppendText("W-Open C Default"  + Environment.NewLine);
                        readPort();
                        break;
                    default:
                        lambdaCom.lbX.openCdefault();
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
                lambdaCom.lbX.closeShutterC();
                //txtCom.AppendText("W-Close Shutter C" + Environment.NewLine);
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
        private void btnShutterTest_MouseClick(object sender, MouseEventArgs e)
        {

            stopProcess = false;
            string shutter = txtShutterSelect.Text.ToString();
            int hertz = (int)decHertz.Value;
            hertz = lambdaCom.getHertz(hertz);
            do
            {
                switch (shutter)
                {
                    case "A":
                        lambdaCom.lbX.openA();
                        //txtCom.AppendText("W-Open A" + Environment.NewLine);
                        readPort();
                        Thread.Sleep(hertz);
                        lambdaCom.lbX.closeShutterA();
                        //txtCom.AppendText("W-Open A" + Environment.NewLine);
                        readPort();
                        break;
                    case "B":
                        lambdaCom.lbX.openB();
                        Thread.Sleep(hertz);
                        lambdaCom.lbX.closeShutterB();
                        break;
                    case "C":
                        lambdaCom.lbX.openC();
                        Thread.Sleep(hertz);
                        lambdaCom.lbX.closeShutterC();
                        break;
                    case "A_&_B":
                        lambdaCom.writeByte(189);
                        lambdaCom.lbX.openAdefault();
                        lambdaCom.lbX.openBdefault();
                        lambdaCom.writeByte(190);
                        Thread.Sleep(hertz);
                        lambdaCom.writeByte(189);
                        lambdaCom.lbX.closeShutterA();
                        lambdaCom.lbX.closeShutterB();
                        lambdaCom.writeByte(190);
                        break;
                }
                Thread.Sleep(hertz);
                Application.DoEvents();
            } while (lambdaCom.isOpen() && stopProcess == false);
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

        }
        private void btnTestWheel_MouseClick(object sender, MouseEventArgs e)
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
            int errors, topSpeed, lastSpeed, cycleCount, delay, filter1, filter2;
            errors = 0;
            delay = int.Parse(txtDelay.Text.ToString());
            speedByte = 0;
            topSpeed = (int)lambdaCom.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lambdaCom.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse( txtNumSteps.Text.ToString());
            filter1 = (int)filter1UpDown.Value;
            filter2 = (int)filter2UpDown.Value;
            wheel = txtTestMode.Text.ToString();
            controller = lambdaCom.getController();
            string status = lambdaCom.getConfig();
            if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")
            {
                txtBoxDialog.AppendText("The error detection does not work on the parallel port!" + Environment.NewLine);
            }
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA(status);
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB(status);
                wheelCConfig = lambdaCom.getWheelC(status);
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                if (lambdaCom.isOpen() == false || stopProcess == true) { return; }//escape
                    errors = myRandom(cycleCount, delay, s, filter1, filter2, "fixedDelay");
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
            }
            stopProcess = false;
        }
        private void HS4TestButton_Click(object sender, EventArgs e)
        {

        }
        //************************************************************************************************
        //Wheel test methods
        private void testWheel()
        {
            string testType = "fixedTest";
            string status = lambdaCom.getConfig();
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay;
            speedByte = 1;
            delay = int.Parse(txtDelay.Text.ToString());
            topSpeed = (int)lambdaCom.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lambdaCom.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse(txtNumSteps.Text.ToString());
            wheel = txtTestMode.Text.ToString();
            if (wheel == "LB10-3 Batch") { controller = lambdaCom.getController(); }
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA(status);
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB(status);
                wheelCConfig = lambdaCom.getWheelC(status);
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
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
            stopProcess = false;
            //Exit = false;
        }
        private void randomTest()
        {
            string testType = "randomDelay";
            string status = lambdaCom.getConfig();
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay;
            errors = 0;
            delay = 10;
            speedByte = 0;
            topSpeed = (int)lambdaCom.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lambdaCom.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse(txtNumSteps.Text.ToString());
            wheel = txtTestMode.Text.ToString();
            controller = lambdaCom.getController();
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA(status);
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB(status);
                wheelCConfig = lambdaCom.getWheelC(status);
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
                do
                {
                    errors = myRandom(cycleCount, delay, s, testType);
                    if (errors > 0)
                    {
                        delay = delay + 10;
                    }
                } while (errors > 0 && delay < 200 && stopProcess == false);
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
            stopProcess = false;
        }

        private int myRandom(int cycleCount, int delay, int s, string testType)
        {
            int totalTime;//used to calculate an error.
            int errors = 0;
            int oldErrors = 0;
            int counter = 0;
            int lastMove = 0;
            speedByte = (byte)lambdaCom.getByte(s);
            for (int i = 0; i < cycleCount; i++)
            {
                do//You do NOT want to send the same command out twice  
                {//especially to the LB10-2
                    filterByte = (byte)random.Next(0, 9);
                } while (filterByte == lastMove);

                moveByte = (byte)lambdaCom.getMoveByte(speedByte, filterByte);
                if (i == 0) { counter = 0; }
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                totalTime = getTime();
                errors = lambdaCom.getErrors(controller, totalTime, s, errors);
                //txtBoxDialog.AppendText("Errors: " + errors + Environment.NewLine);
                counter++;
                //txtBoxDialog.AppendText(lastMove.ToString() + " to " + filterByte.ToString() + " at " + totalTime + "ms " + Environment.NewLine);
                if (errors > oldErrors)
                {
                    txtBoxDialog.AppendText("Error at Speed " + s.ToString() + " from " + lastMove.ToString());
                    txtBoxDialog.AppendText(" to " + filterByte.ToString() + " at " + totalTime + "ms " + Environment.NewLine);
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
                if (txtComPort.Text=="LPT1" || txtComPort.Text=="LPT2") {Thread.Sleep(delay*s); }
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
            }
            return errors;
        }

        private int myRandom(int cycleCount, int delay, int s, int filter1, int filter2, string testType)
        {
            int totalTime;//used to calculate an error.
            int errors = 0;
            int oldErrors = 0;
            int counter = 0;
            int lastMove = 0;
            byte f1=(byte)filter1;
            byte f2=  (byte)filter2;
            filterByte = f1;
            speedByte = (byte)lambdaCom.getByte(s);
            for (int i = 1; i <= cycleCount; i++)
            {
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
                if ((i % 2) == 0 && testType!="seqRandom")
                {
                    filterByte = f1;
                }
                if ((i % 2) != 0 && testType != "seqRandom")
                {
                    filterByte = f2;
                }
                if (testType == "seqRandom")
                {
                    if (filterByte < f2 && (filterByte+1)>= f1) { filterByte++; }
                    else { filterByte = f1; }
                }
                moveByte = (byte)lambdaCom.getMoveByte(speedByte, filterByte);
                if (i == 1) { counter = 0; }
                lambdaCom.lbX.moveMyWheel(wheel, speedByte,filterByte);
                totalTime = getTime();
                errors = lambdaCom.getErrors(controller, totalTime, s, errors);
                txtBoxDialog.AppendText("errors: "  + errors + Environment.NewLine);
                if (errors > oldErrors && testType == "fixedDelay" )
                {
                    txtBoxDialog.AppendText(errors + " Errors at speed " + s.ToString() + " from " + lastMove.ToString());
                    txtBoxDialog.AppendText(" to " + filterByte.ToString() + Environment.NewLine);
                    oldErrors = errors;
                }
                if (errors > 0 && (testType == "randomDelay" ||testType == "seqRandom"))
                {
                    i = cycleCount;
                }
                counter = i;
                txtMoves.Text = counter.ToString();
                lastMove = filterByte;
                Thread.Sleep(delay);//One way to add a delay
            }
            return errors;
        }
        
        private int getTime()
        {
            int beginTime, endTime, totalTime;
            byte loop = 1;
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
            endTime = Environment.TickCount;
            totalTime = endTime - beginTime;
            return totalTime;
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
        }
        private void btnDelay_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Uses the timer values to set the delay time." + Environment.NewLine);
        }
        private void delayButton_Click(object sender, EventArgs e)
        {
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
            stopFreeRun();
            restoreDefaults();
        }
        private void btnResetSC_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Reset LB-SC to last configuration." + Environment.NewLine);
            txtHelp.AppendText("Not Implemented." + Environment.NewLine);
        }
        private void btnResetSC_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            txtHomePos.Text = getShutterHome().ToString() +" uSteps";
        }
        private void btnStopAutoRun_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Stop free run." + Environment.NewLine);
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopFreeRun();
        }
        private void btnRun_MouseClick(object sender, MouseEventArgs e)
        {
            //lambdaCom.stopFreeRun();
            string myMode = txtRunMode.Text;
            int runCycles = (int)decCycles.Value;
            //Set free run cycles
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetCycles);
            byte[] intBytes = BitConverter.GetBytes(runCycles);
            lambdaCom.writeByte(intBytes[0]);
            lambdaCom.writeByte(intBytes[1]);

            txtBoxDialog.Text = ("Run Mode: On " + myMode + Environment.NewLine);
            switch (myMode)
            {
                case "Command":
                    lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
                    lambdaCom.writeByte(byteCom.byteFreeRunCommand);
                    break;
                case "Power On":
                    lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
                    lambdaCom.writeByte(byteCom.byteSetFreeRunPowerOn);
                    break;
                case "Trigger":
                    lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
                    lambdaCom.writeByte(byteCom.byteSetFreeRunTTLIn);
                    break;
                default:
                    lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
                    lambdaCom.writeByte(byteCom.byteFreeRunCommand);
                    break;
            }
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(193);//Save to controller
        }
        private void btnSetTTL_MouseClick(object sender, MouseEventArgs e)
        {
            string ttlIn, ttlOut;
            ttlIn = txtTrigger.Text;
            ttlOut = txtSync.Text;
            switch (ttlIn)
            {
                case "TTL High":
                    setTTLHigh();
                    break;
                case "TTL Low":
                    setTTLLow();
                    break;
                case "Rising Edge":
                    setTTLToggleRisingEdge();
                    break;
                case "Falling Edge":
                    setTTLToggleFallingEdge();
                    break;
                case "Disabled":
                    setTTLDisabled();
                    break;
            }
            switch (ttlOut)
            {
                case "High on Open":
                    setSyncHighOpen();
                    break;
                case "Low on Open":
                    setSyncLowOpen();
                    break;
                case "Disabled":
                    setSyncDisabled();
                    break;
            }
            return;
        }
        private void btnExposure_MouseClick(object sender, MouseEventArgs e)
        {
            uint usTime = (uint)decUs.Value;
            uint msTime = (uint)decMs.Value;
            uint secTime = (uint)decSec.Value;
            uint minTime = (uint)decMin.Value;
            //lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            setExposureTimer(minTime, secTime, msTime, usTime);
            //lambdaCom.setSingleShot();
            decUs.Value = 0;
            decMs.Value = 0;
            decSec.Value = 0;
            decMin.Value = 0;
        }
        private void btnDelay_MouseClick(object sender, MouseEventArgs e)
        {
            uint usTime = (uint)decUs.Value;
            uint msTime = (uint)decMs.Value;
            uint secTime = (uint)decSec.Value;
            uint minTime = (uint)decMin.Value;
            //lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            setDelayTimer(minTime, secTime, msTime, usTime);
            //lambdaCom.setSingleShot();
            decUs.Value = 0;
            decMs.Value = 0;
            decSec.Value = 0;
            decMin.Value = 0;
        }
        private void btnStopAutoRun_MouseClick(object sender, MouseEventArgs e)
        {
            stopFreeRun();
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(193);//Save to controller
        }
        private void btnRestoreSC_MouseClick(object sender, MouseEventArgs e)
        {
            stopFreeRun();
            restoreDefaults();
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(193);//Save to controller
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
                    wheelA = lambdaCom.getWheelA(status);
                    wheelB = lambdaCom.getWheelB(status);
                    wheelC = lambdaCom.getWheelC(status);
                    shutterA = lambdaCom.getShutterA(status);
                    shutterB = lambdaCom.getShutterB(status);//Note case statments change the scope of the dialog box!
                    shutterC = lambdaCom.getShutterC(status);
                    controllerString = lambdaCom.getConfigString(wheelA, wheelB, wheelC, shutterA, shutterB, shutterC);
                    //dialogBox.AppendText("Lambda 10-3" + Environment.NewLine);
                    txtBoxDialog.AppendText(controllerString);
                    break;
                case "10-B":
                    wheelA = lambdaCom.getWheelA(status);
                    shutterA = lambdaCom.getShutterA(status);
                    shutterB = lambdaCom.getShutterB(status);
                    controllerString = lambdaCom.getConfigString(wheelA, shutterA, shutterB);
                    //dialogBox.AppendText("Lambda 10-B" + Environment.NewLine);
                    txtBoxDialog.AppendText(controllerString);
                    break;
                case "LB10-2":
                    //dialogBox.AppendText("Lambda 10-2" + Environment.NewLine);
                    controllerString = lambdaCom.getConfigString();
                    txtBoxDialog.AppendText(controllerString);
                    break;
                case "SC":
                    shutterA = lambdaCom.getShutterA(status);
                    //dialogBox.AppendText("Lambda SC" + Environment.NewLine);
                    controllerString = lambdaCom.getConfigString(shutterA);
                    txtBoxDialog.AppendText(controllerString);
                    break;
               /* default:
                    wheelA = lambdaCom.getWheelA(status);
                    wheelB = lambdaCom.getWheelB(status);
                    wheelC = lambdaCom.getWheelC(status);
                    shutterA = lambdaCom.getShutterA(status);
                    shutterB = lambdaCom.getShutterB(status);//Note case statments change the scope of the dialog box!
                    shutterC = lambdaCom.getShutterC(status);
                    controllerString = lambdaCom.lambdaCom.getConfigString(wheelA, wheelB, wheelC, shutterA, shutterB, shutterC);
                    //dialogBox.AppendText("Lambda 10-3" + Environment.NewLine);
                    txtBoxDialog.AppendText(controllerString);
                    break;*/
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
                lambdaCom.lbX.openA();
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
                lambdaCom.lbX.closeShutterA();
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
                    shutterOpenButton.Checked = true;
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
        }
        private void btnStop_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("Stops all running tests." + Environment.NewLine);
        }
        private void btnStop_MouseClick(object sender, MouseEventArgs e)
        {
            stopProcess = true;
        }
        private void btnGetConfitg_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("returns the configuration information of the controller." + Environment.NewLine);
        }
        private void btnGetPorts_MouseHover(object sender, EventArgs e)
        {
            txtHelp.AppendText("returns the com ports available for use." + Environment.NewLine);
        }
        private void readPort(){
            string myMode = txtComPort.Text.ToString();
            byte loop;
            //txtCom.AppendText("R-");
            do
            {
                loop = lambdaCom.readByte();
                if (loop != 0 && lambdaCom.isOpen()) { /*txtCom.AppendText(loop + " "); */}
                Application.DoEvents();
            } while (loop != 0);
            //txtCom.AppendText(Environment.NewLine);
        }
        private void btnGteWL_Click(object sender, EventArgs e)
        {
        }
        private void btnClearTxt_MouseClick(object sender, MouseEventArgs e)
        {
            txtBoxDialog.Text = "";
        }
        
        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            txtBoxDialog.Text = "";
            lambdaCom.closeCom();
            //txtCom.AppendText("W-Close Port" + Environment.NewLine);
            readPort();
        }
        private void btnRandomTest_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")
            {
                txtBoxDialog.AppendText("The error detection does not work on the parallel port!" + Environment.NewLine);
            }
            randomTest();
        }
        private void btnHS4Test_MouseClick(object sender, MouseEventArgs e)
        {
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay, filter1, filter2;
            errors = 0;
            delay = int.Parse(txtDelay.Text.ToString());
            speedByte = 0;
            topSpeed = (int)lambdaCom.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lambdaCom.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse(txtNumSteps.Text.ToString());
            filter1 = (int)filter1UpDown.Value;
            filter2 = (int)filter2UpDown.Value;
            wheel = txtTestMode.Text.ToString();
            string status = lambdaCom.getConfig();
            controller = lambdaCom.getController();
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA(status);
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB(status);
                wheelCConfig = lambdaCom.getWheelC(status);
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                Application.DoEvents();
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
            stopProcess = false;
        }

        //******************************************************************************************
        // Versa Chrome Methods
        //need load buffer methods.
        //need to initialize buffer file
        //**********************************************************************************
        private void btnMoveStepAngle_Click(object sender, EventArgs e)
        {

        }
        private void btnMoveNM_MouseClick(object sender, MouseEventArgs e)
        {
            int freqVal = (int)decNanoM.Value;
            byte speed = (byte)decTiltSpeed.Value;
            lambdaCom.vf5.moveNM(speed, freqVal);//moveNM(speed, freqVal);
            return;
        }
        private void btnMoveNM_Click(object sender, EventArgs e)
        {
            return;
        }
        private void btnSweep_MouseClick(object sender, MouseEventArgs e)
        {
            byte speed = 3;
            stopProcess = false;
            checkBoxStop.Checked = false;

            if (radio380.Checked)
            {
                lambdaCom.vf5.sweep380(speed);
            }
            if (radio440.Checked)
            {
                lambdaCom.vf5.sweep440(speed);
            }
            if (radio490.Checked)
            {
                lambdaCom.vf5.sweep490(speed);
            }
            if (radio550.Checked)
            {
                lambdaCom.vf5.sweep550(speed);
            }
            if (radio620.Checked)
            {
                lambdaCom.vf5.sweep620(speed);
            }
            if (radio700.Checked)
            {
                lambdaCom.vf5.sweep700(speed);
            }
            if (radio800.Checked)
            {
                lambdaCom.vf5.sweep800(speed);
            }
            return;
        }
        private void btnMoveStepAngle_MouseClick(object sender, MouseEventArgs e)
        {
            //byte stepIncrementLow;
            byte filterVal = (byte)decVF5Filter.Value;
            byte moveByte = (byte)(lambdaCom.getMoveByte(2, filterVal));
            int stepIncrementLong = (int)decStepInc.Value;
            //http://stackoverflow.com/questions/1318933/c-int-to-byte
            byte[] intBytes = BitConverter.GetBytes(stepIncrementLong);
            //Array.Reverse(intBytes);
            byte[] result = intBytes;
            return;
        }
        //Need to clean up return string.  Fist echo then return data!
        private void btnGteWL_MouseClick(object sender, MouseEventArgs e)
        {
            int range;
            int[] baseval = lambdaCom.vf5.getBaseFilters();
            txtBoxDialog.AppendText("Command" + byteCom.byteVFpossition + " ");
            txtBoxDialog.AppendText("+ " + byteCom.byteGetVFAll +Environment.NewLine);
            //Filter 0
                txtFilter1Base.AppendText(baseval[0].ToString());
                range = baseval[0] - lambdaCom.vf5.getRange(baseval[0]);
                txtF1Range.AppendText(range.ToString());
            //Filter 1
                txtFilter2Base.AppendText(baseval[1].ToString());
                range = baseval[1] - lambdaCom.vf5.getRange(baseval[1]);
                txtF2Range.AppendText(range.ToString());
            //Filter 2
                txtFilter3Base.AppendText(baseval[2].ToString());
                range = baseval[2] - lambdaCom.vf5.getRange(baseval[2]);
                txtF3Range.AppendText(range.ToString());
            //Filter 3
                txtFilter4Base.AppendText(baseval[3].ToString());
                range = baseval[3] - lambdaCom.vf5.getRange(baseval[3]);
                txtF4Range.AppendText(range.ToString());
             //Filter 3
                txtFilter5Base.AppendText(baseval[4].ToString());
                range = baseval[4] - lambdaCom.vf5.getRange(baseval[4]);
                txtF5Range.AppendText(range.ToString());
        }

        private void tabVF5_MouseClick(object sender, MouseEventArgs e)
        {
            ;
        }
        private void btnStep5_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.vf5.Step5();
        }
        private void txtBaudSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            lambdaCom.SetUSBBaudrate(uint.Parse(txtBaudSelect.SelectedItem.ToString()));
        }
        private void txtComPort_SelectedValueChanged(object sender, EventArgs e)
        {
            if (txtComPort.Text.ToString() != "USB") { txtBaudSelect.SelectedIndex = 0; }
            //MessageBox.Show("Baudrate set to: ");
        }
        private void btnStepAll_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.vf5.sweepAll();
       }
        private void btnVF5odd_MouseClick(object sender, MouseEventArgs e)
        {
        }
        private void btnGetZero_MouseClick(object sender, MouseEventArgs e)
        {
            txtVF5Home.Text = getShutterHome().ToString() + " uSteps";
        }

        private void btnPowerDown_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.writeByte(byteCom.bytePowerOff);
        }

        private void btnPowerUp_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.writeByte(byteCom.bytePowerOn);
        }

        private void btnGetConfitg_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.clearBuffer();
            txtBoxDialog.AppendText("ConfigString: " + lambdaCom.getConfig() + Environment.NewLine);
        }

        private void btnGetStatus_MouseClick(object sender, MouseEventArgs e)
        {
            lambdaCom.clearBuffer();
            txtBoxDialog.AppendText("Status String: " + lambdaCom.getStatus() + Environment.NewLine);
        }
//________________________________________________________________________________________________________

        
        //Read write  methodes
        public string readConfigString()
        {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
            txtCom.AppendText(Environment.NewLine + "Read Config. Str: " + Environment.NewLine);
            string inputStr = lambdaCom.readConfigString();
            char[] inputChars = inputStr.ToCharArray();//can not properly read the <CR> from the lambda!
            for (int i = 0; i < inputStr.Length; i++)
            {
                txtCom.AppendText("Char at " + i + Environment.NewLine);
                txtCom.AppendText("Sts.| Dec. | Hex. " + Environment.NewLine);
                string hexOutput = String.Format("{0:X}", (byte)(inputChars[i]));
                string decOutput = String.Format("{0:g}", (byte)(inputChars[i]));
                txtCom.AppendText("   " + inputChars[i].ToString() + "      " + decOutput + "      " + hexOutput + Environment.NewLine);
                //txtCom.AppendText(inputChars[i].ToString()+ " ");
            }
            txtCom.AppendText(Environment.NewLine);
            return inputStr;
        }
        
        public string readString()
        {//can not properly read the <CR> from the lambda so readTo(\r)?? should be \n wierd
            string inputStr = lambdaCom.readString();
            char[] inputChars = inputStr.ToCharArray();//can not properly read the <CR> from the lambda!
            txtCom.AppendText(Environment.NewLine + "Read Str: " + Environment.NewLine);
            for (int i = 0; i < inputStr.Length; i++)
            {
               try
                {
                    txtCom.AppendText("Char at " + i + Environment.NewLine);
                    txtCom.AppendText("Sts.| Dec. | Hex. " + Environment.NewLine);
                    string hexOutput = String.Format("{0:X}", (byte)(inputChars[i]));
                    string decOutput = String.Format("{0:g}", (byte)(inputChars[i]));
                    txtCom.AppendText("   " + inputChars[i].ToString() + "      " + decOutput + "      " + hexOutput + Environment.NewLine);
                }
                catch
                {
                    i = inputStr.Length;
                }
            }
            txtCom.AppendText(Environment.NewLine);
            return inputStr;
        }
        
        // Lambda specific methods_____________________________________________________
        //Get Config mthods_________________________________________________________ 


        public byte getShutterHome()
        {
            byte home;
            byte mask = 240;//F0
            //lambdaCom.clearBuffer();//Good to clear the buffer
            lambdaCom.writeByte(byteCom.byteGetHomePos);
            home = lambdaCom.readByte();
            home = lambdaCom.readByte();
            home = (byte)(home - mask);
            /*txtBoxDialog.AppendText(home + "Home" + Environment.NewLine);
            txtBoxDialog.AppendText(home + "Home Return");*/
            //For testting
            lambdaCom.clearBuffer();//Good to clear the buffer
            return home;
        }
        //Move commands_______________________________________________________________________

        
        //Shutter specific methods here___________________________________________________
       
        //TTL methods________________________________________________________________________
        public void setTTLDisabled()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteDisableTTL);
            return;
        }
        public void setTTLHigh()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetTTLHigh);
            return;
        }
        public void setTTLLow()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetTTLLow);
            return;
        }
        public void setTTLToggleRisingEdge()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetTTLToggleRising);
            return;
        }
        public void setTTLToggleFallingEdge()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetTTLToggleFalling);
            return;
        }
        public void setSyncDisabled()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteDisableSync);
            return;
        }
        public void setSyncHighOpen()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSyncHighOpen);
            return;
        }
        public void setSyncLowOpen()
        {
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSyncLowOpen);
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
            if (ms >= 100) { msString = msString.Insert(0, "0"); }
            if (ms < 100 && ms > 9) { msString = msString.Insert(0, "00"); }
            if (ms < 10) { msString = msString.Insert(0, "000"); }
            onesMs = msString.Substring(3, 1);
            msOnes = int.Parse(onesMs);
            byte1 = (byte)((msOnes << 4) + us);
            //byte two
            try { tensMs = msString.Substring(2, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                tensMs = "0";
            }
            msTens = int.Parse(tensMs);
            try { hundredsMs = msString.Substring(1, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                hundredsMs = "0";
            }
            msHundreds = int.Parse(hundredsMs);
            byte2 = (byte)((msHundreds << 4) + msTens);
            byte3 = (byte)(sec);
            byte4 = (byte)(min);
            //byte4 = (byte)(16 + byte4);//set delay
            //Set timeer
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(16);//set exposure + zero hours, 0-5
            lambdaCom.writeByte(byte4);
            lambdaCom.writeByte(byte3);
            lambdaCom.writeByte(byte2);
            lambdaCom.writeByte(byte1);
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
            if (ms >=100) { msString = msString.Insert(0, "0"); }
            if (ms < 100 && ms > 9) { msString = msString.Insert(0, "00"); }
            if (ms < 10) { msString = msString.Insert(0, "000"); }
            onesMs = msString.Substring(3, 1);
            msOnes = int.Parse(onesMs);
            byte1 = (byte)((msOnes << 4) + us);
            //byte two
            try { tensMs = msString.Substring(2, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                tensMs = "0";
            }
            msTens = int.Parse(tensMs);
            try { hundredsMs = msString.Substring(1, 1); }
            catch (Exception) // catches without assigning to a variable
            {
                hundredsMs = "0";
            }
            msHundreds = int.Parse(hundredsMs);
            //msHundreds = 20;
            byte2 = (byte)((msHundreds << 4) + msTens);
            //txtBoxDialog.AppendText("String: " + byte2 + " 100's " + msHundreds + " 10's " + msTens + " 1's" + msOnes);
            byte3 = (byte)(sec);
            byte4 = (byte)(min);
            // byte4 = (byte)(32 + byte4);//set exposure
            //Set timeer
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(32);//set exposure zero hours, 0-5
            lambdaCom.writeByte(byte4);
            lambdaCom.writeByte(byte3);
            lambdaCom.writeByte(byte2);
            lambdaCom.writeByte(byte1);
            return;
        }

        public void setFreeRunTTL(int cycles)
        {
            byte byte1, byte2;
            byte2 = (byte)(cycles >> 8); // second byte
            byte1 = (byte)(cycles);
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetCycles);
            lambdaCom.writeByte(0);//lambdaCom.writeByte(byte3);
            lambdaCom.writeByte(1);//lambdaCom.writeByte(byte1);
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteSetFreeRunTTLIn);//242 ttl
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
            lambdaCom.writeByte(byteCom.byteLBSC_Prefix);
            lambdaCom.writeByte(byteCom.byteResetDefault);
            return;
        }
        public void restoreLast()
        {
            stopFreeRun();
            lambdaCom.writeByte(byteCom.byteRestorLast);
            return;
        }
        public void stopFreeRun()
        {
            lambdaCom.writeByte(byteCom.byteStopFreeRun);
            return;
        }
        //VF Specific commands.*************************************************************************

        private void btnLB10B_Batch_MouseClick(object sender, MouseEventArgs e)
        {
            string ShutterA = txtBatchA.Text;
            string ShutterB = txtBatchB.Text;
            lambdaCom.writeByte(189);
            if (ShutterA == "Open") { lambdaCom.lbX.openAdefault(); }
            else { lambdaCom.lbX.closeShutterA(); }
            if (ShutterB == "Open") { lambdaCom.lbX.openBdefault(); }
            else { lambdaCom.lbX.closeShutterB(); }
            lambdaCom.writeByte(190);
        }

        private void btnSeqTest_MouseClick(object sender, MouseEventArgs e)
        {
            stopProcess = false;
            int errors, topSpeed, lastSpeed, cycleCount, delay, filter1, filter2;
            errors = 0;
            delay = 0;
            speedByte = 0;
            topSpeed = (int)lambdaCom.getSpeedByte(txtTopSpeed.Text.ToString());
            lastSpeed = (int)lambdaCom.getSpeedByte(txtLastSpeed.Text.ToString());
            cycleCount = int.Parse( txtNumSteps.Text.ToString());
            filter1 = (int)filter1UpDown.Value;
            filter2 = (int)filter2UpDown.Value;
            txtBoxDialog.AppendText("cycleCount: " + cycleCount + " filter1-" + filter1 + " filter2-" + filter2 + "  " + Environment.NewLine);
            wheel = txtTestMode.Text.ToString();
            controller = lambdaCom.getController();
            string status = lambdaCom.getConfig();
            if (txtComPort.Text.ToString() == "LPT1" || txtComPort.Text.ToString() == "LPT2")
            {
                txtBoxDialog.AppendText("The error detection does not work on the parallel port!" + Environment.NewLine);
            }
            if (controller == "LB10-3" || controller == "LB10-B")
            {
                wheelAConfig = lambdaCom.getWheelA(status);
            }
            if (controller == "LB10-3")
            {
                wheelBConfig = lambdaCom.getWheelB(status);
                wheelCConfig = lambdaCom.getWheelC(status);
            }
            //Speed loop
            for (int s = topSpeed; s <= lastSpeed; s++)
            {
                Application.DoEvents();//Need to process the close com event while in the loop.
                if (lambdaCom.isOpen() == false || stopProcess == true) { break; }
                if (lambdaCom.isOpen() == false || stopProcess == true) { return; }//escape
                    errors = myRandom(cycleCount, delay, s, filter1, filter2, "seqRandom");
                if (errors > 0 || delay > 200)
                {
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " is not a good speed at delay-" + delay.ToString()+ " . " + Environment.NewLine);
                    errors = 0;
                    delay = delay + 10;
                    s--;
                }
                else
                {
                    txtBoxDialog.AppendText("A goog speed is:" + Environment.NewLine);
                    txtBoxDialog.AppendText("Speed " + s.ToString() + " at  delay " + delay.ToString() + Environment.NewLine);
                    delay = 10;
                }
                //delay = 10;
                errors = 0;
            }
            stopProcess = false;
        }

        private void btnShutterTimer_MouseClick(object sender, MouseEventArgs e)
        {
            int openTime = (int)decOpenMs.Value;
            int closeTime = (int)decCloseMs.Value;
            lambdaCom.writeByte(byteCom.byteOpenA);
            do
            {
                Thread.Sleep(openTime);
                lambdaCom.writeByte(byteCom.byteCloseA);
                Thread.Sleep(closeTime);
                lambdaCom.writeByte(byteCom.byteOpenA);
                Application.DoEvents();
            } while (stopProcess != true);
            stopProcess = false;
            return;
        }
           
}

 /*This function is not reliable.  Most likly on the DG-4 side?
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
