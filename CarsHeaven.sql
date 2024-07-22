USE pc_student;
CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Cars (
    CarId INT AUTO_INCREMENT PRIMARY KEY,
    CarName VARCHAR(255) NOT NULL,
    CarType TEXT,
	CarPic VARCHAR(255),
	Color VARCHAR(50),
    Seats INT,
    RentRate DECIMAL(10, 2) NOT NULL,
	Mileage INT,
	FuelType VARCHAR(50),
	Transmission VARCHAR(50),
	Description TEXT,
    AvailabilityStatus BOOLEAN DEFAULT TRUE,
    RegistrationNumber VARCHAR(100),
    YearOfManufacture YEAR,
    InsuranceDetails TEXT,
	LocationId INT,
    FOREIGN KEY (LocationId) REFERENCES CarsHeaven_Locations(LocationId)
);
ALTER TABLE pc_student.CarsHeaven_Cars ADD COLUMN BrandName VARCHAR(255) NOT NULL AFTER CarId;
TRUNCATE TABLE pc_student.CarsHeaven_Cars;
SELECT * FROM pc_student.CarsHeaven_Cars;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(15),
    UserPassword VARCHAR(100) NOT NULL,
    Address TEXT,
    RegistrationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLogin DATETIME,
    Role ENUM('User', 'Admin') NOT NULL DEFAULT 'User',
    ProfilePic VARCHAR(100),
    IsEmailVerified BOOLEAN NOT NULL DEFAULT FALSE,
    IsPhoneVerified BOOLEAN NOT NULL DEFAULT FALSE,
    UNIQUE (Email),
    UNIQUE (Phone)
);
SELECT * FROM pc_student.CarsHeaven_Users;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Wishlist (
    WishlistId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CarId INT NOT NULL,
    DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES CarsHeaven_Users(UserId),
    FOREIGN KEY (CarId) REFERENCES CarsHeaven_Cars(CarId)
);
INSERT INTO pc_student.CarsHeaven_Wishlist (UserId, CarId) VALUES (1, 2);
SELECT wl.WishlistId,u.UserId,u.UserName,u.Email,u.Phone,c.CarId,c.BrandName,c.CarName,c.CarType,c.Seats,c.RentRate,c.CarPic,wl.DateAdded
FROM pc_student.CarsHeaven_Wishlist wl
JOIN pc_student.CarsHeaven_Users u ON wl.UserId = u.UserId
JOIN pc_student.CarsHeaven_Cars c ON wl.CarId = c.CarId;
SELECT * FROM pc_student.CarsHeaven_Wishlist;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Rentals (
    RentalId INT AUTO_INCREMENT PRIMARY KEY,
    CarId INT,
    UserId INT,
    RentalStart DATE,
    RentalEnd DATE,
    TotalPrice DECIMAL(10, 2),
    PaymentStatus BOOLEAN DEFAULT FALSE,
	DriverId INT,
    FOREIGN KEY (CarId) REFERENCES CarsHeaven_Cars(CarId),
    FOREIGN KEY (UserId) REFERENCES CarsHeaven_Users(UserId),
	FOREIGN KEY (DriverId) REFERENCES CarsHeaven_Drivers(DriverId)
);
SELECT * FROM pc_student.CarsHeaven_Rentals;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Payments (
    PaymentId INT AUTO_INCREMENT PRIMARY KEY,
    RentalId INT,
    PaymentDate DATE,
    Amount DECIMAL(10, 2),
    PaymentMethod VARCHAR(50),
    FOREIGN KEY (RentalId) REFERENCES CarsHeaven_Rentals(RentalId)
);
SELECT * FROM pc_student.CarsHeaven_Payments;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Locations (
    LocationId INT AUTO_INCREMENT PRIMARY KEY,
    LocationName VARCHAR(255) NOT NULL,
    Address VARCHAR(255) NOT NULL,
    City VARCHAR(255),
    State VARCHAR(255),
    ZipCode VARCHAR(10)
);
TRUNCATE TABLE pc_student.CarsHeaven_Locations;
SELECT * FROM pc_student.CarsHeaven_Locations;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Drivers (
    DriverId INT AUTO_INCREMENT PRIMARY KEY,
    DriverName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE,
    Phone VARCHAR(15),
    DriverPic LONGTEXT,
    DriverAge INT,
    LicenseNumber VARCHAR(100) NOT NULL UNIQUE,
    Address VARCHAR(255),
    Rating DECIMAL(2, 1) CHECK (Rating >= 1 AND Rating <= 5)
);
SELECT * FROM pc_student.CarsHeaven_Drivers;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Feedback (
    FeedbackId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    CarId INT,
	UserName VARCHAR(255) NOT NULL,
	Email VARCHAR(255) NOT NULL,
    Phone VARCHAR(255) NOT NULL,
	Address VARCHAR(255) NOT NULL,
    Topic VARCHAR(255) NOT NULL,
    Message VARCHAR(255) NOT NULL,
	Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    SentOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	ReadStatus ENUM('Read', 'Unread') NOT NULL DEFAULT 'Unread',
    FOREIGN KEY (UserId) REFERENCES CarsHeaven_Users(UserId),
    FOREIGN KEY (CarId) REFERENCES CarsHeaven_Cars(CarId)
);
TRUNCate Table pc_student.CarsHeaven_Feedback;
SELECT * FROM pc_student.CarsHeaven_Feedback;

CREATE TABLE IF NOT EXISTS pc_student.CarsHeaven_Sessions (
    SessionId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UserStatus ENUM('Active', 'Inactive') NOT NULL DEFAULT 'Active',
    Token LONGTEXT NOT NULL,
    UserId VARCHAR(255) NOT NULL
);
SELECT * FROM pc_student.CarsHeaven_Sessions;