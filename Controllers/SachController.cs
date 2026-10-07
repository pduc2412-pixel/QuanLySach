using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Data;
using QuanLySach.Models;

namespace QuanLySach.Controllers;

public class SachController : Controller
{
    private static readonly string[] DuoiChoPhep = { ".jpg", ".jpeg", ".png" };
    private static readonly string[] LoaiChoPhep = { "image/jpeg", "image/png" };
    private const long DungLuongToiDa = 5 * 1024 * 1024;

    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SachController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index(string? tuKhoa)
    {
        IQueryable<Sach> query = _context.Sachs.Include(s => s.HinhAnhs);
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            query = query.Where(s => s.TieuDe.Contains(tuKhoa) || s.TacGia.Contains(tuKhoa));
        }
        ViewData["TuKhoa"] = tuKhoa;
        return View(await query.OrderBy(s => s.Id).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var sach = await _context.Sachs.Include(s => s.HinhAnhs).FirstOrDefaultAsync(s => s.Id == id);
        if (sach == null) return NotFound();
        return View(sach);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sach sach)
    {
        var files = LayFileUpload();
        KiemTraFile(files);
        if (!ModelState.IsValid) return View(sach);
        _context.Add(sach);
        await _context.SaveChangesAsync();
        await LuuHinhAnhAsync(sach.Id, files);
        TempData["ThongBao"] = "Thêm sách thành công";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var sach = await _context.Sachs.Include(s => s.HinhAnhs).FirstOrDefaultAsync(s => s.Id == id);
        if (sach == null) return NotFound();
        return View(sach);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sach sach, List<int>? anhXoa)
    {
        if (id != sach.Id) return NotFound();
        var files = LayFileUpload();
        KiemTraFile(files);
        if (!ModelState.IsValid)
        {
            sach.HinhAnhs = await _context.HinhAnhs.AsNoTracking().Where(h => h.SachId == id).ToListAsync();
            return View(sach);
        }
        try
        {
            _context.Update(sach);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Sachs.AnyAsync(s => s.Id == id)) return NotFound();
            throw;
        }
        if (anhXoa != null && anhXoa.Count > 0)
        {
            var anhCanXoa = await _context.HinhAnhs.Where(h => h.SachId == id && anhXoa.Contains(h.Id)).ToListAsync();
            foreach (var anh in anhCanXoa)
            {
                XoaFileVatLy(anh.DuongDan);
            }
            _context.HinhAnhs.RemoveRange(anhCanXoa);
            await _context.SaveChangesAsync();
        }
        await LuuHinhAnhAsync(id, files);
        TempData["ThongBao"] = "Cập nhật sách thành công";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var sach = await _context.Sachs.Include(s => s.HinhAnhs).FirstOrDefaultAsync(s => s.Id == id);
        if (sach == null) return NotFound();
        return View(sach);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sach = await _context.Sachs.Include(s => s.HinhAnhs).FirstOrDefaultAsync(s => s.Id == id);
        if (sach != null)
        {
            foreach (var anh in sach.HinhAnhs)
            {
                XoaFileVatLy(anh.DuongDan);
            }
            _context.Sachs.Remove(sach);
            await _context.SaveChangesAsync();
        }
        TempData["ThongBao"] = "Xóa sách thành công";
        return RedirectToAction(nameof(Index));
    }

    private List<IFormFile> LayFileUpload()
    {
        return Request.Form.Files.GetFiles("files").Where(f => f.Length > 0).ToList();
    }

    private void KiemTraFile(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            var duoi = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!DuoiChoPhep.Contains(duoi) || !LoaiChoPhep.Contains(file.ContentType.ToLowerInvariant()))
            {
                ModelState.AddModelError(string.Empty, $"File {file.FileName} không hợp lệ, chỉ cho phép jpg hoặc png");
            }
            else if (file.Length > DungLuongToiDa)
            {
                ModelState.AddModelError(string.Empty, $"File {file.FileName} vượt quá dung lượng tối đa 5MB");
            }
        }
    }

    private async Task LuuHinhAnhAsync(int sachId, IEnumerable<IFormFile> files)
    {
        var thuMuc = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(thuMuc);
        foreach (var file in files)
        {
            var tenFile = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
            using (var stream = new FileStream(Path.Combine(thuMuc, tenFile), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            _context.HinhAnhs.Add(new SachHinhAnh { SachId = sachId, DuongDan = "/uploads/" + tenFile });
        }
        await _context.SaveChangesAsync();
    }

    private void XoaFileVatLy(string duongDan)
    {
        var duongDanFile = Path.Combine(_env.ContentRootPath, "wwwroot", duongDan.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(duongDanFile))
        {
            System.IO.File.Delete(duongDanFile);
        }
    }
}