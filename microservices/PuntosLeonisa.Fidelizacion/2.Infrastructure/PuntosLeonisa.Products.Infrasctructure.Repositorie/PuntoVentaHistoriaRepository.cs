﻿using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class PuntoVentaHistoriaRepository : Repository<PuntoVentaHistoria>, IPuntoVentaHistoriaRepository
    {
        internal FidelizacionContext _context;
        public PuntoVentaHistoriaRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PuntoVentaHistoria>> GetPuntoVentaHistoriaById(PuntoVentaHistoria data)
        {
            var response = await _context.Set<PuntoVentaHistoria>().Where(x => x.IdPuntoVenta == data.IdPuntoVenta && x.Mes == data.Mes && x.Ano == data.Ano).ToListAsync();
            return response;
        }

        public async Task<IEnumerable<PuntoVentaHistoria>> DeletePuntoVentaHistoriaByMesAndAnio(PuntoVentaHistoria data)
        {
            {
                var registros = await _context.Set<PuntoVentaHistoria>()
                    .Where(x => x.Mes == data.Mes && x.Ano == data.Ano)
                    .ToListAsync();

                _context.Set<PuntoVentaHistoria>().RemoveRange(registros);
                return registros;
            }
        }
    }
}
