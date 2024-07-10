using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class Garantia
    {
        public string? Id { get; set; }
        public int? NroTicket { get; set; }
        public DateTime? FechaReclamacion { get; set; }
        public int? NroPedido { get; set; }
        public string? Proveedor { get; set; }
        public string? Producto { get; set; }
        public string? Observacion { get; set; }
        public string? Imagen1 { get; set; }
        public string? Imagen2 { get; set; }
        public string? Imagen3 { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Departamento { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? ObservacionEstado { get; set; }
        public string? ObservacionProveedor { get; set; }
        public string? NroGuia { get; set; }
        public string? Transportadora { get; set; }
        public string? CorreoProveedor { get; set; }
        public string? TipoReclamacion { get; set; }
        public DateTime? FechaRedencion { get; set; }
        public string? EAN { get; set; }

        public string GenerarHTMLGarantia()
        {
            var imgUrl = "https://incentivosvotre.web.app/assets/images/MSAUC.png";
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\">");
            sb.Append("<head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Tabla para Redimir</title>");
            sb.Append(" <style>\r\n        /* Estilos opcionales para el cuerpo de la página */\r\n        body {\r\n            font-family: Merriweather;\r\n            background-color: white;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n\r\n        .tabla-estilizada {\r\n            border-collapse: collapse;\r\n            width: 90%;\r\n            margin: auto;\r\n            box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);\r\n            border-radius: 10px;\r\n            overflow: hidden;\r\n        }\r\n\r\n        .encabezado-tabla {\r\n            background-color: #594fa0;\r\n            color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            font-weight: bold;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas del encabezado */\r\n        }\r\n\r\n        .celda-tabla {\r\n            background-color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas normales */\r\n        }\r\n\r\n        th,\r\n        td {\r\n            border: 1px solid #000000;\r\n            /* Borde para todas las celdas */\r\n            font-weight: bold;\r\n            /* Texto en negrita para todas las celdas */\r\n        }\r\n\r\n        .imagen-centrada {\r\n            display: block;\r\n            margin-left: auto;\r\n            margin-right: auto;\r\n            width: 50%;\r\n        }\r\n    </style>");
            sb.Append("</head>");
            sb.Append("<body>");

            // Contenido principal
            sb.Append("<div>");
            sb.Append("<!-- Aquí puedes agregar cualquier otro contenido HTML antes de la tabla -->");

            sb.Append("<div style=\"padding: 5%;\" class=\"\">");
            //agregar imagen al correo
            sb.Append($"<img src=\"{imgUrl}\" class=\"imagen-centrada\">");
            sb.Append("</div>");
            sb.Append("<div style=\"text-align: center; padding: 20px;\">");
            if (this.Estado == "Rechazado")
            {
                sb.Append("<p>Queremos informarte que el estado de tu garantía ha cambiado.  Razón: " + this.ObservacionEstado + "</p>");
                if (this.ObservacionEstado == "No se pudo contactar")
                {
                    sb.Append("<p>Pronto nos pondremos en contacto contigo. Debes estar pendiente de tu teléfono y correo electrónico.</p>");
                }
                else
                {
                    sb.Append("<p>Motivo: " + this.ObservacionProveedor + ".</p>");
                }

                sb.Append("<table class=\"tabla-estilizada\">");
                sb.Append("<tr><th class=\"encabezado-tabla\">Ticket de reclamación</th><th class=\"encabezado-tabla\">Fecha reclamación</th><th class=\"encabezado-tabla\">Nro de pedido</th><th class=\"encabezado-tabla\">Proveedor</th><th class=\"encabezado-tabla\">Producto</th><th class=\"encabezado-tabla\">Observación proveedor</th></tr>");
                sb.AppendFormat("<tr><td class=\"celda-tabla\">{0}</td><td class=\"celda-tabla\">{1}</td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td><td class=\"celda-tabla\">{4}</td><td class=\"celda-tabla\">{5}</td></tr>",
                            this.NroTicket, this.FechaReclamacion, this.NroPedido, this.Proveedor, this.Producto, this.ObservacionEstado);

                sb.Append("</table>");
            }
            if (this.Estado == "En progreso")
            {
                sb.Append("<p>Queremos informarte que el estado de tu garantía ha cambiado a: " + this.Estado + "</p>");
                sb.Append("<p>estamos atendiendo tu caso.</p>");
                sb.Append("<table class=\"tabla-estilizada\">");
                sb.Append("<tr><th class=\"encabezado-tabla\">Ticket de reclamación</th><th class=\"encabezado-tabla\">Fecha reclamación</th><th class=\"encabezado-tabla\">Nro de pedido</th><th class=\"encabezado-tabla\">Proveedor</th><th class=\"encabezado-tabla\">Producto</th><th class=\"encabezado-tabla\">Observación proveedor</th></tr>");
                sb.AppendFormat("<tr><td class=\"celda-tabla\">{0}</td><td class=\"celda-tabla\">{1}</td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td><td class=\"celda-tabla\">{4}</td><td class=\"celda-tabla\">{5}</td></tr>",
                            this.NroTicket, this.FechaReclamacion, this.NroPedido, this.Proveedor, this.Producto, this.ObservacionEstado);

                sb.Append("</table>");
            }
            if (this.Estado == "Atendida")
            {
                sb.Append("<p>Queremos informarte que el estado de tu garantía ha cambiado a: " + this.Estado + "</p>");
                sb.Append("<p>" + this.ObservacionProveedor + "</p>");
                sb.Append("<p>Número de guía: " + this.NroGuia + "</p>");
                sb.Append("<p>Transportadora: " + this.Transportadora + "</p>");               
                sb.Append("<table class=\"tabla-estilizada\">");
                sb.Append("<tr><th class=\"encabezado-tabla\">Ticket de reclamación</th><th class=\"encabezado-tabla\">Fecha reclamación</th><th class=\"encabezado-tabla\">Nro de pedido</th><th class=\"encabezado-tabla\">Proveedor</th><th class=\"encabezado-tabla\">Producto</th><th class=\"encabezado-tabla\">Observación proveedor</th></tr>");
                sb.AppendFormat("<tr><td class=\"celda-tabla\">{0}</td><td class=\"celda-tabla\">{1}</td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td><td class=\"celda-tabla\">{4}</td><td class=\"celda-tabla\">{5}</td></tr>",
                            this.NroTicket, this.FechaReclamacion, this.NroPedido, this.Proveedor, this.Producto, this.ObservacionEstado);

                sb.Append("</table>");
            }
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</body>");
            sb.Append("</html>");
            return sb.ToString();
        }

        public string GenerarHTMLGarantiaEnviada()
        {
            var imgUrl = "https://incentivosvotre.web.app/assets/images/MSAUC.png";
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\">");
            sb.Append("<head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Tabla para Redimir</title>");
            sb.Append(" <style>\r\n        /* Estilos opcionales para el cuerpo de la página */\r\n        body {\r\n            font-family: Merriweather;\r\n            background-color: white;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n\r\n        .tabla-estilizada {\r\n            border-collapse: collapse;\r\n            width: 90%;\r\n            margin: auto;\r\n            box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);\r\n            border-radius: 10px;\r\n            overflow: hidden;\r\n        }\r\n\r\n        .encabezado-tabla {\r\n            background-color: #594fa0;\r\n            color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            font-weight: bold;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas del encabezado */\r\n        }\r\n\r\n        .celda-tabla {\r\n            background-color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas normales */\r\n        }\r\n\r\n        th,\r\n        td {\r\n            border: 1px solid #000000;\r\n            /* Borde para todas las celdas */\r\n            font-weight: bold;\r\n            /* Texto en negrita para todas las celdas */\r\n        }\r\n\r\n        .imagen-centrada {\r\n            display: block;\r\n            margin-left: auto;\r\n            margin-right: auto;\r\n            width: 50%;\r\n        }\r\n    </style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<div>");
            sb.Append("<!-- Aquí puedes agregar cualquier otro contenido HTML antes de la tabla -->");
            sb.Append("<div style=\"padding: 5%;\" class=\"\">");
            sb.Append($"<img src=\"{imgUrl}\" class=\"imagen-centrada\">");
            sb.Append("</div>");
            sb.Append("<div style=\"text-align: center; padding: 20px;\">");
            sb.Append("<p>Tu reclamación ha sido solicitada con éxito:</p>");
            sb.Append("<table class=\"tabla-estilizada\">");
            sb.Append("<tr><th class=\"encabezado-tabla\">Ticket de reclamación</th><th class=\"encabezado-tabla\">Nro pedido</th><th class=\"encabezado-tabla\">Fecha reclamación</th><th class=\"encabezado-tabla\">Proveedor</th><th class=\"encabezado-tabla\">Producto</th><th class=\"encabezado-tabla\">Observación usuario</th></tr>");
            sb.AppendFormat("<tr><td class=\"celda-tabla\">{0}</td><td class=\"celda-tabla\">{1}</td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td><td class=\"celda-tabla\">{4}</td><td class=\"celda-tabla\">{5}</td></tr>",
                        this.NroTicket, this.NroPedido, this.FechaReclamacion, this.Proveedor, this.Producto, this.Observacion);

            sb.Append("</table>");
            sb.Append("<p>Estado actual: PENDIENTE</p>");
            sb.Append("<p>Pronto nos pondremos en contacto contigo. Debes estar pendiente de tu teléfono y correo electrónico.</p>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</body>");
            sb.Append("</html>");
            return sb.ToString();
        }
    }

}
