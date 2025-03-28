using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;
using System.Security.Claims;

namespace MvcOAuthEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;

        public ManagedController(ServiceEmpleados service) {
            this.service = service;
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model) {
            string token = await this.service.GetTokenAsync(model.Apellido, model.IdEmpleado);

            if (token != null) {
                ViewBag.Mensaje = "Ya tienes tu token !!!";
                HttpContext.Session.SetString("Token", token);
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.Apellido));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.IdEmpleado.ToString()));
                identity.AddClaim(new Claim("Token", token));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                });
                return RedirectToAction("Index", "Empleados");
            }
            else {
                ViewBag.Mensaje = "Usuario o contraseña incorrectos";
                return View();
            }

            
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
