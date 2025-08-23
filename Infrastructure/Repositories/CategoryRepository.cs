using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public CategoryRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddAsync(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Categories (CategoryName, Description)
                VALUES (@CategoryName, @Description);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var newId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                category.CategoryName,
                category.Description
            });

            return newId;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT CategoryID as CategoryId, CategoryName, Description FROM Categories";
            return await connection.QueryAsync<Category>(sql);
        }
    }

}
