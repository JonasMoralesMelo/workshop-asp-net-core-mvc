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

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Seller.ToListAsync();
        }

        // public void Insert(Seller obj)
        //{
        //  _context.Add(obj);
        //_context.SaveChanges();
        //}

        public async Task InsertAsync(Seller obj)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Seller.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id);
        }
        //public Seller FindById(int id)
        //{
        //Include ele carrega os objeto associados ao objeto principal
        //ele irá fazer o JOIN das tabelas, assim ele busca tmb o Departamento do vendedor
        //O nome dessa ação é EAGER LOADING
        //  return _context.Seller.Include(obj => obj.Department).FirstOrDefault(obj => obj.Id == id);
        //}

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Seller.FindAsync(id);
            _context.Seller.Remove(obj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seller obj)
        {
            //Any() Serve pra dizer se existe algum registro no banco de dados
            // com a condição que eu colocar.
            bool hasAny = await _context.Seller.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }
            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
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
