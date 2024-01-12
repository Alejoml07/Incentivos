using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class UsuarioRedencion
    {
        public UsuarioRedencion()
        {
           this.PuntosRedimidos = this.GetSumPuntos();
        }
        public string? Id { get; set; }

        public int? NroPedido { get; set; }

        public string? NroGuia { get; set; }

        public string? Transportadora { get; set; }

        public Usuario? Usuario { get; set; }

        public UsuarioInfoPuntos? InfoPuntos { get; set; }

        public IEnumerable<ProductoRefence>? ProductosCarrito { get; set; }

        public UsuarioEnvio? Envio
        {
            get;
            set;
        }

        public EstadoOrden? Estado
        {
            get
            {
                if(ProductosCarrito == null)
                {
                    return EstadoOrden.Pendiente;
                }
                

                if (ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Pendiente))
                {
                    return EstadoOrden.Pendiente;
                }

                if (ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Enviado))
                {
                    return EstadoOrden.EnvioParcial;
                }

                var totalEnviados = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Enviado);

                if (totalEnviados == ProductosCarrito.Count())
                {
                    return EstadoOrden.Enviado;
                }

                var totalEntregados = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Entregado);

                if (totalEntregados == ProductosCarrito.Count())
                {
                    return EstadoOrden.Entregado;
                }

                var totalCancelados = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Cancelado);

                if (totalCancelados == ProductosCarrito.Count())
                {
                    return EstadoOrden.Cancelado;
                }

                return EstadoOrden.Pendiente;
            }
        }



        public int? PuntosRedimidos
        {
            get;
            set;
        }


        public DateTime? FechaRedencion { get; set; }


        public int? GetSumPuntos()
        {
            return this.ProductosCarrito?.Sum(p => Convert.ToInt32(p.Puntos) * p.Quantity);
        }
        public string GenerarHTML()
        {
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\">");
            sb.Append("<head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Tabla para Redimir</title>");
            sb.Append(" <style>\r\n        /* Estilos opcionales para el cuerpo de la página */\r\n        body {\r\n            font-family: Merriweather;\r\n            background-color: white;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n\r\n        .tabla-estilizada {\r\n            border-collapse: collapse;\r\n            width: 90%;\r\n            margin: auto;\r\n            box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);\r\n            border-radius: 10px;\r\n            overflow: hidden;\r\n        }\r\n\r\n        .encabezado-tabla {\r\n            background-color: #02376b;\r\n            color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            font-weight: bold;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas del encabezado */\r\n        }\r\n\r\n        .celda-tabla {\r\n            background-color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas normales */\r\n        }\r\n\r\n        th,\r\n        td {\r\n            border: 1px solid #000000;\r\n            /* Borde para todas las celdas */\r\n            font-weight: bold;\r\n            /* Texto en negrita para todas las celdas */\r\n        }\r\n    </style>");
            sb.Append("</head>");
            sb.Append("<body>");

            // Contenido principal
            sb.Append("<div>");
            sb.Append("<!-- Aquí puedes agregar cualquier otro contenido HTML antes de la tabla -->");

            sb.Append("<div style=\"padding: 5%;\" class=\"\">");
            sb.Append("<img src=\"https://stgactincentivos.blob.core.windows.net/$web/img/mis%20sue%C3%B1os%20a%20un%20clic.svg?sp=r&st=2023-12-19T16:00:18Z&se=2023-12-20T00:00:18Z&spr=https&sv=2022-11-02&sr=b&sig=oZC6y4Uw3sH9%2FjTNNYgxzDqxQfKzqZAeS6clt92IS6Y%3D\" style=\"width: 100%; margin: auto; display: block;\">");
            sb.Append("</div>");


            // Tabla con los detalles del usuario
            sb.Append("<table class=\"tabla-estilizada\">");
            // Asumiendo que tienes un objeto UsuarioEnvio con los datos requeridos
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">NRO DE PEDIDO</td><td class=\"celda-tabla\">{0}</td></tr>", this.NroPedido);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">NOMBRE DE USUARIO</td><td class=\"celda-tabla\">{0} {1}</td></tr>", this.Usuario.Nombres, Usuario.Apellidos);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">CÉDULA</td><td class=\"celda-tabla\">{0}</td></tr>", Usuario.Cedula);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">EMPRESA</td><td class=\"celda-tabla\">{0}</td></tr>", Usuario.Empresa);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">NOMBRE DE QUIEN RECIBE</td><td class=\"celda-tabla\">{0} {1}</td></tr>", Envio.Nombres, Envio.Apellidos);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">CONTACTO DE QUIEN RECIBE</td><td class=\"celda-tabla\">{0}</td></tr>", Envio.Celular);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">DEPARTAMENTO</td><td class=\"celda-tabla\">{0}</td></tr>", Envio.Departamento);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">MUNICIPIO</td><td class=\"celda-tabla\">{0}</td></tr>", Envio.Ciudad);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">DIRECCIÓN</td><td class=\"celda-tabla\">{0}</td></tr>", Envio.Direccion);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">OBSERVACIONES</td><td class=\"celda-tabla\">{0}</td></tr>", Envio.Observaciones);
            sb.Append("</table>");

            // ... código posterior ...


            // Tabla con los productos del carrito
            sb.Append("<table class=\"tabla-estilizada\">");
            sb.Append("<tr><th class=\"encabezado-tabla\">IMAGEN</th><th class=\"encabezado-tabla\">PRODUCTO</th><th class=\"encabezado-tabla\">DESCRIPCIÓN</th><th class=\"encabezado-tabla\">CANTIDAD</th></tr>");

            foreach (var producto in ProductosCarrito)
            {
                // Asegúrate de tener propiedades como Imagen, Nombre, Descripción en la clase ProductoRefence
                sb.AppendFormat("<tr><td class=\"celda-tabla\"><img src='{0}'style='width:100px;height:100px'/></td><td class=\"celda-tabla\">{1}</td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td></tr>",
                                producto.UrlImagen1, producto.Nombre, producto.Descripcion, producto.Quantity);
            }

            sb.Append("</table>");

            sb.Append("</div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

    }

    public class UsuarioEnvio
    {
        public string? Id { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Direccion { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public string? Ciudad { get; set; }
        public string? Departamento { get; set; }
        public string? Observaciones { get; set; }

    }

    public enum EstadoOrden
    {
        Pendiente,
        Enviado,
        EnvioParcial,
        Cancelado,
        Entregado
    }

}
