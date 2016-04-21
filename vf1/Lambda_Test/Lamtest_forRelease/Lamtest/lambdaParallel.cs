/* -----------------------------------------------------------------
 * 
 * LED initialization code written by Levent S. 
 * E-mail: ls@izdir.com
 * 
 * This code is provided without implied warranty so the author is
 * not responsible about damages by the use of the code.
 * 
 * You can use this code for any purpose even in any commercial 
 * distributions by referencing my name. 
 * 
 * ! Don't remove or alter this notice in any distribution !
 * 
 * -----------------------------------------------------------------*/
//http://logix4u.net/Legacy_Ports/Parallel_Port/How_Inpout32.dll_works_.html
//http://sandeep-aparajit.blogspot.com/2008/08/io-how-to-program-readwrite-parallel.html
/*----------------------------------------------------------------------
 * 
 * In order for this class to work the driver "inpout32.dll" 
 * must be installed in the dirrectory Windows/System32
 * 
 * The DLL Inpout32:
 * The functions in the DLL are implemented in two source files, "inpout32drv.cpp" and "osversion.cpp". 
 * osversion.cpp checks the version of operating system. "inpout32drv.cpp" does installing the kernel mode driver, 
 * loading it , writing/ reading parallel port etc... The two functions exported from inpout32.dll are
 * 
 * 1) Inp32(), reads data from a specified parallel port register.
 * 2) Out32(), writes data to specified parallel port register.
 * 
 * The other functions implemented in Inpout32.dll are 
 * 1) DllMain(), called when dll is loaded or unloaded. When the dll is loaded , it 
 * checks the OS version and loads hwinterface.sys if needed.
 * 2) Closedriver(), close the opened driver handle. called before unloading the driver.
 * 3) Opendriver(), open a handle to hwinterface driver.
 * 4) inst() , Extract 'hwinterface.sys' from binary resource to 'systemroot\drivers' 
 * directory and creates a service. This function is called when 'Opendriver' function fails 
 * to open a valid handle to 'hwinterface' service.
 * 5) start() , starts the hwinterface service using Service Control Manager APIs.
 * 6) SystemVersion() Checks the OS version and returns appropriate code.

 * ------------------------------------------------------------------*/
using System;
using System.Runtime.InteropServices;//From portAccess
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Lamtest
{
    class lambdaParallel
    {
        [DllImport("inpout32.dll", EntryPoint="Out32")]
	    //public static extern void Output(int adress, int value);
        public static extern void Output(short adress, short value);
	    [DllImport("inpout32.dll", EntryPoint="Inp32")]
	    public static extern byte Input(int adress);
    }
}
