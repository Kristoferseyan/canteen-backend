using CanteenWebApi.Entities;
using CanteenWebApiLibrary.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Services
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task<global::System.Boolean> DeleteRoleAsync(Guid roleId);
        Task<RoleDto> GetRoleByIdAsync(Guid roleId);
        Task<List<RoleDto>> GetRolesAsync();
        Task<RoleDto> UpdateRoleAsync(Guid roleId, RoleDto roleDto);
    }

    public class RoleService : IRoleService
    {
        private readonly CanteenDbContext _context;


        public RoleService(CanteenDbContext context)
        {
            _context = context;
        }


        public async Task<List<RoleDto>> GetRolesAsync()
        {
            try
            {

                var roles = await _context.Roles
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        RoleName = r.RoleName
                    })
                    .ToListAsync();

                return roles;
            }
            catch (Exception ex)
            {

                throw new Exception("Error fetching roles", ex);
            }
        }


        public async Task<RoleDto> GetRoleByIdAsync(Guid roleId)
        {
            try
            {

                var role = await _context.Roles
                    .Where(r => r.Id == roleId)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        RoleName = r.RoleName
                    })
                    .FirstOrDefaultAsync();

                if (role == null)
                {
                    throw new Exception("Role not found");
                }

                return role;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching role with ID {roleId}", ex);
            }
        }


        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            try
            {
                var newRole = new Role
                {
                    RoleName = roleDto.RoleName
                };

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();

                return new RoleDto
                {
                    Id = newRole.Id,
                    RoleName = newRole.RoleName
                };
            }
            catch (Exception ex)
            {

                throw new Exception("Error creating role", ex);
            }
        }


        public async Task<RoleDto> UpdateRoleAsync(Guid roleId, RoleDto roleDto)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                {
                    throw new Exception("Role not found");
                }

                role.RoleName = roleDto.RoleName;
                await _context.SaveChangesAsync();

                return new RoleDto
                {
                    Id = role.Id,
                    RoleName = role.RoleName
                };
            }
            catch (Exception ex)
            {

                throw new Exception($"Error updating role with ID {roleId}", ex);
            }
        }


        public async Task<bool> DeleteRoleAsync(Guid roleId)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                {
                    throw new Exception("Role not found");
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error deleting role with ID {roleId}", ex);
            }
        }
    }
}
