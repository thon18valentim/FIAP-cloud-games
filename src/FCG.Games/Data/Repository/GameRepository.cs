using Microsoft.EntityFrameworkCore;
using FCG.Core.Data;
using FCG.Games.Models;

namespace FCG.Games.Data.Repository;

public class GameRepository : GenericRepository<Game> , IGameRepository
{
    private readonly GameContext _context;
    public GameRepository(GameContext context) : base(context)
    {
        _context = context; 
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Game?> GetByName(string name)
    {
        return await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
    }
     
    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<bool> Commit()
    {
       return await _context.Commit();
    }
}
