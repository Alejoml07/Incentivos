using PuntosLeonisa.Seguridad.Domain.Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.Interfaces
{
    public interface ISecurityService
    {
        string HasPassword(string? password);
        PasswordVerifyResult VerifyPassword(string password, string? pwd);
    }
}
