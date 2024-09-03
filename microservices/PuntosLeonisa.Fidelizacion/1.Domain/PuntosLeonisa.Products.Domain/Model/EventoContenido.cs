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

    }
}
