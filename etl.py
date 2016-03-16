"""A module for controlling an Electically Tunable lens from Optotine using their USB driver stick. 

Functions:
	openETL -- opens a serial port on COM4 and performs the handshake necessary to open communication with the driver.


Dependencies:
	pyserial

References:
	Optotune Lens Driver 4 manual, available locally on Denali as pdf
	Documentation for PySerial at http://pyserial.sourceforge.net/index.html

"""

import serial

def openETL():
	"""Performs handshake and returns serial object and response, which should read \'Ready\\r\\n\'"""
	ETL = serial.Serial(3, baudrate=115200, timeout=10)
	ETL.write('Start')
	error = ETL.readline()
	return ETL, error


