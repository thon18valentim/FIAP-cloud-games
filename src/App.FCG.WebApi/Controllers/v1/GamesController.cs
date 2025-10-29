using App.FCG.WebApi.Models.Dtos;
using FCG.Core.Web;
using FCG.Games.Data.Repository;
using FCG.Games.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.FCG.WebApi.Controllers.v1
{
    [Route("api/games")]
    public class GamesController : MainController
    {
        private readonly IGameRepository _gameRepository;

        public GamesController(INotificador notificador, IGameRepository gameRepository, IUser user) : base(notificador, user)
        {
            _gameRepository = gameRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> Get()
        {
            var games = await _gameRepository.GetAll();

            return Ok(games);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IEnumerable<GameGetDto>>> GetById(Guid id)
        {
            var game = await _gameRepository.GetById(id);

            if (game is null) return CustomResponse();

            var result = new GameGetDto
            {
                Name = game.Name,
                Description = game.Description,
                PublisherName = game.PublisherName,
                ReleaseDate = game.ReleaseDate,
                Price = game.Price
            };

            return CustomResponse(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GameInsertDto game)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var newGame = new Game(
                game.Name,
                game.Description,
                game.PublisherName,
                game.ReleaseDate,
                game.Price
            );

            _gameRepository.Insert(newGame);

            await _gameRepository.UnitOfWork.Commit();

            return CreatedAtAction(nameof(GetById), new { id = newGame.Id }, newGame);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<IEnumerable<Game>>> Put([FromRoute] Guid id, [FromBody] GameUpdateDto game)
        {
            if (id != game.Id) return BadRequest();

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var gameUpdate = new Game(
                game.Name,
                game.Description,
                game.PublisherName,
                game.ReleaseDate,
                game.Price
            );

            _gameRepository.Update(gameUpdate);

            await _gameRepository.UnitOfWork.Commit();

            return CustomResponse(gameUpdate);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var game = await _gameRepository.GetById(id);

            if (game is null) return NotFound();

            _gameRepository.Delete(game);

            await _gameRepository.UnitOfWork.Commit();

            return CustomResponse(game);
        }
    }
}
