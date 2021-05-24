using GEO.Data;
using GEO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GEO.Services
{
    public interface IAddressService
    {
        Task<Direccion> GetAddress(int id);
        Task<Direccion> PutAddress(int id, Direccion direccion);
        Task<Direccion> PostAddress(Direccion direccion);
    }

    public class AddressService : IAddressService
    {
        private readonly GeoContext _context;

        public AddressService(GeoContext context)
        {
            _context = context;
        }

        public async Task<Direccion> GetAddress(int id)
        {
            return await _context.Direcciones.FindAsync(id);
        }

        public async Task<Direccion> PostAddress(Direccion direccion)
        {
            _context.Direcciones.Add(direccion);
            await _context.SaveChangesAsync();
            return direccion;
        }

        public async Task<Direccion> PutAddress(int id, Direccion direccion)
        {
            try
            {
                direccion.Id = id;
                _context.Direcciones.Update(direccion);
                await _context.SaveChangesAsync();
                return direccion;
            } catch (Exception e)
            {
                throw new NullReferenceException(e.Message);
            }
            
        }
    }
}
