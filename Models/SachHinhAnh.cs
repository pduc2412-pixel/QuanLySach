using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySach.Models;

[Table("SachHinhAnhs")]
public class SachHinhAnh
{
    public int Id { get; set; }

    public int SachId { get; set; }

    public string DuongDan { get; set; } = string.Empty;

    public Sach? Sach { get; set; }
}