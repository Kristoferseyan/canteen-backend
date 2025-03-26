using CanteenWebApi.Entities;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;

namespace CanteenWebApiLibrary.Services
{
    public interface ICategoryServices
    {
        Task<Category> CreateCategory(Category category);
        Task<ApiResponseMessage<List<CategoryDto>>> GetChildCategoriesByParentName(string parentCategoryName);
    }
}