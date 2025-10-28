using Microsoft.EntityFrameworkCore;
using FCG.Core.Data;
using FCG.Clients.Models;

namespace FCG.Clients.Data.Repository
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly ClientContext _context;

        public ClientRepository(ClientContext context) : base(context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Client?> GetByCpf(string cpf)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Cpf.Numero == cpf);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
