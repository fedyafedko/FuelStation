using FuelStation.Common.Models.DTOs.FuelRequest;

namespace FuelStation.BLL.Services.Interfaces;

public interface IFuelRequestService
{
    Task<FuelRequestDTO> CreateAsync(Guid userId, CreateFuelRequestDTO dto);
}