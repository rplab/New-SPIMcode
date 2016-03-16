'''Functions written hastily when imaging CUBIC cleared brain tissue. The intent was to continuously adjust the focus as the imaging depth increased, which was neccessary because an air objective was used to image the sample, which was held in a glass fluorimeter cell and had a high index of refraction.'''
import PyDAQmx as pydaq
import numpy as np
import time
import os
import sys

import kbhit as kbhit

def galvoScan(focus, offset, images, exposure, readout=10., stripe=False, aomStripeWidth=50, focusChannel="Dev1/ao0", scanChannel="Dev1/ao1"):
	"""Function to scan galvo mirror across field of view, centered on offset voltage and with the focus set by the perpendicular galvo. This function is specific to a National Instruments DAQ card, used to send signals to the mirror galvonometers. A trigger (e.g. from the camera) is currently necessary on PFI0, if undesirable, change CfgDigEdgeStartTrig.
	
	***To Do:
	    - add error handling with Try/Except
	    - add scan signal bounds as an input. Not only does this vary a little with alignment, but needs to be different
	      for the visible laser galvos and enables an ROI.

	    Done - add a digital I/O channel to accept a trigger from the camera.
	    Done - figure out how to wait until sweep is done before exiting function (something better than timed delay?)

	Inputs:
	    focus - float, Voltage (in V) used by the off-axis galvo to bring the laser into focus.
	    offset - float, Voltage (in V) that steers the laser through the center of the scan range.
	    images - int, number of images in the scan
	    exposure - float/int, exposure time in milliseconds. The laser should take this long to cover the field of view.
	               *** In light sheet readout mode, the exposure time given to the camera refers to each 4 row block exposed in succession
		       So the number here should be equal to Vn/4 * Exp1 (total number of rows and camera's exposure time)
	    readout - float, readout time in milliseconds of the camera
	    
	    focusChannel - string, Hardware device and channel that controls the focus galvo, e.g. "Dev1/ao0"
	    scanChannel - string, Hardware device and channel that controls the scan galvo, e.g. "Dev1/ao1"
	"""

	focus = float(focus)
	#z = np.linspace(0, 400, images)
	
	offset = float(offset)
	exposure = exposure/1000.
	readout = readout/1000.

	analog_output = pydaq.Task()
	analog_output.CreateAOVoltageChan(focusChannel, "", -5.0, 5.0, pydaq.DAQmx_Val_Volts, None)
	analog_output.CreateAOVoltageChan(scanChannel, "", -5.0, 5.0, pydaq.DAQmx_Val_Volts, None)
	analog_output.CreateAOVoltageChan("Dev1/ao2", "", 0., 10., pydaq.DAQmx_Val_Volts, None)
	analog_output.CreateAOVoltageChan("Dev1/ao3", "", 0., 10., pydaq.DAQmx_Val_Volts, None)


	"""Create a timer that can be used to trigger the camera in light sheet readout mode (Hamamatus Orca Flash).
	There is a delay in the camera (see manual) of 1H*9, where 1H = HLN/(26600000) (HLN 2592 to 266*10^6 - refer to section 10-3-2 of the manual).
	The frame rate is calculated as 1/(Exp1 + (Vn+10)*1H), where Vn is the number of vertical lines and Exp1 can be varied from 9.7us to 10s
	"""
	HLN = 2592
	H1 = HLN/266000000.
	readout = 2048*H1
	print(readout)
	timer = pydaq.Task()
	timer.CreateCOPulseChanTime("Dev1/Ctr0", "timer", pydaq.DAQmx_Val_Seconds, pydaq.DAQmx_Val_Low, 0.01, readout, exposure)
	timer.CfgImplicitTiming(pydaq.DAQmx_Val_FiniteSamps, images)
	
	"""For the time being, I am assuming that the field of view is (over)filled by sweeping through 600 milliVolts.
	At 40x with the Hamamatsu sCMOS, this should be about 333 microns, so there is a rough correspondence of 1.8mV to 
	1um. Since the NIR (bessel) beam has a FWHM of about 3um, I will move in 1mV (0.55um) steps, which should appear
	continuos. This means that the sampling rate should be set to samples/duration (where samples is 600)."""
	samples = 1000
	samplingRate = float(samples)/exposure
	analog_output.CfgSampClkTiming(None, samplingRate, pydaq.DAQmx_Val_Rising, pydaq.DAQmx_Val_FiniteSamps, samples+1)

	scan = np.linspace(-1.800, 1.800, samples) + offset
	#go back to starting value:
	scan = np.append(scan, scan[0])
	#print(str(scan[0]) + str(scan[-1]))
	focus = focus*np.ones(samples)
	#for debugging, go back to 0:
	#focus = np.append(focus, 0.0)
	temp = np.concatenate((np.zeros(aomStripeWidth), 10.*np.ones(aomStripeWidth)),0)
	temp = np.tile(temp, samples/len(temp))
	if (stripe==True):
		aom = temp[0:samples+1]
	else:
		aom = 10.*np.ones(samples)
	blank = 10*np.ones(samples)
	aom = np.append(aom, 0.)
	blank = np.append(blank, 0.)
	#To minimize unnessecary sample exposure, move focus far away:
	focus = np.append(focus, -3.0 )
	samples = samples + 1

	"""Since the analog out write function has an ouput that is the actual number of samples per channel successfully
	written to the buffer, create a variable to store those values: """
	temp = (pydaq.c_byte*4)()
	actualWritten = pydaq.cast(temp, pydaq.POINTER(pydaq.c_long))


	#analog_output.CfgDigEdgeStartTrig("PFI12", pydaq.DAQmx_Val_Rising)
	analog_output.CfgDigEdgeStartTrig("PFI0", pydaq.DAQmx_Val_Rising)

	if (focusChannel == "Dev1/ao0"):
		writeData = np.concatenate((focus, scan, aom, blank),1)
	else:
		writeData = np.concatenate((scan, focus, aom, blank),1)

	timer.StartTask()
	
	for image in np.arange(0,images):
		writeData = np.concatenate((focus, scan, aom, blank),1)
		analog_output.WriteAnalogF64(samples, False, -1, pydaq.DAQmx_Val_GroupByChannel, writeData, actualWritten, None)
		analog_output.StartTask()

		done = pydaq.bool32()
		analog_output.IsTaskDone(pydaq.byref(done))
	
	#	while (done.value != 1):
	#		analog_output.IsTaskDone(pydaq.byref(done))
		
		analog_output.WaitUntilTaskDone(10)
		analog_output.StopTask()
		focus = focus - 0.001352

	timer.WaitUntilTaskDone(-1)
	timer.StopTask()



	return



