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

                // add tratamiento de datos, si el dato ya existe update
                var tratamiento = await tratamientoDatosRepository.GetById(value.Email);
                var usuario = await usuarioRepository.GetUsuarioByEmail(value.Email);
                if (tratamiento != null)
                {
                    return new GenericResponse<TratamientoDatosDto>
                    {
                        Message = "El usuario ya acepto terminos y condiciones",
                        Result = value,
                    };
                }
                else
                {
                    value.Id = Guid.NewGuid().ToString();
                    value.FechaAceptacion = DateTime.Now.AddHours(-5);
                    usuario.TratamientoDatos= true;
                    await usuarioRepository.Update(usuario);
                    await tratamientoDatosRepository.Add(value);
                    return new GenericResponse<TratamientoDatosDto>
                    {
                        Message = "Registro agregado",
                        Result = value,
                    };
                }
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
            if (usuario.Result.TipoUsuario != "Asesoras vendedoras")
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Usuario no necesita aceptar terminos y condiciones",
                    Result = true,
                });
            }
            if(usuario.Result.TratamientoDatos == true)
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    Message = "Usuario ya acepto terminos y condiciones",
                    Result = true,
                });
                
            }else if (usuario.Result.TratamientoDatos == false && usuario.Result.TipoUsuario == "Asesoras vendedoras")
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    Message = "El usuario no ha aceptado termino y condiciones",
                    Result = false,
                });
            }
            else
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    Message = "El usuario no ha aceptado termino y condiciones",
                    Result = false,
                });
            }

        }
    }
}
