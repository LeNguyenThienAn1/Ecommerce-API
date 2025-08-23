using Application.DTOs;
using Application.Interfaces.Queries;
using Application.Interfaces.Services;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryQueries _categoryQueries;

        public CategoryService(ICategoryQueries categoryQueries)
        {
            _categoryQueries = categoryQueries;
        }

        // GetAll với filter 
        public async Task<List<CategoryInfoDto>> GetAllCategoriesAsync(CategoryFilterDto filter)
        {
            var categories = await _categoryQueries.GetAllCategoriesAsync();
            var result = new List<CategoryInfoDto>();
            foreach (var category in categories)
            {
                var categoryDto = new CategoryInfoDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                };
                result.Add(categoryDto);
            }
            return result;
        }

        // GetAll không filter (cho User)
        public async Task<List<CategoryInfoDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryQueries.GetAllCategoriesAsync();
            var result = new List<CategoryInfoDto>();

            foreach (var category in categories)
            {
                var categoryDto = new CategoryInfoDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                };
                result.Add(categoryDto);
            }

            return result;
        }

        // Get by Id
        public async Task<CategoryEntity> GetCategoryByIdAsync(Guid id)
        {
            return await _categoryQueries.GetCategoryByIdAsync(id);
        }

        // Create or Update
        public async Task<bool> CreateOrUpdateCategoryAsync(CreateOrUpdateCategoryDto dto)
        {
            var categoryEntity = new CategoryEntity
            {
                Name = dto.Name,
                Description = dto.Description
            };

            if (dto.Id == Guid.Empty)
            {
                return await _categoryQueries.CreateCategoryAsync(categoryEntity);
            }
            else
            {
                categoryEntity.Id = dto.Id;
                return await _categoryQueries.UpdateCategoryAsync(categoryEntity);
            }
        }

        // Delete
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            return await _categoryQueries.DeleteCategoryAsync(id);
        }
    }
}
