using Newtonsoft.Json;
using System;
using System.Xml;

namespace PuntosLeonisa.fidelizacion.Domain.Service.DTO.Usuarios
{
    public class UsuarioDto
    {
        public string? Id { get; set; }

        public string? Cedula { get; set; }

        public string? Nombres { get; set; }

        public string? Apellidos { get; set; }

        public string? Genero { get; set; }

        public string? Email { get; set; }

        public string? Celular { get; set; }

        public string? TipoUsuario { get; set; }

        public string? Estado { get; set; }

        public string? FechaCambioEstado { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public string? Agencia { get; set; }

        public string? Empresa { get; set; }

        public string? contraseña { get; set; }


    }
}

