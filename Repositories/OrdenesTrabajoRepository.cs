using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class OrdenesTrabajoRepository : Repository<OrdenesTrabajo>, IOrdenesTrabajoRepository
    {
        public OrdenesTrabajoRepository(TallerMecanicoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrdenesTrabajo>> GetOrdenesConDetallesAsync()
        {
            return await _dbSet
                .Include(o => o.Cliente)
                .Include(o => o.Mecanico)
                .Include(o => o.Vehiculo)
                .ToListAsync();
        }

        public async Task<OrdenesTrabajo?> GetOrdenConDetallesByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Cliente)
                .Include(o => o.Mecanico)
                .Include(o => o.Vehiculo)
                .FirstOrDefaultAsync(o => o.OrdenId == id);
        }
    }
}
