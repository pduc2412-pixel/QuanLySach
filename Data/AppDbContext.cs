using Microsoft.EntityFrameworkCore;
using QuanLySach.Models;

namespace QuanLySach.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Sach> Sachs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sach>().HasData(
            new Sach { Id = 1, TieuDe = "Lập trình C# cơ bản", TacGia = "Nguyễn Văn A", TheLoai = "Công nghệ", NamXuatBan = 2022, Gia = 120000, SoLuong = 15 },
            new Sach { Id = 2, TieuDe = "Cơ sở dữ liệu SQL Server", TacGia = "Trần Thị B", TheLoai = "Công nghệ", NamXuatBan = 2021, Gia = 150000, SoLuong = 10 },
            new Sach { Id = 3, TieuDe = "Dế Mèn phiêu lưu ký", TacGia = "Tô Hoài", TheLoai = "Thiếu nhi", NamXuatBan = 2019, Gia = 60000, SoLuong = 30 }
        );
    }
}
