using FCG.Games.Models;
using FCG.Core.Data;

namespace FCG.Games.Data.Repository
{
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        private readonly GameContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public GameRepository(GameContext context) : base(context)
        {
            _context = context;
        }
    }
}
