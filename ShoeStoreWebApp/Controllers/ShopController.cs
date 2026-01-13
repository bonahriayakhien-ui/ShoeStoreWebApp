using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ShoeStoreWebApp.Data;
using ShoeStoreWebApp.Models;

namespace ShoeStoreWebApp.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===============================
        // DANH SÁCH SẢN PHẨM + TÌM KIẾM + LỌC
        // ===============================
        public IActionResult Index(string? search, int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    (p.Description != null && p.Description.Contains(search))
                );

                ViewBag.Search = search;
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);

                ViewBag.CategoryName = _context.Categories
                    .Where(c => c.Id == categoryId.Value)
                    .Select(c => c.Name)
                    .FirstOrDefault();
            }

            return View(query.ToList());
        }

        // ===============================
        // CHI TIẾT SẢN PHẨM
        // ===============================
        public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Replies)
                        .ThenInclude(r => r.User)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }


        // ===============================
        // THÊM BÌNH LUẬN (COMMENT)
        // ===============================
        [HttpPost]
        [Authorize]
        public IActionResult AddComment(
            int productId,
            string content,
            int rating,
            int? parentCommentId)
        {
            var comment = new Comment
            {
                ProductId = productId,
                Content = content,
                Rating = rating,
                ParentCommentId = parentCommentId,
                UserId = _userManager.GetUserId(User),
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = productId });
        }
        [HttpPost]
        [Authorize]
        public IActionResult DeleteComment(int id, int productId)
        {
            var comment = _context.Comments
                .Include(c => c.Replies)
                .FirstOrDefault(c => c.Id == id);

            if (comment == null)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            // 🔐 Chỉ cho phép: chủ comment hoặc Admin
            if (comment.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            // ❗ Xóa reply trước
            if (comment.Replies != null && comment.Replies.Any())
            {
                _context.Comments.RemoveRange(comment.Replies);
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = productId });
        }


    }
}
