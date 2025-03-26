using CanteenWebApi.Entities;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly CanteenDbContext _context;

        public CategoryServices(CanteenDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }


        public async Task<ApiResponseMessage<List<CategoryDto>>> GetChildCategoriesByParentName(string parentCategoryName)
        {
            var parentCategory = await _context.Categories
                .Where(c => c.CategoryName == parentCategoryName)
                .Select(c => new { c.Id })
                .FirstOrDefaultAsync();

            if (parentCategory == null)
            {
                return new ApiResponseMessage<List<CategoryDto>>
                {
                    Message = "Parent category not found.",
                    IsSuccess = false,
                    Data = new List<CategoryDto>()
                };
            }
            var childCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategory.Id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return new ApiResponseMessage<List<CategoryDto>>
            {
                Message = childCategories.Any() ? "Child categories retrieved successfully." : "No child categories found.",
                IsSuccess = childCategories.Any(),
                Data = childCategories
            };
        }

        public async Task<ApiResponseMessage<List<CategoryDto>>> FetchAllChildCategories()
        {
            var childCategories = await _context.Categories
                .Where(c => c.ParentCategoryId != null)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return new ApiResponseMessage<List<CategoryDto>>
            {
                Message = childCategories.Any() ? "All child categories retrieved successfully." : "No child categories found.",
                IsSuccess = childCategories.Any(),
                Data = childCategories
            };
        }


    }
}

