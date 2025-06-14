﻿using Newtonsoft.Json;

namespace PuntosLeonisa.Fidelizacion.Domain.Model.Carrito
{
    public class PuntoDeVenta : IDisposable
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Ciudad { get; set; }
        public string? Superficie { get; set; }
        public string? Agencia { get; set; }
        public string? Formato { get; set; }
        public string? Status { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int? Eliminado { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {

            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
