namespace PuntosLeonisa.Seguridad.Infrasctructure.Common.DTO
{
    public class EmailDTO
    {
        public string SenderEmail { get; set; } = "retail@leonisa.com";
        public string SenderName { get; set; } = "Mis sueños a un clic";
        public string[] Recipients { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
