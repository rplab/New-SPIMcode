% Program:  initializeMMC_tableTop_HamamatsuOnly.m
% Summary:  For use with testing micro manager implementation in matlab
%           on the tabletop setup.  Import micro manager core object and set
%           initialize with the Hamamatsu camera.  Turn camera on before
%           running.  Initialize w/ 30ms exposure.
% Inputs:   None
% Outputs:  None
% Author:   Brandon Schlomann
% Date:     4/17/16:  First draft written.

function mmc = initializeMMC_tableTop_HamamatsuOnly()
import mmcorej.*;
mmc = CMMCore;
mmc.loadSystemState ('C:\Program Files\Micro-Manager-1.4\Hamamatsu_only.cfg');
mmc.loadDevice('Camera','HamamatsuHam','HamamatsuHam_DCAM')
mmc.initializeDevice('Camera')
mmc.setCameraDevice('Camera');
mmc.setExposure(30);


end

