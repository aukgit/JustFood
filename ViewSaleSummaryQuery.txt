SELECT        dbo.DetailedSale.SaleID, dbo.ViewInventorySalables.CategoryID, dbo.ViewInventorySalables.Category, dbo.ViewInventorySalables.SoldPrice, 
                         dbo.ViewInventorySalables.InventoryQuantity, COUNT(CASE WHEN dbo.DetailedSale.IsDiscard = 0 THEN 1 END) 
                         - COUNT(CASE WHEN dbo.DetailedSale.IsDiscard = 1 THEN 1 END) AS SoldQuantity, SUM(dbo.DetailedSale.Discount) AS Discounted, SUM(dbo.DetailedSale.SoldAt) 
                         AS CashInHand
FROM            dbo.ViewInventorySalables LEFT OUTER JOIN
                         dbo.DetailedSale ON dbo.ViewInventorySalables.CategoryID = dbo.DetailedSale.CategoryID
GROUP BY dbo.ViewInventorySalables.CategoryID, dbo.ViewInventorySalables.Category, dbo.ViewInventorySalables.SoldPrice, dbo.ViewInventorySalables.InventoryQuantity, 
                         dbo.DetailedSale.SaleID
HAVING        (dbo.DetailedSale.SaleID IS NOT NULL)