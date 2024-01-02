

using Newtonsoft.Json;
using System;
using System.Xml;

namespace PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios
{
    public class ProveedorDto
    {
        public string Id { get; set; }

        public string? Nit { get; set; }

        public string? Nombres { get; set; }

        public string? Email { get; set; }

    }
}