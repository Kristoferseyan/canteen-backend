
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemServices _menuItemServices;

        public MenuItemController(IMenuItemServices menuItemServices)
        {   
            _menuItemServices = menuItemServices;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseMessage<MenuItemDto>>> CreateOrUpdateMenuItem(MenuItemDto dto)
        {
            var response = await _menuItemServices.CreateOrUpdateMenuItemDto(dto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("DisplayMenuItemCategory")]
        public async Task<ActionResult<ApiResponseMessage<IEnumerable<MenuItemCategoryDto>>>> MenuItemCategory()
        {
            var item = await _menuItemServices.DisplayMenuItemCategory();

            if (item == null)
            {
                return Ok(new ApiResponseMessage<IEnumerable<MenuItemCategoryDto>>()
                {
                    Data = [],
                    Message = "No Product",
                    IsSuccess = false
                }); 
            }

            return Ok(new ApiResponseMessage<IEnumerable<MenuItemCategoryDto>>()
            {
                Data = item,
                Message = "Ok",
                IsSuccess = true
            });

        }

        [HttpGet("parent-categories")]
        public async Task<IActionResult> GetParentCategories()
        {
            var categories = await _menuItemServices.GetParentCategories();
            return Ok(categories);
        }

        [HttpGet("GetByCategory/{categoryId}")]
        public async Task<ActionResult<ApiResponseMessage<IEnumerable<MenuItemDto>>>> GetMenuItemsByCategoryId(Guid categoryId)
        {
            var result = await _menuItemServices.GetMenuItemsByCategoryId(categoryId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        [HttpGet("by-parent-category/{parentCategoryId}")]
        public async Task<IActionResult> GetMenuItemsByParentCategory(Guid parentCategoryId)
        {
            var result = await _menuItemServices.GetMenuItemsByParentCategoryId(parentCategoryId);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            var result = await _menuItemServices.DeleteMenuItem(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("decrease-stock")]
        public async Task<ActionResult<ApiResponseMessage<MenuItemDto>>> DecreaseStock(DecreaseStockDto decreaseDto)
        {
            var response = await _menuItemServices.DecreaseStock(decreaseDto.Id, decreaseDto.Stock);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
        
        [HttpGet("featured")]
        public async Task<ActionResult<ApiResponseMessage<IEnumerable<MenuItemDto>>>> GetFeaturedMenuItems()
        {
            var response = await _menuItemServices.GetFeaturedMenuItems();
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
        
        [HttpPost("{id}/set-featured")]
        public async Task<ActionResult<ApiResponseMessage<MenuItemDto>>> SetItemAsFeatured(Guid id, [FromBody] FeaturedTimeDto featuredTimeDto)
        {
            var response = await _menuItemServices.SetItemAsFeatured(id, featuredTimeDto.StartTime, featuredTimeDto.EndTime);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
        
        [HttpPost("{id}/remove-featured")]
        public async Task<ActionResult<ApiResponseMessage<MenuItemDto>>> RemoveFeaturedStatus(Guid id)
        {
            var response = await _menuItemServices.RemoveFeaturedStatus(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

    }
}
