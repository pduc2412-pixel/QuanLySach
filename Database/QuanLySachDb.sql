IF DB_ID(N'QuanLySachDb') IS NULL
    CREATE DATABASE QuanLySachDb;
GO

USE QuanLySachDb;
GO

IF OBJECT_ID(N'dbo.Sachs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sachs (
        Id INT IDENTITY(1,1) NOT NULL,
        TieuDe NVARCHAR(200) NOT NULL,
        TacGia NVARCHAR(100) NOT NULL,
        TheLoai NVARCHAR(100) NULL,
        NamXuatBan INT NOT NULL,
        Gia DECIMAL(18,0) NOT NULL,
        SoLuong INT NOT NULL,
        CONSTRAINT PK_Sachs PRIMARY KEY (Id)
    );

    SET IDENTITY_INSERT dbo.Sachs ON;
    INSERT INTO dbo.Sachs (Id, TieuDe, TacGia, TheLoai, NamXuatBan, Gia, SoLuong) VALUES
        (1, N'Lập trình C# cơ bản', N'Nguyễn Văn A', N'Công nghệ', 2022, 120000, 15),
        (2, N'Cơ sở dữ liệu SQL Server', N'Trần Thị B', N'Công nghệ', 2021, 150000, 10),
        (3, N'Dế Mèn phiêu lưu ký', N'Tô Hoài', N'Thiếu nhi', 2019, 60000, 30);
    SET IDENTITY_INSERT dbo.Sachs OFF;
END
GO
GO

IF OBJECT_ID(N'dbo.SachHinhAnhs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SachHinhAnhs (
        Id INT IDENTITY(1,1) NOT NULL,
        SachId INT NOT NULL,
        DuongDan NVARCHAR(MAX) NOT NULL,
        CONSTRAINT PK_SachHinhAnhs PRIMARY KEY (Id),
        CONSTRAINT FK_SachHinhAnhs_Sachs_SachId FOREIGN KEY (SachId) REFERENCES dbo.Sachs (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_SachHinhAnhs_SachId ON dbo.SachHinhAnhs (SachId);
END
GO
