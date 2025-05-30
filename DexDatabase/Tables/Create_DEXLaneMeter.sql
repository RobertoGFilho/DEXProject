﻿CREATE TABLE DEXLaneMeter (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DEXMeterId INT NOT NULL,
    ProductIdentifier VARCHAR(50) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    NumberOfVends INT NOT NULL,
    ValueOfPaidSales DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (DEXMeterId) REFERENCES DEXMeter(Id)
);

