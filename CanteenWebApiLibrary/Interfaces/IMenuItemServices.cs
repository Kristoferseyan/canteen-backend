using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;

namespace CanteenWebApiLibrary.Services
{
    public interface IMenuItemServices
    {
        Task<ApiResponseMessage<MenuItemDto>> CreateOrUpdateMenuItemDto(MenuItemDto menuItemDto);
        Task<IEnumerable<MenuItemCategoryDto>> DisplayMenuItemCategory();
        Task<IEnumerable<CategoryDto>> GetParentCategories();
        Task<ApiResponseMessage<IEnumerable<MenuItemDto>>> GetMenuItemsByCategoryId(Guid categoryId);
        Task<ApiResponseMessage<IEnumerable<MenuItemDto>>> GetMenuItemsByParentCategoryId(Guid parentCategoryId);
        Task<ApiResponseMessage<bool>> DeleteMenuItem(Guid menuItemId);
        Task<ApiResponseMessage<MenuItemDto>> DecreaseStock(Guid id, int stock);
        Task<ApiResponseMessage<MenuItemDto>> SetItemAsFeatured(Guid menuItemId, DateTime startTime, DateTime endTime);
        Task<ApiResponseMessage<MenuItemDto>> RemoveFeaturedStatus(Guid menuItemId);
        Task<ApiResponseMessage<IEnumerable<MenuItemDto>>> GetFeaturedMenuItems();
    }
}