using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMVC.Models;
using SalesWebMVC.Services;
using SalesWebMVC.Models.ViewModels;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var list = _sellerService.FindAll();
            return View(list);
        }

        public IActionResult Create()
        {
            //Esse método abre o formulario para cadastrar o vendendor.
            var departments = _departmentService.FindAll();
            var viewModel = new SellerViewModel { Departments = departments };
            return View(viewModel);
        }
        [HttpPost]// Informa que é uma ação de POST e não de GET
        [ValidateAntiForgeryToken] //Para previnir ataque(quando alguem utiliza seu acesso e envia dados maliciosos) aproveitando sua autenticacao
        public IActionResult Create(Seller seller)
        {
            _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id) // o sinal de interrogacao é opcional, para indicar que passar o parametro Id é opcional
        {
            if (id == null) { return NotFound(); }

            var obj = _sellerService.FindById(id.Value);
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }
        [HttpPost]// Informa que é uma ação de POST e não de GET
        [ValidateAntiForgeryToken] //Para previnir ataque(quando alguem utiliza seu acesso e envia dados maliciosos) aproveitando sua autenticacao
        public IActionResult Delete(int id)
        {
            _sellerService.Remove(id);

            return RedirectToAction(nameof(Index));
        }
    }
}