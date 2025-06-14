﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios
{
    public  class UsuarioResponseLiteDto
    {
        public string? Nombres { get; set; }

        public string? Cedula { get; set; }

        public string? Apellidos { get; set; }

        public string? Celular { get; set; }

        public string? Genero { get; set; }

        public string? TipoUsuario { get; set; }

        public string? Email { get; set; }

        public string? Proveedor { get; set; }

        public string? Agencia { get; set; }

        public string? Empresa { get; set; }

        public string Tkn { get; set; }
        public string? Ciudad { get; set; }
    }
}
