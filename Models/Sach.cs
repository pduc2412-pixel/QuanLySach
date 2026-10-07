using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLySach.Models;

public class Sach
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
    [StringLength(200)]
    [Display(Name = "Tiêu đề")]
    public string TieuDe { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tác giả")]
    [StringLength(100)]
    [Display(Name = "Tác giả")]
    public string TacGia { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Thể loại")]
    public string? TheLoai { get; set; }

    [Range(1000, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
    [Display(Name = "Năm xuất bản")]
    public int NamXuatBan { get; set; } = DateTime.Now.Year;

    [Range(0, 1000000000, ErrorMessage = "Giá không hợp lệ")]
    [Column(TypeName = "decimal(18,0)")]
    [Display(Name = "Giá")]
    public decimal Gia { get; set; }

    [Range(0, 100000, ErrorMessage = "Số lượng không hợp lệ")]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; }

    [ValidateNever]
    public List<SachHinhAnh> HinhAnhs { get; set; } = new();
}
