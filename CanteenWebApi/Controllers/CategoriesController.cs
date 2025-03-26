using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        public CategoriesController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        [HttpGet("children")]
        public async Task<ActionResult<ApiResponseMessage<List<CategoryDto>>>> GetChildCategories([FromQuery] string parentName)
        {
            var response = await _categoryServices.GetChildCategoriesByParentName(parentName);
            return Ok(response); 
        }



    }
}

