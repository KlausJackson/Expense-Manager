-- create database qlct;
-- drop database qlct;

-- select qlct rồi chạy
CREATE TABLE categories (
    catID INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
	monthlySpend DECIMAL(10, 2) default null,
	userID int not null,
	foreign key (userID) references users(userID)
); 
-- vi du: do an, tien dien, tien nuoc, tien tieu vat, tien thue nha,...

CREATE TABLE users (
    userID int primary key identity(1,1),
	username nvarchar(30) not null,
	passwd nvarchar(100) not null
);
-- danh sach tai khoan

CREATE TABLE expenses (
    expID INT PRIMARY KEY IDENTITY(1,1),
	userID int not null,
    date DATE NOT NULL,
    description NVARCHAR(200),
    amount DECIMAL(10, 2) NOT NULL,
    catID INT null,
	foreign key (userID) references users(userID),
    FOREIGN KEY (catID) REFERENCES categories(catID)
);
-- cac khoan chi tieu

CREATE TABLE income (
    incID INT PRIMARY KEY IDENTITY(1,1),
	userID int not null,
	date date not null,
    amount DECIMAL(10, 2) NOT NULL,
    description NVARCHAR(200),
	foreign key (userID) references users(userID),
);
-- cac khoan thu nhap


delete from expenses --where userID = 5;
delete from categories
select * from categories;
select * from expenses;
select * from users
select * from income


-- chạy dòng này để lấy connectionString
SELECT 
    'Data Source=' + @@SERVERNAME + 
    ';Initial Catalog=' + DB_NAME() + 
    ';Integrated Security=True;' AS ConnectionString