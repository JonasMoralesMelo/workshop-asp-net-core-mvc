using SalesWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Services.Exceptions;

namespace SalesWebMVC.Services
{
    public class SellerService 
    {
        private readonly SalesWebMVCContext _context;

        public SellerService(SalesWebMVCContext context)
        {
            _context = context;
        }

        public List<Seller> FindAll()
        {
            return _context.Seller.ToList();
        }

        public void Insert(Seller obj)
        {
            _context.Add(obj);
            _context.SaveChanges();
        }
        public Seller FindById(int id)
        {
            //Include ele carrega os objeto associados ao objeto principal
            //ele irá fazer o JOIN das tabelas, assim ele busca tmb o Departamento do vendedor
            //O nome dessa ação é EAGER LOADING
            return _context.Seller.Include(obj=>obj.Department).FirstOrDefault(obj => obj.Id == id);
        }

        public void Remove(int id)
        {
            var obj = _context.Seller.Find(id);
            _context.Seller.Remove(obj);
            _context.SaveChanges();
        }

        public void Update(Seller obj)
        {
            //Any() Serve pra dizer se existe algum registro no banco de dados
            // com a condição que eu colocar.
            if(!_context.Seller.Any(x=>x.Id == obj.Id))
            {
                throw new NotFoundException("Id not found");
            }
            try
            {
                _context.Update(obj);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //Caso lance uma exceção DbUpdateConcurrencyException de nivel de dados acontecer,
                // a camada de servico irá lancar uma excecao na camada dela "DbConcurrencyException"
                //assim o controlador "SellersController" vai lhe dar somente com excecoes da camada de serviço.
                //essa é uma forma de respeitar a arquitetura MVC Controllador conversa com a camada de servico
                //excecoes do nível a dados são capturados pelo servico e relançadas
                // em exececoes de servico para o controlador.
                throw new DbConcurrencyException(e.Message);
            }

        }
    }
}
