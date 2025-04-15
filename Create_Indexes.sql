CREATE INDEX IX_DEXMeter_Machine_Date
ON DEXMeter (Machine, DEXDateTime);

CREATE INDEX IX_DEXLaneMeter_Product
ON DEXLaneMeter (ProductIdentifier);

CREATE INDEX IX_DEXLaneMeter_DEXMeterId
ON DEXLaneMeter (DEXMeterId);
