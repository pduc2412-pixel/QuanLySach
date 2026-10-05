using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Data;
using QuanLySach.Models;

namespace QuanLySach.Controllers;

public class SachController : Controller
{
    private readonly AppDbContext _context;

    public SachController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? tuKhoa)
    {
        var query = _context.Sachs.AsQueryable();
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
        var sach = await _context.Sachs.FirstOrDefaultAsync(s => s.Id == id);
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
        if (!ModelState.IsValid) return View(sach);
        _context.Add(sach);
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Thêm sách thành công";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var sach = await _context.Sachs.FindAsync(id);
        if (sach == null) return NotFound();
        return View(sach);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sach sach)
    {
        if (id != sach.Id) return NotFound();
        if (!ModelState.IsValid) return View(sach);
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
        TempData["ThongBao"] = "Cập nhật sách thành công";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var sach = await _context.Sachs.FirstOrDefaultAsync(s => s.Id == id);
        if (sach == null) return NotFound();
        return View(sach);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sach = await _context.Sachs.FindAsync(id);
        if (sach != null)
        {
            _context.Sachs.Remove(sach);
            await _context.SaveChangesAsync();
        }
        TempData["ThongBao"] = "Xóa sách thành công";
        return RedirectToAction(nameof(Index));
    }
}
