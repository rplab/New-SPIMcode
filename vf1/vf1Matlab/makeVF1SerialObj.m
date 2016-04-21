% Program:      makeVF1SerialObj.m
% Summary:      Quick function to create a serial object for the VF1 rotating mount
%               with the correct attributes.
% Inputs:       port = string of port id, i.e. 'COM4'.
% Outputs:      vf1 = serial object called vf1
% Author:       Brandon Schlomann
% Date:         4/14/16

function vf1 = makeVF1SerialObj(port)

vf1 = serial(port,'baudrate',128000,'terminator',13,'timeout',.5);
fopen(vf1);

end