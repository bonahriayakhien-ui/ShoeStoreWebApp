using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoeStoreWebApp.Data;
using ShoeStoreWebApp.Models;

namespace ShoeStoreWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🛒 Thêm vào giỏ
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            var cart = GetCart();

            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item == null)
            {
                cart.Add(new CartItem()
                {
                    ProductId = id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                item.Quantity++;
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // 📄 Xem giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // =====================
        // HÀM LƯU VÀ LẤY SESSION
        // =====================
        private List<CartItem> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart == null)
                return new List<CartItem>();

            return JsonConvert.DeserializeObject<List<CartItem>>(sessionCart) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        // ➕ Tăng số lượng
        public IActionResult Increase(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // ➖ Giảm số lượng
        public IActionResult Decrease(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);

                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // ❌ Xóa sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
        // ============================
        // 📌 CHECKOUT
        // ============================

        // Nhập thông tin khách hàng
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            return View();
        }

        // Lưu đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(string customerName, string phone, string address)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            // Tạo đơn hàng mới
            var order = new Order
            {
                CustomerName = customerName,
                Phone = phone,
                Address = address,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Xóa giỏ hàng
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            return View();
        }

    }
}
