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
    public class CategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddCategoryAsync(CategoryDto category)
        {
           return await _repository.AddAsync(new Category
            {
                CategoryName = category.CategoryName,
                Description = category.Description
            });
        }
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();
        }
    }
}
