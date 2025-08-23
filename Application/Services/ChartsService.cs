using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ChartsService
    {
        private readonly IChartsRepository _repository;

        public ChartsService(IChartsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductSalesDto>> GetGetTopProductsAsync()
        {
            var top = await _repository.GetGetTopProductsAsync();
            return top.Select(c => new ProductSalesDto
            {
                ProductName = c.ProductName,
                TotalSales = c.TotalSales
            }).ToList();


        }
        public async Task<List<MonthlySalesDto>> GetMonthlySalesAsync()
        {
            var categories = await _repository.GetMonthlySalesAsync();
            return categories.Select(c => new MonthlySalesDto
            {
                 MonthName = c.MonthName,
                MonthNumber = c.MonthNumber,
                TotalSales = c.TotalSales   
            }).ToList();
        }
    }
}
