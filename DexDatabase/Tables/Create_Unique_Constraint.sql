ALTER TABLE DEXMeter
ADD CONSTRAINT UQ_DEXMeter_Machine_Date UNIQUE (Machine, DEXDateTime);


