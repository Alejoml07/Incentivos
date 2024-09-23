using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class EventoContenido
    {
        public string Id { get; set; }
        public string? TipoEvento { get; set; }
        public string? WelcomeTitle { get; set; }
        public string? Description { get; set; }
        public string? PodcastInfo { get; set; }
        public string? InvitationMessage { get; set; }
        public string? EventDate { get; set; }
        public string? CheckboxText { get; set; }
        public string? LegalText { get; set; }
        public string? PageTitle { get; set; }
        public string? NombreEvento { get; set; }
        public DateTime? FechaEvento { get; set; }
        public string? selectedFile { get; set; }
        public List<Campos>? Campos { get; set; } = new List<Campos>();
    }

    public class Campos
    {
        public string? Nombre { get; set; }
        public bool? Mostrar { get; set; }
        public string? PlaceHolder { get; set; }
        public string? Title { get; set; }
        public string? Tipo { get; set; }
    }
}
