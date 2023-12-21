
namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication
{


    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("response")]
    public class Response
    {
        public string Action { get; set; }

        [XmlElement("data")]
        public DataContainer Data { get; set; }
    }
     
    public class DataContainer
    {
        [XmlElement("errorcode")]
        public int ErrorCode { get; set; }
        [XmlElement("acceptreport")]
        public AcceptReport AcceptReport { get; set; }
    }


   

    public class AcceptReport
    {
        [XmlElement("statuscode")]
        public int StatusCode { get; set; }

        [XmlElement("statusmessage")]
        public string StatusMessage { get; set; }

        [XmlElement("messageid")]
        public int MessageId { get; set; }

        [XmlElement("recipient")]
        public string Recipient { get; set; }

        [XmlElement("messagetype")]
        public string MessageType { get; set; }

        [XmlElement("messagedata")]
        public string MessageData { get; set; }
    }




}
