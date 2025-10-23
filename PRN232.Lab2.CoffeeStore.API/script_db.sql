-- Drop database if exists
IF DB_ID('CoffeeStoreDB2') IS NOT NULL
BEGIN
        ALTER DATABASE CoffeeStoreDB2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE CoffeeStoreDB2;
END
GO

-- Create database
CREATE DATABASE CoffeeStoreDB2;
GO
USE CoffeeStoreDB2;
GO

--------------------------------------------------
-- SECTION 1: TABLE CREATION (parent tables BEFORE child tables)
--------------------------------------------------

-- 1. Users Table
CREATE TABLE Users
(
    UserId       UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username     NVARCHAR(100) NOT NULL UNIQUE,
    Email        NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role         NVARCHAR(50) NOT NULL DEFAULT 'Customer',
    CreatedDate  DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- 2. Categories Table
CREATE TABLE Categories
(
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- 3. Products Table
CREATE TABLE Products
(
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    CategoryId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories(CategoryId)
        ON DELETE SET NULL
);
GO

-- 4. Orders Table (UserId đồng bộ kiểu UNIQUEIDENTIFIER)
CREATE TABLE Orders
(
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,
    PaymentId INT NULL,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId)
        ON DELETE CASCADE
);
GO

-- 5. Payments Table
CREATE TABLE Payments
(
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    PaymentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(OrderId)
        ON DELETE CASCADE
);
GO

-- Add FK PaymentId to Orders after Payments table exists
ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_Payments FOREIGN KEY (PaymentId)
        REFERENCES Payments(PaymentId);
GO

-- 6. OrderDetails Table
CREATE TABLE OrderDetails
(
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(OrderId)
        ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductId)
        REFERENCES Products(ProductId)
        ON DELETE NO ACTION
);
GO

-- 7. RefreshTokens Table
CREATE TABLE RefreshTokens
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

--------------------------------------------------
-- SECTION 2: DATA SEEDING (SAMPLE DATA)
--------------------------------------------------

USE CoffeeStoreDB2;
GO

-- Tắt khóa ngoại tạm thời
EXEC sp_msforeachtable @command1="ALTER TABLE ? NOCHECK CONSTRAINT ALL";

-- Xóa dữ liệu các bảng với điều kiện giữ lại user 3 và 4
DELETE FROM OrderDetails WHERE OrderDetailId <> -1;
DELETE FROM Payments WHERE PaymentId <> 0;
DELETE FROM Orders WHERE OrderId <> 0;
DELETE FROM RefreshTokens WHERE Id <> '00000000-0000-0000-0000-000000000000';
DELETE FROM Products WHERE ProductId <> 0;
DELETE FROM Categories WHERE CategoryId <> 0;
DELETE FROM Users WHERE UserId NOT IN ('33333233-3333-3333-3333-333333333333');

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

-- Seed Users thêm nhiều bản ghi (đã giữ 3,4 trước đó)
INSERT INTO Users (UserId, Username, Email, PasswordHash, Role)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'john.doe', 'john.doe@example.com', 'hashed_password_1_placeholder', 'Customer'),
    ('22222222-2222-2222-2222-222222222222', 'jane.smith', 'jane.smith@example.com', 'hashed_password_2_placeholder', 'Customer'),
    ('33333333-3333-3333-3333-333333333333', 'admin', 'admin@example.com', '$2a$11$V4.bg/xR46aDiQESlwZkduiwpEItOzhIlCJw/5hBu00PMjmCjUlya', 'Admin'),
    ('44444444-4444-4444-4444-444444444444', 'anhtu', 'anhtu@example.com', '$2a$11$c1y.lGkRgSLJRtRuOCZn7OXwA6lEZJdUuK3PlwIXtX.ZO0urrVr9S' , 'Customer'),
    ('55555555-5555-5555-5555-555555555555', 'test.user2', 'test2@example.com', 'hashed_password_5_placeholder', 'Customer'),
    ('66666666-6666-6666-6666-666666666666', 'extra.user1', 'extra1@example.com', 'hashed_password_extra_1', 'Customer'),
    ('77777777-7777-7777-7777-777777777777', 'extra.user2', 'extra2@example.com', 'hashed_password_extra_2', 'Customer'),
    ('88888888-8888-8888-8888-888888888888', 'extra.user3', 'extra3@example.com', 'hashed_password_extra_3', 'Customer'),
    ('99999999-9999-9999-9999-999999999999', 'extra.user4', 'extra4@example.com', 'hashed_password_extra_4', 'Customer'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'extra.user5', 'extra5@example.com', 'hashed_password_extra_5', 'Customer');
GO

-- Seed Categories
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
    (10, 'Bottled Drinks', 'Bottled water, juices, and sodas.'),
    (11, 'Smoothies', 'Fresh and cool blended fruit drinks.'),
    (12, 'Specialty Coffee', 'Unique coffee blends.'),
    (13, 'Herbal Tea', 'Relaxing herbal infusion teas.'),
    (14, 'Sweets', 'Various sweets and snacks.'),
    (15, 'Healthy Snacks', 'Light and healthy snack options.'),
    (16, 'Hot Chocolate', 'Warm chocolate drinks.'),
    (17, 'Seasonal Specials', 'Limited time offers and seasonal drinks.'),
    (18, 'Sodas', 'Carbonated drinks.'),
    (19, 'Energy Drinks', 'High energy beverages.'),
    (20, 'Water', 'Still and sparkling water.');
SET IDENTITY_INSERT Categories OFF;
GO

