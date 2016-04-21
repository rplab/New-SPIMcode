% Program:      setOnLine.m
% Summary:      Quick function to set VF1 to OnLine mode.  Useful as a quick
%               communication test.
% Inputs:       device = handle to serial object
% Outputs:      None
% Author:       Brandon Schlomann
% Date:         4/14/16

function setOnLine(device)

if strcmp(device.Status,'open') == 1
    
    % Send '238' as one byte to set to on line
    fwrite(device,char(238),'char')
    
    % Read out 2 bytes from buffer
    fread(device,2);
    

else
    disp('Serial port not open')
end

end