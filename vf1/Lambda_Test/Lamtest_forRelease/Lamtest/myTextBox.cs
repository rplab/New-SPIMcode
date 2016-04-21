using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lamtest
{
        public class myTextBox : TextBox
        {
            private const int WM_CHAR = 0x102;
            protected override void WndProc(ref Message m)
            {
                //See if the CTRL key is being pressed.
                if (myTextBox.ModifierKeys.CompareTo(Keys.Control) == 0)
                {
                    switch (m.Msg)
                    {
                        case WM_CHAR:
                            // Disable CTRL+X.
                            switch (m.WParam.ToInt32())
                            {
                                case 24://X = 24th letter of alphabet
                                    break;
                                // Do nothing here to disable the default message handling.
                                default:
                                    //Make sure that you pass unhandled messages back to the default message handler.
                                    base.WndProc(ref m);
                                    break;
                            }
                            break;
                        default:
                            //Make sure that you pass unhandled messages back to the default message handler.
                            base.WndProc(ref m);
                            break;
                    }
                }
                else
                    //Make sure that you pass unhandled messages back to the default message handler.
                    base.WndProc(ref m);
            }
        }

}
