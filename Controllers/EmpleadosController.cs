using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Filters;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;

namespace MvcOAuthEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;

        public EmpleadosController(ServiceEmpleados service) {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index() {
            List<Empleado> empleados = await service.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int id) {
            Empleado empleado = await service.FindEmpleadoAsync(id);
            return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Perfil() {
            Empleado perfil = await service.GetPerfilAsync();
            return View(perfil);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis() {
            List<Empleado> compis = await service.GetCompisAsync();
            return View(compis);
        }

        public async Task<IActionResult> EmpleadosOficio() {
            List<string> oficios = await service.GetOficiosAsync();
            ViewBag.Oficios = oficios;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio(int? incremento, List<string> oficios, string accion) {
            List<string> ofs = await service.GetOficiosAsync();
            ViewBag.Oficios = ofs;

            if (accion.ToLower() == "incrementar") {
                await service.UpdateEmpleadosOficiosAsync(incremento.Value, oficios);
            } 

            List<Empleado> empleados = await service.GetEmpleadosOficioAsync(oficios);
            return View(empleados);
        }
    }
}
