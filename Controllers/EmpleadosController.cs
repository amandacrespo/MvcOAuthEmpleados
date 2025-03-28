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

        public async Task<IActionResult> Details(int id) {
            Empleado empleado = await service.FindEmpleadoAsync(id);
            return View(empleado);
        }
    }
}
