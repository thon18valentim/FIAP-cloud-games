using App.FCG.WebApi.Models.Dtos;
using FCG.Games.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using FCG.Games.Models;

namespace App.FCG.WebApi.Controllers.v1
{
    [Route("api/games")]
    public class GamesController : MainController
    {
        private readonly IGameRepository _gameRepository;

        public GamesController(IGameRepository gameRepository)
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
        public async Task<ActionResult<IEnumerable<Game>>> GetById(Guid id)
        {
            var game = await _gameRepository.GetById(id);

            if (game is null) return NotFound();

            return Ok(game);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Game>>> Post([FromBody] GameInsertDto game)
        {
            if (!game.IsValid()) 
            {
                game.ValidationResult.Errors.ToList().ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));
                return CustomResponse();
            }

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

        [HttpPut]
        public async Task<ActionResult<IEnumerable<Game>>> Put([FromBody] GameUpdateDto game)
        {
            if (!game.IsValid())
            {
                game.ValidationResult.Errors.ToList().ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));
                return CustomResponse();
            }

            var newGame = new Game(
                game.Name,
                game.Description,
                game.PublisherName,
                game.ReleaseDate,
                game.Price
            );

            _gameRepository.Update(newGame);

            await _gameRepository.UnitOfWork.Commit();

            return CreatedAtAction(nameof(GetById), new { id = newGame.Id }, newGame);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            var game = await _gameRepository.GetById(id);

            if (game is null) return CustomResponse(204);

            _gameRepository.Delete(game);

            await _gameRepository.UnitOfWork.Commit();

            return CustomResponse();
        }
    }
}
