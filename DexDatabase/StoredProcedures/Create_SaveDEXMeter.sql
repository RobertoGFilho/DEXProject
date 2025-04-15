CREATE PROCEDURE SaveDEXMeter
    @Machine CHAR(1),
    @DEXDateTime DATETIME2,
    @MachineSerialNumber VARCHAR(50),
    @ValueOfPaidVends DECIMAL(10,2)
AS
BEGIN
    INSERT INTO DEXMeter (Machine, DEXDateTime, MachineSerialNumber, ValueOfPaidVends)
    VALUES (@Machine, @DEXDateTime, @MachineSerialNumber, @ValueOfPaidVends);

    SELECT SCOPE_IDENTITY() AS DexMeterId;
END;

