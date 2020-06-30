using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMVC.Models;
using SalesWebMVC.Services;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services.Exceptions;
using System.Diagnostics;

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

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync(); //await é uma indicação para esperar a achamada Async()
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            //Esse método abre o formulario para cadastrar o vendendor.
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerViewModel { Departments = departments };
            return View(viewModel);
        }
        [HttpPost]// Informa que é uma ação de POST e não de GET
        [ValidateAntiForgeryToken] //Para previnir ataque(quando alguem utiliza seu acesso e envia dados maliciosos) aproveitando sua autenticacao
        public async Task<IActionResult> Create(Seller seller)
        {
            if (!ModelState.IsValid)// essa é uma validação back-end para que o usuario preencha as informações corretas do formulário
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id) // o sinal de interrogacao é opcional, para indicar que passar o parametro Id é opcional
        {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }
            return View(obj);

        }
        [HttpPost]// Informa que é uma ação de POST e não de GET
        [ValidateAntiForgeryToken] //Para previnir ataque(quando alguem utiliza seu acesso e envia dados maliciosos) aproveitando sua autenticacao
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {

                return RedirectToAction(nameof(Error), new { message = e.Message });
            }

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); ; }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" }); ;
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); ; }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            List<Department> departments = await _departmentService.FindAllAsync();
            SellerViewModel viewModel = new SellerViewModel { Seller = obj, Departments = departments };
            return View(viewModel);
        }

        [HttpPost]// Informa que é uma ação de POST e não de GET
        [ValidateAntiForgeryToken] //Para previnir ataque(quando alguem utiliza seu acesso e envia dados maliciosos) aproveitando sua autenticacao
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid)// essa é uma validação back-end para que o usuario preencha as informações corretas do formulário
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id mismatch(não corresponde)" });
            }
            try
            {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e)
            {

                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            catch (DbConcurrencyException e)
            {

                return RedirectToAction(nameof(Error), new { message = e.Message });

            }

        }
        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                //Current é opcional por isso usamos o sinal de ?                                                                       
                //operador de coalescência nula ( ?? ) é um operador lógico que retorna o seu operando do lado direito quando o seu operador do lado esquerdo é null ou undefined
                //essa declação está pegando Id interno da requisição
            };
            return View(viewModel);
        }


    }
}
