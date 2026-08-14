-- Run once against your StockCli database.
-- Changes Quantity from INT to DECIMAL(18, 3) so Kg can store values like 1.5.

ALTER TABLE StockItems
ALTER COLUMN Quantity DECIMAL(18, 3) NOT NULL;
