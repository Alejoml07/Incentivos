﻿using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface ICarritoRepository : IRepository<Carrito>
    {
        Task<IEnumerable<Carrito>> GetPuntosEnCarrito(string? email);
    }
}
