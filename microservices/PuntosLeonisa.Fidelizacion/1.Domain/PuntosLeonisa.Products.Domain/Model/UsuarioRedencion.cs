using PuntosLeonisa.Products.Domain.Model;
using System.Text;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class UsuarioRedencion
    {
        public UsuarioRedencion()
        {
            this.PuntosRedimidos = this.GetSumPuntos();
            this.Estado = this.GetEstadoOrden();
        }
        public string? Id { get; set; }

        public int? NroPedido { get; set; }

        public int? ValorMovimiento { get; set; }

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
            get;
            set;
        }



        public int? PuntosRedimidos
        {
            get;
            set;
        }


        public DateTime? FechaRedencion { get; set; }

        public EstadoOrden GetEstadoOrden()
        {

            if (ProductosCarrito == null)
            {
                return EstadoOrden.Pendiente;
            }
            var total = ProductosCarrito.Count();
            var totalEnviados2 = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Enviado);
            var totalCancelados = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Cancelado);
            var totalEnviados = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Enviado);
            var totalEntregados = ProductosCarrito.Count(p => p.Estado == EstadoOrdenItem.Entregado);

            if (ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Enviado) && total > 1 && totalEnviados2 != total && totalEntregados == 0)
            {
                return EstadoOrden.EnvioParcial;
            }

            if (ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Entregado) && total > 1 && totalEntregados != total)
            {
                return EstadoOrden.EntregadoParcial;
            }

            if (ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Pendiente))
            {
                return EstadoOrden.Pendiente;
            }

            if (totalEnviados == ProductosCarrito.Count())
            {
                return EstadoOrden.Enviado;
            }

            if (totalEntregados == ProductosCarrito.Count())
            {
                return EstadoOrden.Entregado;
            }

            if (totalCancelados == ProductosCarrito.Count())
            {
                return EstadoOrden.Cancelado;
            }
            return EstadoOrden.Pendiente;
        }


        public int? GetSumPuntos()
        {
            return this.ProductosCarrito?.Sum(p => Convert.ToInt32(p.Puntos) * p.Quantity);
        }
        public string GenerarHTML()
        {
            var imgUrl = "https://incentivosvotre.web.app/assets/images/MSAUC.png";
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\">");
            sb.Append("<head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Tabla para Redimir</title>");
            sb.Append(" <style>\r\n        /* Estilos opcionales para el cuerpo de la página */\r\n        body {\r\n            font-family: Merriweather;\r\n            background-color: white;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n\r\n        .tabla-estilizada {\r\n            border-collapse: collapse;\r\n            width: 90%;\r\n            margin: auto;\r\n            box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);\r\n            border-radius: 10px;\r\n            overflow: hidden;\r\n        }\r\n\r\n        .encabezado-tabla {\r\n            background-color: #594fa0;\r\n            color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            font-weight: bold;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas del encabezado */\r\n        }\r\n\r\n        .celda-tabla {\r\n            background-color: white;\r\n            text-align: center;\r\n            padding: 0.5rem;\r\n            border: 1px solid #000000;\r\n            /* Borde para las celdas normales */\r\n        }\r\n\r\n        th,\r\n        td {\r\n            border: 1px solid #000000;\r\n            /* Borde para todas las celdas */\r\n            font-weight: bold;\r\n            /* Texto en negrita para todas las celdas */\r\n        }\r\n\r\n        .imagen-centrada {\r\n            display: block;\r\n            margin-left: auto;\r\n            margin-right: auto;\r\n            width: 50%;\r\n        }\r\n\r\n        .imagen-producto {\r\n            width: 100px;\r\n            height: 100px;\r\n        }\r\n    </style>");
            sb.Append("</head>");
            sb.Append("<body>");

            // Contenido principal
            sb.Append("<div>");
            sb.Append("<!-- Aquí puedes agregar cualquier otro contenido HTML antes de la tabla -->");

            sb.Append("<div style=\"padding: 5%;\" class=\"\">");
            // agregar imagen al correo
            sb.Append($"<img src=\"{imgUrl}\" class=\"imagen-centrada\">");
            sb.Append("</div>");

            // Tabla con los detalles del usuario
            sb.Append("<table class=\"tabla-estilizada\">");
            // Asumiendo que tienes un objeto UsuarioEnvio con los datos requeridos
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">NRO DE PEDIDO</td><td class=\"celda-tabla\">{0}</td></tr>", this.NroPedido);
            if (this.Usuario.Nombres != null && Usuario.Apellidos != null)
            {
                sb.AppendFormat("<tr><td class=\"encabezado-tabla\">NOMBRE DE USUARIO</td><td class=\"celda-tabla\">{0} {1}</td></tr>", this.Usuario.Nombres, Usuario.Apellidos);
            }
            if (Usuario.Cedula != null)
            {
                sb.AppendFormat("<tr><td class=\"encabezado-tabla\">CÉDULA</td><td class=\"celda-tabla\">{0}</td></tr>", Usuario.Cedula);
            }
            if (Usuario.Empresa != null)
            {
                sb.AppendFormat("<tr><td class=\"encabezado-tabla\">EMPRESA</td><td class=\"celda-tabla\">{0}</td></tr>", Usuario.Empresa);
            }
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">NOMBRE DE QUIEN RECIBE</td><td class=\"celda-tabla\">{0} {1}</td></tr>", Envio.Nombres, Envio.Apellidos);
            sb.AppendFormat("<tr><td class=\"encabezado-tabla\">TIPO DE USUARIO</td><td class=\"celda-tabla\">{0}</td></tr>", Usuario.TipoUsuario);
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
                sb.AppendFormat("<tr><td class=\"celda-tabla\"><img src='{0}' class='imagen-producto'/></td><td class=\"celda-tabla\">{1}</td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td></tr>",
                                producto.UrlImagen1, producto.Nombre, producto.Descripcion, producto.Quantity);
            }

            sb.Append("</table>");

            sb.Append("</div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public string GenerarHTMLCambioEstado(string id)
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
            // agregar imagen al correo
            sb.Append($"<img src=\"{imgUrl}\" class=\"imagen-centrada\">");
            sb.Append("</div>");
            sb.Append("<div style=\"text-align: center; padding: 20px;\">");
            sb.Append("<h2>¡Hola! " + this.Usuario.Nombres + "</h2>");
            sb.Append("<p>Queremos informarte que el estado de tu producto ha cambiado. Aquí están los detalles:</p>");

            if (ProductosCarrito.FirstOrDefault().Estado == EstadoOrdenItem.Cancelado)
            {
                foreach (var producto in ProductosCarrito.Where(x => x.Estado == EstadoOrdenItem.Cancelado && x.ProveedorLite.Id == id))
                {
                    sb.Append("<table class=\"tabla-estilizada\">");
                    sb.Append("<tr><th class=\"encabezado-tabla\">Nro pedido</th><th class=\"encabezado-tabla\">IMAGEN</th><th class=\"encabezado-tabla\">PRODUCTO</th><th class=\"encabezado-tabla\">CANTIDAD</th></tr>");
                    sb.AppendFormat("<tr><td class=\"celda-tabla\">{0}</td><td class=\"celda-tabla\"><img src='{1}' style='width:100px;height:100px'/></td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td></tr>",
                                    this.NroPedido, producto.UrlImagen1, producto.Nombre, producto.Quantity);

                    sb.Append("</table>");
                    sb.Append($"<p><strong>Estado actual del producto:</strong> {producto.Estado}</p>");
                }
            }
            else
            {
                foreach (var producto in ProductosCarrito)
                {
                    sb.Append("<table class=\"tabla-estilizada\">");
                    sb.Append("<tr><th class=\"encabezado-tabla\">Nro pedido</th><th class=\"encabezado-tabla\">IMAGEN</th><th class=\"encabezado-tabla\">PRODUCTO</th><th class=\"encabezado-tabla\">CANTIDAD</th></tr>");
                    sb.AppendFormat("<tr><td class=\"celda-tabla\">{0}</td><td class=\"celda-tabla\"><img src='{1}' style='width:100px;height:100px'/></td><td class=\"celda-tabla\">{2}</td><td class=\"celda-tabla\">{3}</td></tr>",
                                    this.NroPedido, producto.UrlImagen1, producto.Nombre, producto.Quantity);

                    sb.Append("</table>");
                    sb.Append($"<p><strong>Estado actual del producto:</strong> {producto.Estado}</p>");
                    sb.Append($"<p><strong>Número de guía:</strong> {producto.NroGuia}</p>");
                    sb.Append($"<p><strong>Transportadora:</strong> {producto.Transportadora}</p>");
                }
            }
            sb.Append("<p>Saludos cordiales,</p>");
            sb.Append("</div>");
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
        public string? DireccionBasic { get; set; }
        public string? DireccionComplemento { get; set; }
        public string? Celular { get; set; }
        public string? CelularSecundario { get; set; }
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
        Entregado,
        EntregadoParcial
    }

}
