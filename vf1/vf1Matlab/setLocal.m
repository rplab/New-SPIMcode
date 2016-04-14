% Program:      setLocal.m
% Summary:      Quick function to set VF1 to local mode.  Useful as a quick
%               communication test.
% Inputs:       device = handle to serial object
% Outputs:      None
% Author:       Brandon Schlomann
% Date:         4/14/16

function setLocal(device)

if strcmp(device.Status,'open') == 1
    
    % Send '239' as one byte to set to local
    fwrite(device,char(239),'char')
    
    % Read out 2 bytes from buffer
    fread(device,2);
    

else
    disp('Serial port not open')
end

end