using System;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Products.Application.Core.Interfaces
{
    public interface IApplicationCore<T> where T : class
    {

        Task<GenericResponse<T>> GetById(string id);
        Task<GenericResponse<T>> Add(T value);
        Task<GenericResponse<T[]>> AddRange(T[] value);
        Task<GenericResponse<T>> Update(T value);
        Task<GenericResponse<T>> Delete(T value);
        Task<GenericResponse<T>> DeleteById(string id);
        Task<GenericResponse<IEnumerable<T>>> GetAll();
    }
}

