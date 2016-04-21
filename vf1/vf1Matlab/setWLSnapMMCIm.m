% FOR USE WITH SUTTER LAMBDA VF-1/VERSACHROME TUNABLE BP FILTERS/LAMBDA 10-B 
% CONTROLLER.
%
% Set the wavelength according to the pre-programmed lookup table.  All 
% communication is structred in single byte messages.  In this case, one byte 
% is sent indicating the setting of a wavelength, and then the wavelength is 
% represented by 2 bytes.  NOTE:  An option exists to control the speed of 
% rotation that involves appending more bits to one of the wavelength bytes, 
% which would involve modification to this code.  See the VF-5 operations manual, 
% available on the Sutter website under 'Tech Support', 'Product Manuals'
% tab.  
%
% Inputs:  device = handle of serial device
%          lambda = wavelength in nm
%          core = CMMCore object
%
% Author:  Brandon Schlomann
%
% Date:     7/28/15:    First written as setWL
%           4/14/16:    Added lines to read from buffer.  Important to keep
%                       buffer clear if you want to actually read something
%                       important from the device.
%           4/17/16:    Added specific routine to snapIm instead of bang. 
%                       Renamed setWLSnapMMCIm, added third input.

function   setWLSnapMMCIm(device,lambda,core )

% Convert to 2 bytes
lambda2byte = int16(lambda);


if strcmp(device.Status,'open') == 1
    
    % Send '218' as one byte to initialize WL change
    fwrite(device,char(218),'char')
    
    % Send the 2 byte WL value
    fwrite(device,lambda2byte,'int16')
    
    % Read out first 3 bytes from the buffer.
    % First byte = echoing 218 command
    % Second and Third bytes = WL in hex
    fread(device,3);
    
    % Read out carriage return, indicates motor has moved
    didMove = fread(device,1);
    if ~isempty(didMove)
        % snap image with CMMCore
        core.snapImage();
    end
    
else
    disp('Serial port not open')
end


end

