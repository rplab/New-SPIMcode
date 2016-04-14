% Program:      getWL.m
% Summary:      Get WL value of VF1.
% Inputs:       device = handle to serial object
% Outputs:      wl = wavelength in nanometers
% Author:       Brandon Schlomann
% Date:         4/14/16

function wl = getWL(device)

if strcmp(device.Status,'open') == 1
    
    % Send '219' as one byte to set to on line
    fwrite(device,char(219),'char')
    
    % Read 2 'echo' bytes
    fread(device,2);
    
    % Read out 2 bytes from buffer representing WL
    lb = fread(device,1);
    hb = fread(device,1);
    
    % Read out carriage return byte
    fread(device,1);
    
    % Conver to decimal
    wl = hb*256 + lb;
    % Ex:  hb = 2, lb = 8, wl = 2*256 + 8 = 520
    
else
    disp('Serial port not open')
end

end