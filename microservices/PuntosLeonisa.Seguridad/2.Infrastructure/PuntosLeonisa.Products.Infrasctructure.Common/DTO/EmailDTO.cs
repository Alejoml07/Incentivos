namespace PuntosLeonisa.Seguridad.Infrasctructure.Common.DTO
{
    public class EmailDTO
    {
        public string senderEmail { get; set; } = "retail@leonisa.com";
        public string senderName { get; set; } = "Mis sueños a un clic";
        public string[] recipients { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
    }
}
