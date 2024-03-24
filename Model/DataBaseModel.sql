CREATE DATABASE StorageDB
GO
USE StorageDB
GO

CREATE TABLE Measure
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE
)

CREATE TABLE Product
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE,
	[MeasureId] int FOREIGN KEY REFERENCES Measure(Id),
	[Price] money CHECK(Price > 0),
	[LifeInDays] int CHECK(LifeInDays > 0)
)

CREATE TABLE Supplier
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE,
	[Rating] float CHECK(Rating > 0)
)

CREATE TABLE ProductSupplier
(
	[Id] int IDENTITY PRIMARY KEY,
	[ProductId] int FOREIGN KEY REFERENCES Product(Id),
	[SupplierId] int FOREIGN KEY REFERENCES Supplier(Id)
)

CREATE TABLE Storage
(
	[Id] int IDENTITY PRIMARY KEY,
	[Name] nvarchar(100) CHECK([Name] != N'') UNIQUE,
	[MaxCapacity] int CHECK(MaxCapacity > 0)
)

CREATE TABLE Supplies
(
	[Id] int IDENTITY PRIMARY KEY,
	[ProductId] int FOREIGN KEY REFERENCES Product(Id),
	[SupplierId] int FOREIGN KEY REFERENCES Supplier(Id),
	[StorageId] int FOREIGN KEY REFERENCES Storage(Id),
	[Count] int CHECK([Count] > 0),
	[Price] money CHECK(Price > 0),
	[ShippingDate] datetime CHECK(ShippingDate <= GETDATE()),
    [ArrivalDate] datetime,

	CHECK(ArrivalDate > ShippingDate)
)

CREATE TABLE StorageProduct
(
	[Id] int IDENTITY PRIMARY KEY,
	[StorageId] int FOREIGN KEY REFERENCES Storage(Id),
	[ProductId] int FOREIGN KEY REFERENCES Product(Id),
	[CountOfProduct] int CHECK(CountOfProduct > 0)
)
