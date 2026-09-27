-- Ek Query Mein Complete Setup
USE master;
GO

-- Your database name yahan daalo (jahan Students table hai)
USE StudentDB;  -- CHANGE THIS to your actual database name
GO

-- Create Table (CORRECTED)
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
CREATE TABLE dbo.Users (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'Admin',
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- Insert ADMIN ONLY
INSERT INTO dbo.Users (Username, Password, FullName, Email, Role) VALUES 
('admin', 'admin123', 'Administrator', 'admin@sms.com', 'Admin');

-- Show Results
SELECT 'Admin Setup Complete!' AS Status;
SELECT * FROM dbo.Users;