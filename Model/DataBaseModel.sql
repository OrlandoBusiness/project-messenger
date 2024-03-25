CREATE DATABASE StorageDB
GO
USE StorageDB
GO

CREATE TABLE Measure
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE NOT NULL
)

CREATE TABLE Product
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE NOT NULL,
	[MeasureId] int FOREIGN KEY REFERENCES Measure(Id) NOT NULL,
	[Price] money CHECK(Price > 0),
	[LifeInDays] int CHECK(LifeInDays > 0)
)

CREATE TABLE Supplier
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE NOT NULL,
	[Password] nvarchar(100) CHECK([Password] != N'') UNIQUE NOT NULL,
	[Rating] float CHECK(Rating > 0)
)

CREATE TABLE ProductSupplier
(
	[Id] int IDENTITY PRIMARY KEY,
	[ProductId] int FOREIGN KEY REFERENCES Product(Id) NOT NULL,
	[SupplierId] int FOREIGN KEY REFERENCES Supplier(Id) NOT NULL
)

CREATE TABLE Storage
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE NOT NULL,
	[Password] nvarchar(100) CHECK([Password] != N'') UNIQUE NOT NULL,
	[MaxCapacity] int CHECK(MaxCapacity > 0)
)

CREATE TABLE Supplie
(
	[Id] int IDENTITY PRIMARY KEY,
	[ProductId] int FOREIGN KEY REFERENCES Product(Id) NOT NULL,
	[SupplierId] int FOREIGN KEY REFERENCES Supplier(Id) NOT NULL,
	[StorageId] int FOREIGN KEY REFERENCES Storage(Id) NOT NULL,
	[Count] int CHECK([Count] > 0) NOT NULL,
	[Price] money CHECK(Price > 0) NOT NULL,
	[ShippingDate] datetime CHECK(ShippingDate <= GETDATE()) NOT NULL,
    [ArrivalDate] datetime NOT NULL,

	CHECK(ArrivalDate > ShippingDate)
)

CREATE TABLE StorageProduct
(
	[Id] int IDENTITY PRIMARY KEY,
	[StorageId] int FOREIGN KEY REFERENCES Storage(Id) NOT NULL,
	[ProductId] int FOREIGN KEY REFERENCES Product(Id) NOT NULL,
	[CountOfProduct] int CHECK(CountOfProduct > 0)
)

CREATE TABLE [Order]
(
	[Id] int IDENTITY PRIMARY KEY,
	[ProductId] int FOREIGN KEY REFERENCES Product(Id) NOT NULL,
	[Count] int CHECK([Count] > 0) NOT NULL,
	[IsActive] bit DEFAULT 1
)