def galvoContinuousScan(focus, offset, duration, focusChannel="Dev1/ao0", scanChannel="Dev1/ao1"):
	"""Function to continuously scan galvo mirror across field of view, centered on offset voltage and with the focus set by the perpendicular galvo. This function is specific to a National Instruments DAQ card, used to send signals to the mirror galvonometers. This function is useful when the camera is in free-runnung mode for, e.g. sample location, focus adjustment, etc.

	The main differences between this function and galvoScan are that 1. writing to the DAQ card occurs continuously in a loop and 2. writing starts immediately (there is no waiting for an external trigger).


	***To Do:
	    - add error handling with Try/Except


	Inputs:
	    focus - float, Voltage (in V) used by the off-axis galvo to bring the laser into focus.
	    offset - float, Voltage (in V) that steers the laser through the center of the scan range.
	    duration - float, Time (in s) that the scan should take to completely sweep the field of view.
	    focusChannel - string, Hardware device and channel that controls the focus galvo, e.g. "Dev1/ao0"
	    scanChannel - string, Hardware device and channel that controls the scan galvo, e.g. "Dev1/ao1"
	"""
	analog_output = pydaq.Task()
	analog_output.CreateAOVoltageChan(focusChannel, "", -5.0, 5.0, pydaq.DAQmx_Val_Volts, None)
	analog_output.CreateAOVoltageChan(scanChannel, "", -5.0, 5.0, pydaq.DAQmx_Val_Volts, None)
	
	"""For the time being, I am assuming that the field of view is (over)filled by sweeping through 600 milliVolts.
	At 40x with the Hamamatsu sCMOS, this should be about 333 microns, so there is a rough correspondence of 1.8mV to 
	1um. Since the NIR (bessel) beam has a FWHM of about 3um, I will move in 1mV (0.55um) steps, which should appear
	continuos. This means that the sampling rate should be set to samples/duration (where samples is 600)."""
	samples = 10000*2
	samplingRate = float(samples)/duration
	analog_output.CfgSampClkTiming(None, samplingRate, pydaq.DAQmx_Val_Rising, pydaq.DAQmx_Val_ContSamps, samples)

	scan = np.linspace(-1.800, 1.800, samples/2.) + offset
	"""Trace backward for less acceleration on the galvo (pyramid instead of sawtooth):
	"""
	scan = np.concatenate((scan,scan[::-1]),0)
	focus = focus*np.ones(samples)


	"""Since the analog out write function has an ouput that is the actual number of samples per channel successfully
	written to the buffer, create a variable to store those values: """
	temp = (pydaq.c_byte*4)()
	actualWritten = pydaq.cast(temp, pydaq.POINTER(pydaq.c_long))

	if (focusChannel == "Dev1/ao0"):
		writeData = np.concatenate((focus, scan),1)
	else:
		writeData = np.concatenate((scan, focus),1)

	analog_output.WriteAnalogF64(samples, True, -1, pydaq.DAQmx_Val_GroupByChannel, writeData, actualWritten, None)

	kb = kbhit.KBHit()
	while(True):
		if kb.kbhit():
			try:
				k_in = kb.getarrow()
				analog_output.StopTask()
				if (k_in == 1):
					focus = focus + 0.001
					print('offset = %5.3f, focus = %5.3f' % (scan[0], focus[0]))
				elif (k_in == 3):
					focus = focus - 0.001
					print('offset = %5.3f, focus = %5.3f' % (scan[0], focus[0]))
				elif (k_in == 0):
					scan = scan + 0.001
					print('offset = %5.3f, focus = %5.3f' % (scan[0], focus[0]))
				elif (k_in == 2):
					scan = scan - 0.001
					print('offset = %5.3f, focus = %5.3f' % (scan[0], focus[0]))
				else:
					derp = "derr"
				writeData = np.concatenate((focus, scan),1)
				analog_output.WriteAnalogF64(samples, False, -1, pydaq.DAQmx_Val_GroupByChannel, writeData, actualWritten, None)
				analog_output.StartTask()
				#print(k_in, k_in==1)
			#I currently can't figure out how to exit gracefully, this exception doesn't allow the function to be called again, as the task is still reserved (?)
	#		except KeyboardInterrupt:
	#			print("caught a keyboard interrupt!")
			except:
				Exit = raw_input("Want to exit?")
				if (Exit == 'y'):
					analog_output.StopTask()
					analog_output.ClearTask()
					return [np.median(scan), focus[0]]
					#sys.exit(0)
				else:
					analog_output.StopTask()

	#For the time being, I will leave the program running until the user throws a keyboard interrupt with Ctr-C

	#try:
	#	while (1==1):
	#		derp = "derp"
	#except:
	#	derp = "derr"


	return








