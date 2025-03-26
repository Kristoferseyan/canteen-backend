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
    public class MenuItemServices : IMenuItemServices
    {
        private readonly CanteenDbContext _context;

        public MenuItemServices(CanteenDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseMessage<MenuItemDto>> DecreaseStock(Guid id, int stock)
        {
            var response = new ApiResponseMessage<MenuItemDto>();
            try
            {
                var menuItem = await _context.MenuItems.FirstOrDefaultAsync(o => o.Id == id);

                if (menuItem is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Item not found";
                    return response;
                }

                if (menuItem.Stock < stock)
                {
                    response.IsSuccess = false;
                    response.Message = "Not enough stock available";
                    return response;
                }

                menuItem.Stock -= stock;
                await _context.SaveChangesAsync();

                response.Data = new MenuItemDto
                {
                    Id = menuItem.Id,
                    ItemName = menuItem.ItemName,
                    Description = menuItem.Description,
                    price = menuItem.Price,
                    CategoryId = menuItem.CategoryId,
                    Stock = menuItem.Stock
                };

                response.IsSuccess = true;
                response.Message = "Stock updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error updating stock: {ex.Message}";
            }
            return response;
        }

        public async Task<ApiResponseMessage<MenuItemDto>> CreateOrUpdateMenuItemDto(MenuItemDto menuItemDto)
        {
            var response = new ApiResponseMessage<MenuItemDto>();
            try
            {
                if (menuItemDto.Id == null)
                {
                    var menuItem = new MenuItem
                    {
                        Id = Guid.NewGuid(),
                        ItemName = menuItemDto.ItemName,
                        Description = menuItemDto.Description,
                        Price = menuItemDto.price,
                        CategoryId = menuItemDto.CategoryId,
                        Stock = menuItemDto.Stock,
                        FeaturedStartTime = menuItemDto.FeaturedStartTime,
                        FeaturedEndTime = menuItemDto.FeaturedEndTime
                    };

                    _context.MenuItems.Add(menuItem);
                    await _context.SaveChangesAsync();
                    response.Data = menuItemDto;
                    response.IsSuccess = true;
                    response.Message = "Menu item created successfully";
                }
                else
                {
                    var menuItem = _context.MenuItems.FirstOrDefault(x => x.Id == menuItemDto.Id);
                    if (menuItem == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Menu item not found";
                        return response;
                    }

                    menuItem.ItemName = menuItemDto.ItemName;
                    menuItem.Description = menuItemDto.Description;
                    menuItem.Price = menuItemDto.price;
                    menuItem.CategoryId = menuItemDto.CategoryId;
                    menuItem.Stock = menuItemDto.Stock;
                    menuItem.FeaturedStartTime = menuItemDto.FeaturedStartTime;
                    menuItem.FeaturedEndTime = menuItemDto.FeaturedEndTime;

                    await _context.SaveChangesAsync();
                    response.Data = menuItemDto;
                    response.IsSuccess = true;
                    response.Message = "Menu item updated successfully";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponseMessage<MenuItemDto>> SetItemAsFeatured(Guid menuItemId, DateTime startTime, DateTime endTime)
        {
            var response = new ApiResponseMessage<MenuItemDto>();
            try
            {
                if (startTime >= endTime)
                {
                    response.IsSuccess = false;
                    response.Message = "Start time must be before end time";
                    return response;
                }

                var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId);
                if (menuItem == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Menu item not found";
                    return response;
                }

                menuItem.FeaturedStartTime = startTime;
                menuItem.FeaturedEndTime = endTime;

                await _context.SaveChangesAsync();

                response.Data = new MenuItemDto
                {
                    Id = menuItem.Id,
                    ItemName = menuItem.ItemName,
                    Description = menuItem.Description,
                    price = menuItem.Price,
                    CategoryId = menuItem.CategoryId,
                    Stock = menuItem.Stock,
                    FeaturedStartTime = menuItem.FeaturedStartTime,
                    FeaturedEndTime = menuItem.FeaturedEndTime
                };

                response.IsSuccess = true;
                response.Message = "Menu item set as featured successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error setting featured status: {ex.Message}";
            }
            return response;
        }

        public async Task<ApiResponseMessage<MenuItemDto>> RemoveFeaturedStatus(Guid menuItemId)
        {
            var response = new ApiResponseMessage<MenuItemDto>();
            try
            {
                var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId);
                if (menuItem == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Menu item not found";
                    return response;
                }

                menuItem.FeaturedStartTime = null;
                menuItem.FeaturedEndTime = null;

                await _context.SaveChangesAsync();

                response.Data = new MenuItemDto
                {
                    Id = menuItem.Id,
                    ItemName = menuItem.ItemName,
                    Description = menuItem.Description,
                    price = menuItem.Price,
                    CategoryId = menuItem.CategoryId,
                    Stock = menuItem.Stock,
                    FeaturedStartTime = null,
                    FeaturedEndTime = null
                };

                response.IsSuccess = true;
                response.Message = "Featured status removed successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error removing featured status: {ex.Message}";
            }
            return response;
        }

        public async Task<ApiResponseMessage<IEnumerable<MenuItemDto>>> GetFeaturedMenuItems()
        {
            var response = new ApiResponseMessage<IEnumerable<MenuItemDto>>();
            try
            {
                var currentTime = DateTime.Now;

                var featuredItems = await _context.MenuItems
                    .Where(m => m.FeaturedStartTime <= currentTime &&
                               m.FeaturedEndTime >= currentTime)
                    .Select(m => new MenuItemDto
                    {
                        Id = m.Id,
                        ItemName = m.ItemName,
                        Description = m.Description,
                        price = m.Price,
                        CategoryId = m.CategoryId,
                        Stock = m.Stock,
                        FeaturedStartTime = m.FeaturedStartTime,
                        FeaturedEndTime = m.FeaturedEndTime
                    })
                    .ToListAsync();

                response.Data = featuredItems;
                response.IsSuccess = true;
                response.Message = featuredItems.Any()
                    ? "Featured menu items retrieved successfully"
                    : "No featured items available at this time";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error retrieving featured menu items: {ex.Message}";
            }
            return response;
        }

        public async Task<IEnumerable<MenuItemCategoryDto>> DisplayMenuItemCategory()
        {
            var data = await (from a in _context.MenuItems
                              join b in _context.Categories on a.CategoryId equals b.Id
                              select new MenuItemCategoryDto
                              {
                                  Id = a.Id,
                                  ItemName = a.ItemName,
                                  price = a.Price,
                                  CategoryId = b.Id,
                                  CategoryName = b.CategoryName,
                                  ParentCategoryId = b.ParentCategoryId
                              }).ToArrayAsync();
            return data.AsEnumerable();
        }

        public async Task<IEnumerable<CategoryDto>> GetParentCategories()
        {
            var data = await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return data;
        }

        public async Task<ApiResponseMessage<IEnumerable<MenuItemDto>>> GetMenuItemsByCategoryId(Guid categoryId)
        {
            var response = new ApiResponseMessage<IEnumerable<MenuItemDto>>();
            try
            {
                var items = await _context.MenuItems
                    .Where(m => m.CategoryId == categoryId)
                    .Select(m => new MenuItemDto
                    {
                        Id = m.Id,
                        ItemName = m.ItemName,
                        Description = m.Description,
                        price = m.Price,
                        CategoryId = m.CategoryId,
                        Stock = m.Stock
                    })
                    .ToListAsync();

                response.Data = items;
                response.IsSuccess = true;
                response.Message = items.Any() ? "Menu items retrieved successfully" : "No items found for this category";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error retrieving menu items: {ex.Message}";
            }
            return response;
        }

        public async Task<ApiResponseMessage<IEnumerable<MenuItemDto>>> GetMenuItemsByParentCategoryId(Guid parentCategoryId)
        {
            var response = new ApiResponseMessage<IEnumerable<MenuItemDto>>();
            try
            {
                var categoryIds = await _context.Categories
                    .Where(c => c.Id == parentCategoryId || c.ParentCategoryId == parentCategoryId)
                    .Select(c => c.Id)
                    .ToListAsync();

                var items = await _context.MenuItems
                    .Where(m => categoryIds.Contains(m.CategoryId))
                    .Select(m => new MenuItemDto
                    {
                        Id = m.Id,
                        ItemName = m.ItemName,
                        Description = m.Description,
                        price = m.Price,
                        CategoryId = m.CategoryId,
                        Stock = m.Stock
                    })
                    .ToListAsync();

                response.Data = items;
                response.IsSuccess = true;
                response.Message = items.Any() ? "Menu items retrieved successfully" : "No items found for this category";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error retrieving menu items: {ex.Message}";
            }
            return response;
        }

        public async Task<ApiResponseMessage<bool>> DeleteMenuItem(Guid menuItemId)
        {
            var response = new ApiResponseMessage<bool>();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(menuItemId);
                if (menuItem == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Menu item not found.";
                    return response;
                }

                var deletedCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == "Deleted Items");

                if (deletedCategory == null)
                {
                    deletedCategory = new Category
                    {
                        Id = Guid.NewGuid(),
                        CategoryName = "Deleted Items"
                    };
                    _context.Categories.Add(deletedCategory);
                    await _context.SaveChangesAsync();
                }

                menuItem.CategoryId = deletedCategory.Id;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.IsSuccess = true;
                response.Message = "Menu item moved to 'Deleted Items' category.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.IsSuccess = false;
                response.Message = $"Error deleting menu item: {ex.Message}";
            }

            return response;
        }


    }
}