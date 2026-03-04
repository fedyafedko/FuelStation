using AutoMapper;
using FuelStation.BLL.Services.Interfaces;
using FuelStation.Common.Exceptions;
using FuelStation.Common.Models.DTOs.User;
using FuelStation.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace FuelStation.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDTO> GetUserAsync(Guid userId)
    {
        var entity = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var user = _mapper.Map<UserDTO>(entity);

        return user;
    }
}