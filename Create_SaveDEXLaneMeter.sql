CREATE PROCEDURE SaveDEXLaneMeter
    @DEXMeterId INT,
    @ProductIdentifier VARCHAR(50),
    @Price DECIMAL(10,2),
    @NumberOfVends INT,
    @ValueOfPaidSales DECIMAL(10,2)
AS
BEGIN
    INSERT INTO DEXLaneMeter (DEXMeterId, ProductIdentifier, Price, NumberOfVends, ValueOfPaidSales)
    VALUES (@DEXMeterId, @ProductIdentifier, @Price, @NumberOfVends, @ValueOfPaidSales);
END;
