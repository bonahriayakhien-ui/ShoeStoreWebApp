using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeStoreWebApp.Data;
using ShoeStoreWebApp.Models;
using System.Diagnostics;

namespace ShoeStoreWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            var products = _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)              
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.Products = products;

            return View();
        }

        // ➕ Privacy Action đã được bổ sung
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Chính sách bảo mật";
            return View();
        }

        // Giữ nguyên Error để tránh lỗi 500
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
