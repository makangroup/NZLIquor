DELETE FROM GiftCard
DELETE FROM [OrderItem]
DELETE FROM [Order]
DELETE FROM [CorporateCustomerCompanyOwners]
DELETE FROM [CorporateCustomerTradeReferences]
DELETE FROM [CorporateCustomer]
DELETE FROM [Customer] WHERE Id NOT IN (1)
DELETE FROM Affiliate
DELETE FROM [Address] WHERE Id NOT IN (1, 7, 11, 12)