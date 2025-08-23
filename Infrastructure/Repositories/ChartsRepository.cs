using Application.DTOs;
using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ChartsRepository:IChartsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ChartsRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductSales>> GetGetTopProductsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT TOP 5 
                p.ProductName,
                SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalSales
            FROM [Order Details] od
            JOIN Products p ON od.ProductID = p.ProductID
            GROUP BY p.ProductName
            ORDER BY TotalSales DESC;";

            var topProducts = connection.Query<ProductSales>(sql).ToList();

            return topProducts; 
        }

        public async Task<IEnumerable<MonthlySales>> GetMonthlySalesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT DATENAME(MONTH, o.OrderDate) AS MonthName,
                   MONTH(o.OrderDate) AS MonthNumber,
                   SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalSales
            FROM Orders o
            JOIN [Order Details] od ON o.OrderID = od.OrderID
            GROUP BY DATENAME(MONTH, o.OrderDate), MONTH(o.OrderDate)
            ORDER BY MonthNumber;";

            var monthlySales = await connection.QueryAsync<MonthlySales>(sql);

            return monthlySales.ToList();
        }
    }
}
