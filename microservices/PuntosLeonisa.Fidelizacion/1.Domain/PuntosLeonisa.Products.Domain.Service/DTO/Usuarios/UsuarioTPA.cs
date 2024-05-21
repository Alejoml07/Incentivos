using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios
{
    public class UsuarioTpa
    {
        public Data Data { get; set; }
        public bool Ok { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public DateTime? FechaInicioSesion { get; set; }
        public int IdPersona { get; set; }
        public Persona Persona { get; set; }
        public object HistorialesContrasenas { get; set; }
        public object CodigoDosPasos { get; set; }
        public bool EstaBloqueado { get; set; }
        public string Token { get; set; }
    }

    public class Persona
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Identificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool? IngresoEnTemporada { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string Nacionalidad { get; set; }
        public string TipoDocumento { get; set; }
        public string Genero { get; set; }
        public string Estado { get; set; }
        public object IdCentroCosto { get; set; }
        public string TipoContrato { get; set; }
        public double? Salario { get; set; }
        public DateTime? FechaAumento { get; set; }
        public string Direcccion { get; set; }
        public string Celular { get; set; }
        public string CajaCompensacion { get; set; }
        public string FondoPension { get; set; }
        public string Email { get; set; }
        public string FondoCesantias { get; set; }
        public string Eps { get; set; }
        public string EstadoCivil { get; set; }
        public string NombreConyugue { get; set; }
        public string TipoVivienda { get; set; }
        public int? NumeroHijos { get; set; }
        public int? Estrato { get; set; }
        public int? NumeroPersonasEnVivienda { get; set; }
        public string Telefono { get; set; }
        public int? IdRol { get; set; }
        public bool AceptaTerminosTratamientoDatos { get; set; }
        public object IdLiderDirecto { get; set; }
        public object IdTienda { get; set; }
        public int? IdEmpresa { get; set; }
        public object IdCiudad { get; set; }
        public int? IdDepartamento { get; set; }
        public string Ciudad { get; set; }
        public object NroHorasContrato { get; set; }
        public object NuevoNroHorasContrato { get; set; }
        public object IdEmpresaNueva { get; set; }
        public object Empresa { get; set; }
        public DateTime? FechaNuevoIngreso { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public object TiendaPersona { get; set; }
        public Rol Rol { get; set; }
        public object Usuario { get; set; }
        public object NotificacionesCreadoras { get; set; }
        public object NotificacionesVendedoras { get; set; }
        public object Tienda { get; set; }
        public object LiderDirecto { get; set; }
        public object Lideres { get; set; }
        public object PlaneacionComercial { get; set; }
        public object Novedades { get; set; }
        public object PersonasTiendas { get; set; }
        public object NovedadesPlaneadas { get; set; }
    }

    public class Rol
    {
        public int Id { get; set; }
        public string RolNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public List<Persona> Personas { get; set; }
        public object RolesMenu { get; set; }
    }
}