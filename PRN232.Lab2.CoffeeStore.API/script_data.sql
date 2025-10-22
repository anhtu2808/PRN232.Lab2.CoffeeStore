-- Tắt khóa ngoại tạm thời
EXEC sp_msforeachtable @command1="ALTER TABLE ? NOCHECK CONSTRAINT ALL";

-- Xóa dữ liệu các bảng bằng DELETE với điều kiện giả (giữ lại bản ghi có Id = giá trị cố định)
DELETE FROM OrderDetails WHERE OrderDetailId <> -1; -- vì không có id = -1 nên xóa hết
DELETE FROM Payments WHERE PaymentId <> 0;           -- vì không có id = 0 nên xóa hết
DELETE FROM Orders WHERE OrderId <> 0;
DELETE FROM RefreshTokens WHERE Id <> '00000000-0000-0000-0000-000000000000';
DELETE FROM InvalidTokens WHERE TokenId <> -1;
DELETE FROM Products WHERE ProductId <> 0;
DELETE FROM Categories WHERE CategoryId <> 0;
DELETE FROM Users WHERE UserId <> '00000000-0000-0000-0000-000000000000';

-- Bật lại khóa ngoại
EXEC sp_msforeachtable @command1="ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL";

-- Reset identity các bảng có IDENTITY
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
DBCC CHECKIDENT ('Payments', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);

    
GO 


--------------------------------------------------
-- SECTION 2: DATA SEEDING WITH FIXED IDs AND IDENTITY_INSERT
--------------------------------------------------

PRINT 'Seeding data with fixed IDs and identity inserts...';

-- Seed Users with fixed GUIDs (không cần IDENTITY_INSERT)
INSERT INTO Users (UserId, Username, Email, PasswordHash)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'john.doe', 'john.doe@example.com', 'hashed_password_1_placeholder'),
    ('22222222-2222-2222-2222-222222222222', 'jane.smith', 'jane.smith@example.com', 'hashed_password_2_placeholder'),
    ('33333333-3333-3333-3333-333333333333', 'admin.user', 'admin@example.com', 'hashed_password_3_placeholder'),
    ('44444444-4444-4444-4444-444444444444', 'test.user1', 'test1@example.com', 'hashed_password_4_placeholder'),
    ('55555555-5555-5555-5555-555555555555', 'test.user2', 'test2@example.com', 'hashed_password_5_placeholder');
GO

-- Seed Categories with fixed IDs and identity insert enabled
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (CategoryId, Name, Description)
VALUES
    (1, 'Espresso Drinks', 'Rich and intense coffee shots and drinks.'),
    (2, 'Brewed Coffee', 'Classic drip-brewed coffee selections.'),
    (3, 'Tea & Infusions', 'A variety of hot and iced teas.'),
    (4, 'Bakery & Pastries', 'Freshly baked goods to complement your drink.'),
    (5, 'Frappuccinos', 'Blended iced beverages with coffee or cream base.'),
    (6, 'Juices & Smoothies', 'Healthy and refreshing fruit-based drinks.'),
    (7, 'Sandwiches', 'Freshly made sandwiches for a quick meal.'),
    (8, 'Salads', 'Healthy green salads with various toppings.'),
    (9, 'Desserts', 'Cakes, cookies, and other sweet treats.'),
    (10, 'Bottled Drinks', 'Bottled water, juices, and sodas.');
SET IDENTITY_INSERT Categories OFF;
GO

