function [imstack] = sweepWLmmc_test(vf1,mmc,wlvec)

nwls = numel(wlvec);
imrows = zeros(nwls,2048^2);
imstack = zeros(2048,2048,nwls);
for i = 1:nwls
    setWLSnapMMCIm(vf1,wlvec(i),mmc);
    imrows(i,:) = mmc.getImage();    
end

for j = 1:nwls
    imstack(:,:,j) = reshape(imrows(i,:),[2048,2048]);
end

end