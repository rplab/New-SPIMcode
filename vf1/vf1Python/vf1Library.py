# Program:  vf1Library.py
# Summary:  Library of functions for communicating with Sutter's VF1 rotating mount
#           housing a Semrock tuneable bp filter.  Communication with the VF1 is 
#           done by in 1 and 2 byte messages.  For 1 byte messages, we simply use
#           chr(integer), where 0 <= integer <= 255.  For 2 byte messages, we use
#           the struct package to pack a python int WL value into 2 bytes.  Message 
#           dictionary is available in Sutter's VF-5 manual, found at
#           http://www.sutter.com/manuals/LBVF-5_OpMan.pdf
#           though it contains much information not relevant for the VF-1.
# Author:   Brandon Schlomann


import serial
import struct

def createVF1Serial(port):
    # create and return a pyserial Serial class, called vf1.  Set required baudrate and a timeout.
    # Input port is a string, name of appropriate serial port, e.g, for usual USB 
    # connection on the table-top computer, port = 'COM4'.
    #
    # To view available ports, in terminal type 
    # python -m serial.tools.list_ports

    vf1 = serial.Serial(port)
    vf1.baudrate = 128000                       # baudrate for vf1
    vf1.timeout = .5                            # .5 second timeout
    
    return vf1

def setLocal(ser):
    if ser.isOpen():
        ser.write(chr(239))
        ser.read(2)                             # read output to clear buffer
    else:
        print 'vf1 is not open'

def setOnline(ser):
    if ser.isOpen():
        ser.write(chr(238))
        ser.read(2)                             # read output to clear buffer
    else:
        print 'vf1 is not open'
        
def setWL(ser,wl):
    if ser.isOpen():
        ser.write(chr(218))
        ser.write(struct.pack('<H',wl))         # '<H' means little-endian, unsigned 2 byte int
        out = ser.read(4)                       # read output to clear buffer
        
        # For future, may want to trigger camera         
        if '\r' in out:                         # '\r' = carriage return, appears after motor is stepped
            trigTest()
            
    else:
        print 'vf1 is not open'
        
def getWL(ser):
    if ser.isOpen():
        ser.write(chr(219))
        ser.read(1)                             # Read first byte, echo of command
        ser.read(1)                             # Read second byte, always \x01
        
        # Read hex bytes that make up wavelength
        lowbyte = ser.read(1)                   
        highbyte = ser.read(1)
        ser.read(1)
        
        currentWL = ord(highbyte)*256 + ord(lowbyte)
        # Ex:  WL = 520, highbyte = 2, lowbyte = 8
        
        return currentWL
        
    else:
        print 'vf1 is not open'

def trigTest():
    print 'Bang!'       