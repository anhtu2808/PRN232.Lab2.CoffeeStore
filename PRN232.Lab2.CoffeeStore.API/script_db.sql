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

PRINT 'Seeding data...';

-- Seed Users (NEW)
-- Note: Passwords here are plain text for example purposes ONLY.
-- In a real application, you MUST hash and salt passwords.
INSERT INTO Users (Username, Email, PasswordHash)
VALUES ('john.doe', 'john.doe@example.com', 'hashed_password_1_placeholder'),
       ('jane.smith', 'jane.smith@example.com', 'hashed_password_2_placeholder'),
       ('admin.user', 'admin@example.com', 'hashed_password_3_placeholder'),
       ('test.user1', 'test1@example.com', 'hashed_password_4_placeholder'),
       ('test.user2', 'test2@example.com', 'hashed_password_5_placeholder');
GO

-- Seed Categories
INSERT INTO Categories (Name, Description) VALUES
                                               ('Espresso Drinks', 'Rich and intense coffee shots and drinks.'),
                                               ('Brewed Coffee', 'Classic drip-brewed coffee selections.'),
                                               ('Tea & Infusions', 'A variety of hot and iced teas.'),
                                               ('Bakery & Pastries', 'Freshly baked goods to complement your drink.'),
                                               ('Frappuccinos', 'Blended iced beverages with coffee or cream base.'),
                                               ('Juices & Smoothies', 'Healthy and refreshing fruit-based drinks.'),
                                               ('Sandwiches', 'Freshly made sandwiches for a quick meal.'),
                                               ('Salads', 'Healthy green salads with various toppings.'),
                                               ('Desserts', 'Cakes, cookies, and other sweet treats.'),
                                               ('Bottled Drinks', 'Bottled water, juices, and sodas.');
-- Add more to reach 20+
INSERT INTO Categories (Name, Description)
SELECT TOP 15 'Category ' + CAST(ROW_NUMBER() OVER (ORDER BY object_id) AS VARCHAR), 'Sample category description'
FROM sys.all_objects;
GO

-- Seed Products
INSERT INTO Products (Name, Description, Price, CategoryId, IsActive) VALUES
                                                                          ('Espresso', 'A single shot of our signature espresso.', 3.00, 1, 1),
                                                                          ('Latte', 'Espresso with steamed milk and a light layer of foam.', 4.50, 1, 1),
                                                                          ('Cappuccino', 'Espresso with a thick layer of milk foam.', 4.50, 1, 1),
                                                                          ('Americano', 'Espresso shots topped with hot water.', 3.50, 1, 1),
                                                                          ('House Drip Coffee', 'Our daily signature brewed coffee.', 2.75, 2, 1),
                                                                          ('Cold Brew', 'Slow-steeped cold coffee for a smooth taste.', 4.00, 2, 1),
                                                                          ('English Breakfast Tea', 'A classic black tea.', 3.00, 3, 1),
                                                                          ('Green Tea', 'A light and refreshing green tea.', 3.00, 3, 1),
                                                                          ('Croissant', 'Buttery and flaky, baked fresh daily.', 3.25, 4, 1),
                                                                          ('Blueberry Muffin', 'A soft muffin packed with blueberries.', 3.50, 4, 1),
                                                                          ('Caramel Frappuccino', 'Coffee blended with caramel, milk, and ice.', 5.50, 5, 1),
                                                                          ('Mocha Cookie Crumble', 'Mocha sauce, Frappuccino chips, blended with coffee, milk and ice.', 5.75, 5, 1),
                                                                          ('Orange Juice', 'Freshly squeezed orange juice.', 4.00, 6, 1),
                                                                          ('Strawberry Smoothie', 'A blend of strawberries, banana, and yogurt.', 5.00, 6, 1),
                                                                          ('Chicken Pesto Sandwich', 'Grilled chicken, pesto, and mozzarella on ciabatta.', 8.50, 7, 1),
                                                                          ('Turkey Club', 'Turkey, bacon, lettuce, and tomato on sourdough.', 9.00, 7, 1),
                                                                          ('Caesar Salad', 'Romaine lettuce, croutons, parmesan, and Caesar dressing.', 7.50, 8, 1),
                                                                          ('Greek Salad', 'Mixed greens with feta, olives, and vinaigrette.', 8.00, 8, 1),
                                                                          ('Chocolate Cake Slice', 'A rich and decadent slice of chocolate cake.', 5.00, 9, 1),
                                                                          ('Oatmeal Cookie', 'A classic chewy oatmeal cookie.', 2.50, 9, 1),
                                                                          ('Bottled Water', 'Still spring water.', 2.00, 10, 1),
                                                                          ('Sparkling Water', 'Carbonated mineral water.', 2.50, 10, 1);
GO

-- Seed Orders, Payments, and OrderDetails
-- This section uses a loop-like structure to create consistent related data.
DECLARE
@i INT = 1;
WHILE
@i <= 25
BEGIN
        DECLARE
@OrderId INT;
        DECLARE
@PaymentId INT;
        DECLARE
@OrderTotal DECIMAL(18, 2);
        -- MODIFIED: Use a valid UserId from the Users table we seeded.
        -- This will cycle through user IDs 1, 2, 3, 4, 5, 1, 2, ...
        DECLARE
@UserId INT = (@i - 1) % 5 + 1;
        DECLARE
@Status NVARCHAR(50) = CASE @i % 4 WHEN 0 THEN 'Completed' WHEN 1 THEN 'Pending' WHEN 2 THEN 'Shipped' ELSE 'Cancelled'
END;
        DECLARE
@PaymentMethod NVARCHAR(50) = CASE @i % 3 WHEN 0 THEN 'Credit Card' WHEN 1 THEN 'PayPal' ELSE 'Cash'
END;
        DECLARE
@OrderDate DATETIME2 = DATEADD(day, -(@i), GETDATE());

        -- Step 1: Create an Order
INSERT INTO Orders (UserId, OrderDate, Status, PaymentId)
VALUES (@UserId, @OrderDate, @Status, NULL);
SET
@OrderId = SCOPE_IDENTITY();

        -- Step 2: Create OrderDetails for this Order
INSERT INTO OrderDetails(OrderId, ProductId, Quantity, UnitPrice)
VALUES (@OrderId, (@i % 20) + 1, @i % 3 + 1, (SELECT Price FROM Products WHERE ProductId = (@i % 20) + 1)),
       (@OrderId, (@i % 15) + 5, 1, (SELECT Price FROM Products WHERE ProductId = (@i % 15) + 5));

-- Calculate the order total
SELECT @OrderTotal = SUM(Quantity * UnitPrice)
FROM OrderDetails
WHERE OrderId = @OrderId;

-- Step 3: Create a Payment for the Order
INSERT INTO Payments(OrderId, Amount, PaymentDate, PaymentMethod)
VALUES (@OrderId, @OrderTotal, @OrderDate, @PaymentMethod);
SET
@PaymentId = SCOPE_IDENTITY();

        -- Step 4: Update the Order with the PaymentId
UPDATE Orders
SET PaymentId = @PaymentId
WHERE OrderId = @OrderId;

SET
@i = @i + 1;
END;
GO

PRINT 'Database CoffeeStoreDB2 created and seeded successfully.';
GO




