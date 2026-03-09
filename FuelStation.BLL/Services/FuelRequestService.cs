using AutoMapper;
using FuelStation.BLL.Services.Interfaces;
using FuelStation.Common.Enums;
using FuelStation.Common.Exceptions;
using FuelStation.Common.Models.DTOs.FuelRequest;
using FuelStation.DAL.Entities;
using FuelStation.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuelStation.BLL.Services;

public class FuelRequestService : IFuelRequestService
{
    private readonly IRepository<FuelRequest> _fuelRequestRepository;
    private readonly IRepository<Car> _carRepository;
    private readonly IMapper _mapper;
    private readonly ILocationService _locationService;

    public FuelRequestService(
        IRepository<FuelRequest> fuelRequestRepository,
        IRepository<Car> carRepository,
        IMapper mapper,
        ILocationService locationService)
    {
        _fuelRequestRepository = fuelRequestRepository;
        _carRepository = carRepository;
        _mapper = mapper;
        _locationService = locationService;
    }

    public async Task<FuelRequestDTO> CreateAsync(Guid userId, CreateFuelRequestDTO dto)
    {
        var car = await _carRepository
            .Query()
            .FirstOrDefaultAsync(x => x.Id == dto.CarId)
            ?? throw new NotFoundException("Car not found");

        if (car.UserId != userId)
            throw new ForbiddenException("Access denied");

        var entity = _mapper.Map<FuelRequest>(dto);
        entity.LocationId = await _locationService.GetOrCreateLocationIdAsync(dto.Location);
        entity.CreateAt = DateTime.UtcNow;
        entity.Status = RequestStatus.Pending;
        entity.PricePerLiter = 50;
        entity.TotalPrice = (decimal)dto.RequestedLiters * entity.PricePerLiter;

        var result = await _fuelRequestRepository.InsertAsync(entity);

        if (!result)
            throw new ExternalException("Failed to create fuel request");

        return _mapper.Map<FuelRequestDTO>(entity);
    }
}