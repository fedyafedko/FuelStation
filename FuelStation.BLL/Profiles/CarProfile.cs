using AutoMapper;
using FuelStation.Common.Models.DTOs.Car;
using FuelStation.DAL.Entities;

namespace FuelStation.BLL.Profiles;

public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap<Car, CarDTO>();
        CreateMap<CreateCarDTO, Car>();
        CreateMap<UpdateCarDTO, Car>();
    }
}