-- Seed Products
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
    (10, 'Blueberry Muffin', 'A soft muffin packed with blueberries.', 3.50, 4, 1),
    (11, 'Caramel Macchiato', 'Sweet caramel espresso drink.', 5.00, 12, 1),
    (12, 'Pumpkin Spice Latte', 'Seasonal favorite.', 5.50, 17, 1),
    (13, 'Herbal Mint Tea', 'Refreshing and soothing.', 3.50, 13, 1),
    (14, 'Chocolate Chip Cookie', 'Fresh baked cookies.', 2.00, 14, 1),
    (15, 'Granola Bar', 'Healthy and crunchy.', 1.50, 15, 1),
    (16, 'Hot Chocolate', 'Rich and creamy.', 4.00, 16, 1),
    (17, 'Energy Booster', 'Energy drink with caffeine.', 3.75, 19, 1),
    (18, 'Sparkling Water', 'Refreshing sparkling water.', 2.50, 20, 1),
    (19, 'Lemon Soda', 'Tart and fizzy.', 2.25, 18, 1),
    (20, 'Mango Smoothie', 'Sweet mango blended drink.', 4.75, 11, 1);
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
    (5, '55555555-5555-5555-5555-555555555555', DATEADD(day, -5, GETDATE()), 'Pending', NULL),
    (6, '11111111-1111-1111-1111-111111111111', DATEADD(day, -6, GETDATE()), 'Completed', NULL),
    (7, '22222222-2222-2222-2222-222222222222', DATEADD(day, -7, GETDATE()), 'Shipped', NULL),
    (8, '33333333-3333-3333-3333-333333333333', DATEADD(day, -8, GETDATE()), 'Cancelled', NULL),
    (9, '44444444-4444-4444-4444-444444444444', DATEADD(day, -9, GETDATE()), 'Completed', NULL),
    (10, '55555555-5555-5555-5555-555555555555', DATEADD(day, -10, GETDATE()), 'Pending', NULL),
    (11, '11111111-1111-1111-1111-111111111111', DATEADD(day, -11, GETDATE()), 'Shipped', NULL),
    (12, '22222222-2222-2222-2222-222222222222', DATEADD(day, -12, GETDATE()), 'Pending', NULL),
    (13, '33333333-3333-3333-3333-333333333333', DATEADD(day, -13, GETDATE()), 'Cancelled', NULL),
    (14, '44444444-4444-4444-4444-444444444444', DATEADD(day, -14, GETDATE()), 'Completed', NULL),
    (15, '55555555-5555-5555-5555-555555555555', DATEADD(day, -15, GETDATE()), 'Pending', NULL),
    (16, '11111111-1111-1111-1111-111111111111', DATEADD(day, -16, GETDATE()), 'Shipped', NULL),
    (17, '22222222-2222-2222-2222-222222222222', DATEADD(day, -17, GETDATE()), 'Pending', NULL),
    (18, '33333333-3333-3333-3333-333333333333', DATEADD(day, -18, GETDATE()), 'Cancelled', NULL),
    (19, '44444444-4444-4444-4444-444444444444', DATEADD(day, -19, GETDATE()), 'Completed', NULL),
    (20, '55555555-5555-5555-5555-555555555555', DATEADD(day, -20, GETDATE()), 'Pending', NULL);
SET IDENTITY_INSERT Orders OFF;
GO

-- Seed OrderDetails
SET IDENTITY_INSERT OrderDetails ON;
DECLARE @od INT = 1;
DECLARE @order INT = 1;
WHILE @od <= 40 -- ít nhất 40 dòng chi tiết để mỗi đơn có ~2 sản phẩm
    BEGIN
        INSERT INTO OrderDetails (OrderDetailId, OrderId, ProductId, Quantity, UnitPrice)
        VALUES (@od, @order, ((@od - 1) % 20) + 1, (@od % 3) + 1, (SELECT Price FROM Products WHERE ProductId = ((@od - 1) % 20) + 1));

        SET @od = @od + 1;
        SET @order = CASE WHEN @order < 20 THEN @order + 1 ELSE 1 END;
    END
SET IDENTITY_INSERT OrderDetails OFF;
GO

-- Seed Payments
SET IDENTITY_INSERT Payments ON;
DECLARE @p INT = 1;
DECLARE @o INT = 1;
WHILE @p <= 20
    BEGIN
        INSERT INTO Payments (PaymentId, OrderId, Amount, Status, PaymentDate, PaymentMethod)
        VALUES (
                   @p,
                   @o,
                   (SELECT SUM(Quantity * UnitPrice) FROM OrderDetails WHERE OrderId = @o),
                   CASE WHEN @o % 2 = 0 THEN 'COMPLETED' ELSE 'PENDING' END, -- 👈 xen kẽ
                   DATEADD(day, -@o, GETDATE()),
                   CASE @o % 3
                       WHEN 0 THEN 'Credit Card'
                       WHEN 1 THEN 'ZaloPay'
                       ELSE 'Cash'
                       END
               );

        SET @p += 1;
        SET @o += 1;
    END;
SET IDENTITY_INSERT Payments OFF;

GO

-- Cập nhật Orders liên kết Payments
UPDATE Orders SET PaymentId = OrderId WHERE PaymentId IS NULL;
GO

-- Seed RefreshTokens
INSERT INTO RefreshTokens (Id, UserId, Token, ExpiresAt, CreatedAt)
VALUES
    ('15c6657c-2916-433d-a33a-eb6e0283b5e4', '44444444-4444-4444-4444-444444444444', 'n7+k1R0XNvsBZtf3dMOh4jITutK/DP+vExzutDs2XVc7A6ebAY452syve+rao79DbzlM33VMGKr4ei/8Gfc95g==', '3000-10-29 12:31:03.937', '2025-10-22 12:31:03.937');
GO




