using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhatTrienWebTraCuuvaChoThue.Models;

namespace PhatTrienWebTraCuuvaChoThue.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: Users/Register
        public IActionResult Register() => View();

        // POST: Users/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (exists)
                {
                    ModelState.AddModelError("", "Email đã được sử dụng.");
                    return View(user);
                }

                // ✅ Mã hoá mật khẩu trước khi lưu
                user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Users/Login
        public IActionResult Login() => View();

        // POST: Users/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // ✅ Mã hoá mật khẩu nhập vào để so sánh
            string hashedPassword = PasswordHelper.HashPassword(password);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.Role ?? "User");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            return user == null ? NotFound() : View(user);
        }
        // GET: Users/Create (Admin)
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login");

            return View();
        }

        // POST: Users/Create (Admin thêm người dùng)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                bool exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (exists)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(user);
                }

                user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Users"); 
            }
            return View(user);
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login");
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            return user == null ? NotFound() : View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Role")] User user)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login");
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Users.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.FullName = user.FullName;
                    existing.Email = user.Email;
                    existing.Role = user.Role;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login");
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            return user == null ? NotFound() : View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Users/ForgotPassword
        public IActionResult ForgotPassword() => View();

        // POST: Users/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Email không tồn tại.";
                return View();
            }

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Mật khẩu đã được cập nhật. Vui lòng đăng nhập lại.";
            return View();
        }

    }
}
