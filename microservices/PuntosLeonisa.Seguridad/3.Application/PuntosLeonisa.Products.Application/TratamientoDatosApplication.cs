using AutoMapper;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Application
{
    public class TratamientoDatosApplication : ITratamientoDatosApplication
    {
        private readonly IMapper mapper;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly ITratamientoDatosRepository tratamientoDatosRepository;
        private readonly GenericResponse<TratamientoDatosDto> response;

        public TratamientoDatosApplication(IMapper mapper, ITratamientoDatosRepository tratamientoDatosRepository, IUsuarioRepository usuarioRepository)
        {
            this.mapper = mapper;
            this.tratamientoDatosRepository = tratamientoDatosRepository;
            this.usuarioRepository = usuarioRepository;
        }

        public async Task<GenericResponse<TratamientoDatosDto>> Add(TratamientoDatosDto value)
        {
            try
            {
                var datos = await tratamientoDatosRepository.GetById(value.Id);
                if(datos != null)
                {
                    mapper.Map(value, datos);
                    await tratamientoDatosRepository.Update(datos);
                    
                }
                else
                {
                    value.Id = Guid.NewGuid().ToString();
                    await tratamientoDatosRepository.Add(value);

                }
                response.Result = value;
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<GenericResponse<TratamientoDatosDto[]>> AddRange(TratamientoDatosDto[] value)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<TratamientoDatosDto>> Delete(TratamientoDatosDto value)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<TratamientoDatosDto>> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<TratamientoDatosDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<TratamientoDatosDto>> GetById(string id)
        {
            var dato = await tratamientoDatosRepository.GetById(id);
            if (dato != null)
            {
                response.Result = dato;
                return response;
            }
            else
            {
                return new GenericResponse<TratamientoDatosDto>
                {
                    Message = "No se encontro el registro",
                    Result = null,
                };
            }
        }

        public Task<GenericResponse<TratamientoDatosDto>> Update(TratamientoDatosDto value)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<bool>> VerificarUser(string email)
        {
            var usuario = usuarioRepository.GetUsuarioByEmail(email);

            if(usuario == null || usuario.Result.TratamientoDatos == null || usuario.Result.TratamientoDatos == false)
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    Message = "No se encontro el usuario",
                    Result = false,
                });
            }else
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    Message = "Usuario encontrado",
                    Result = true,
                });
            }

        }
    }
}
