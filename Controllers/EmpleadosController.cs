﻿using Microsoft.AspNetCore.Mvc;
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
    }
}
