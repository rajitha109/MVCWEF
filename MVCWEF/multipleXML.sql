 CREATE PROCEDURE
 getAllDetails 
 as
 BEGIN
 DECLARE @TempExportTable TABLE
 (
 Product XML,
 Seller XML
)
INSERT INTO @TempExportTable VALUES
(
(SELECT * FROM Product FOR XML AUTO,ROOT('byProduct')),
(SELECT * FROM Seller FOR XML AUTO,ROOT('bySeller'))
)

SELECT
 Product as '*',
 Seller as '*'
from @TempExportTable
FOR XML PATH('ExportList')
END