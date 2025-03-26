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
    public class UserService : IUserService
    {
        private readonly CanteenDbContext _context;

        public UserService(CanteenDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponseMessage<UserDto>> CreateOrUpdateUserDto(UserDto userDto)
        {
            var response = new ApiResponseMessage<UserDto>();
            try
            {
                if (userDto.id == null)
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Username = userDto.Username,
                        Password = userDto.password,
                        RoleId = userDto.RoleId
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    response.Data = userDto;
                    response.IsSuccess = true;
                    response.Message = "User created successfully";
                }
                else
                {
                    var user = _context.Users.FirstOrDefault(x => x.Id == userDto.id);
                    if (user == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "User not found";
                        return response;
                    }
                    user.FirstName = userDto.FirstName;
                    user.LastName = userDto.LastName;
                    user.Username = userDto.Username;
                    user.Password = userDto.password;
                    user.RoleId = userDto.RoleId;
                    await _context.SaveChangesAsync();
                    response.Data = userDto;
                    response.IsSuccess = true;
                    response.Message = "User updated successfully";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<IEnumerable<UserRoleDto>> DisplayUserRole()
        {
            var data = await (from a in _context.Users
                              join b in _context.Roles on a.RoleId equals b.Id
                              select new UserRoleDto
                              {
                                  id = a.Id,
                                  FirstName = a.FirstName,
                                  LastName = a.LastName,
                                  password = a.Password,
                                  Username = a.Username,
                                  RoleId = b.Id,
                                  RoleName = b.RoleName
                              }).ToListAsync();
            return data.AsEnumerable();
        }
        public async Task<List<User>> DisplayAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<ApiResponseMessage<UserDto>> GetUserById(Guid id)
        {
            var response = new ApiResponseMessage<UserDto>();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found";
                    return response;
                }
                
                response.Data = new UserDto
                {
                    id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    password = user.Password,
                    RoleId = user.RoleId
                };
                response.IsSuccess = true;
                response.Message = "User retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            
            return response;
        }
        public async Task<UserDto?> AuthenticateUser(string username, string password)
        {
            var user = await _context.Users
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null || user.Password != password)
            {
                return null;
            }

            return new UserDto
            {
                id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                RoleId = user.RoleId,
            };
        }

        public async Task<ApiResponseMessage<UserDto>> UpdateUserRoleAsync(Guid userId, string newRoleId)
        {
            var response = new ApiResponseMessage<UserDto>();
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found";
                    return response;
                }

                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == Guid.Parse(newRoleId));

                if (role == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Role not found";
                    return response;
                }

                user.RoleId = Guid.Parse(newRoleId);

                await _context.SaveChangesAsync();

                response.Data = new UserDto
                {
                    id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    RoleId = user.RoleId
                };
                response.IsSuccess = true;
                response.Message = "User role updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }



    }

}