-- Seed Products with fixed IDs and identity insert enabled
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (ProductId, Name, Description, Price, CategoryId, IsActive)
VALUES
    (1, 'Espresso', 'A single shot of our signature espresso.', 3.00, 1, 1),
    (2, 'Latte', 'Espresso with steamed milk and a light layer of foam.', 4.50, 1, 1),
    (3, 'Cappuccino', 'Espresso with a thick layer of milk foam.', 4.50, 1, 1),
    (4, 'Americano', 'Espresso shots topped with hot water.', 3.50, 1, 1),
    (5, 'House Drip Coffee', 'Our daily signature brewed coffee.', 2.75, 2, 1),
    (6, 'Cold Brew', 'Slow-steeped cold coffee for a smooth taste.', 4.00, 2, 1),
    (7, 'English Breakfast Tea', 'A classic black tea.', 3.00, 3, 1),
    (8, 'Green Tea', 'A light and refreshing green tea.', 3.00, 3, 1),
    (9, 'Croissant', 'Buttery and flaky, baked fresh daily.', 3.25, 4, 1),
    (10, 'Blueberry Muffin', 'A soft muffin packed with blueberries.', 3.50, 4, 1);
SET IDENTITY_INSERT Products OFF;
GO


-- Seed Orders
SET IDENTITY_INSERT Orders ON;
INSERT INTO Orders (OrderId, UserId, OrderDate, Status, PaymentId)
VALUES
    (1, '11111111-1111-1111-1111-111111111111', DATEADD(day, -1, GETDATE()), 'Pending', NULL),
    (2, '22222222-2222-2222-2222-222222222222', DATEADD(day, -2, GETDATE()), 'Shipped', NULL),
    (3, '33333333-3333-3333-3333-333333333333', DATEADD(day, -3, GETDATE()), 'Cancelled', NULL),
    (4, '44444444-4444-4444-4444-444444444444', DATEADD(day, -4, GETDATE()), 'Completed', NULL),
    (5, '55555555-5555-5555-5555-555555555555', DATEADD(day, -5, GETDATE()), 'Pending', NULL);
SET IDENTITY_INSERT Orders OFF;
GO

-- Seed OrderDetails
SET IDENTITY_INSERT OrderDetails ON;
INSERT INTO OrderDetails (OrderDetailId, OrderId, ProductId, Quantity, UnitPrice)
VALUES
    (1, 1, 1, 1, (SELECT Price FROM Products WHERE ProductId = 1)),
    (2, 1, 2, 2, (SELECT Price FROM Products WHERE ProductId = 2)),
    (3, 2, 3, 1, (SELECT Price FROM Products WHERE ProductId = 3)),
    (4, 2, 4, 1, (SELECT Price FROM Products WHERE ProductId = 4)),
    (5, 3, 5, 3, (SELECT Price FROM Products WHERE ProductId = 5));
SET IDENTITY_INSERT OrderDetails OFF;
GO

-- Seed Payments
SET IDENTITY_INSERT Payments ON;
INSERT INTO Payments (PaymentId, OrderId, Amount, Status, PaymentDate, PaymentMethod)
VALUES
    (1, 1, (SELECT SUM(Quantity*UnitPrice) FROM OrderDetails WHERE OrderId = 1), 'PENDING', DATEADD(day, -1, GETDATE()), 'Credit Card'),
    (2, 2, (SELECT SUM(Quantity*UnitPrice) FROM OrderDetails WHERE OrderId = 2), 'PENDING', DATEADD(day, -2, GETDATE()), 'PayPal'),
    (3, 3, (SELECT SUM(Quantity*UnitPrice) FROM OrderDetails WHERE OrderId = 3), 'PENDING', DATEADD(day, -3, GETDATE()), 'Cash'),
    (4, 4, 0, 'PENDING', DATEADD(day, -4, GETDATE()), 'Credit Card'),
    (5, 5, 0, 'PENDING', DATEADD(day, -5, GETDATE()), 'PayPal');
SET IDENTITY_INSERT Payments OFF;
GO

-- Update Orders to link to Payments
UPDATE Orders SET PaymentId = 1 WHERE OrderId = 1;
UPDATE Orders SET PaymentId = 2 WHERE OrderId = 2;
UPDATE Orders SET PaymentId = 3 WHERE OrderId = 3;
UPDATE Orders SET PaymentId = 4 WHERE OrderId = 4;
UPDATE Orders SET PaymentId = 5 WHERE OrderId = 5;
GO



     