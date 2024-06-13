using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Scanner;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IUsuarioScannerApplication
    {
        Task<GenericResponse<UsuarioScannerDto>> AddUsuarioScanner(PeticionCedulaDto data);
    }
}
