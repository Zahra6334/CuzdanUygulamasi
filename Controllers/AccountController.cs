using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using CuzdanUygulamasi.Models.ViewModels;
using CuzdanUygulamasi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IKullaniciServisi _kullaniciServisi;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        // Statik kullanıcı listesi örnek amaçlı
        private static List<Kullanici> kullanicilar = new List<Kullanici>();

        public AccountController(IKullaniciServisi kullaniciServisi,ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _kullaniciServisi = kullaniciServisi;
            _context = context;
            logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _kullaniciServisi.FindByEmailAndPasswordAsync(model.KullaniciAdi, model.Sifre);
            if (user == null)
            {
                ModelState.AddModelError("", "Email veya şifre hatalı");
                return View(model);
            }

            // Kullanıcı claims oluştur
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // UserId
                   // Kullanıcı adı
        new Claim(ClaimTypes.Email, user.Email)                  // Email
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Tarayıcı kapansa bile cookie saklanır
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Cookie süresi
            };

            // Kullanıcıyı login et
            await HttpContext.SignInAsync(
     CookieAuthenticationDefaults.AuthenticationScheme,
     new ClaimsPrincipal(claimsIdentity),
     authProperties
 );


            return RedirectToAction("Details", "Kullanici", new { id = user.Id });
        }


        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("AccountController -> Register çalıştı.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Öncelikle email kontrolünü veritabanından yapabilirsin, değilse statik listeden yapıyorsan:
                if (kullanicilar.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Bu email zaten kayıtlı.");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    AdSoyad = model.Ad + " " + model.Soyad,
                    Email = model.Email,
                    SifreHash = model.Sifre,
                    KayitTarihi = DateTime.Now
                };

                // Şifreyi hashle
                var passwordHasher = new PasswordHasher<Kullanici>();
                kullanici.SifreHash = passwordHasher.HashPassword(kullanici, model.Sifre);

                // Veritabanına kaydet
                await _kullaniciServisi.KullaniciOlusturAsync(kullanici);

                // Test amaçlı statik listeye ekle
                kullanicilar.Add(kullanici);

                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return Json(new { success = false, message = "Email adresi gereklidir" });
            }

            var user = await _kullaniciServisi.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }

            var token = await _kullaniciServisi.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "Account",
                new { userId = user.Id, token = token },
                protocol: HttpContext.Request.Scheme);

            // Burada email gönderimi yapılmalı
            // await _emailService.SendPasswordResetEmail(user.Email, resetLink);

            return Json(new { success = true, message = "Şifre sıfırlama bağlantısı gönderildi" });
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new ResetPasswordViewModel
            {
                Id = userId,
                Token = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _kullaniciServisi.FindByIdAsync(model.Id);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View(model);
            }

            var result = await _kullaniciServisi.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            _logger.LogInformation("AccountController -> ResetPasswordConfirmation çalıştı.");
            return View();
        }
    }
}
