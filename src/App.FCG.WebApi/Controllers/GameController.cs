using App.FCG.WebApi.Models.Dtos;
using FCG.Games.Data.Repository;
using FCG.Games.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.FCG.WebApi.Controllers;


[Route("/[controller]")]
public class GameController : MainController
{
    private readonly IGameRepository _gameRepository;

    public GameController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            return Ok(await _gameRepository.GetAll());

        }
        catch (Exception)
        {
            return CustomResponse();
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] Guid id)
    {
        try
        {
            return Ok(_gameRepository.GetById(id));
        }
        catch (Exception)
        {
            return CustomResponse();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GameInsertDto input)
    {
        try
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var game = Game.Create(
                input.Name,
                input.Description,
                input.PublisherName,
                input.Price
            );

            _gameRepository.Insert(game);
            await _gameRepository.Commit();
            return Ok(game);

        }
        catch (Exception e)
        {
            return CustomResponse();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] GameUpdateDto input)
    {
        try
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var game = await _gameRepository.GetById(input.Id);
            game.Name = input.Name;
            game.Description = input.Description;
            game.PublisherName = input.PublisherName;
            game.Price = input.Price;

            _gameRepository.Update(game);
            await _gameRepository.Commit();

            return Ok(game);

        }
        catch (Exception e)
        {
            return CustomResponse();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Game game)
    {
        try
        {
            _gameRepository.Delete(game);
            await _gameRepository.Commit();

            return Ok();
        }
        catch (Exception e)
        {
            return CustomResponse();
        }
    }

}
