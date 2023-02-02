using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShitheadApi.Attributes;
using ShitheadApi.Models;
using ShitheadApi.Models.Entities;

namespace ShitheadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GameController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;

        public GameController(DatabaseContext databaseContext,
                              UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;;
        }

        #region Game

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Game>> AddGame(Game game)
        {
            var errorMessage = "Couldn't create game";
            DataBundel<Game> errorResult = new(null, false, errorMessage);

            try
            {
                await _databaseContext.Game.AddAsync(game);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Game>> UpdateGame(Game game)
        {
            var errorMessage = "Couldn't create game";
            DataBundel<Game> errorResult = new(null, false, errorMessage);

            try
            {
                _databaseContext.Game.Update(game);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Game>> RemoveGame(Game game)
        {
            var errorMessage = "Couldn't create game";
            DataBundel<Game> errorResult = new(null, false, errorMessage);

            try
            {
                _databaseContext.Game.Remove(game);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpGet]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Game>> GetGame(Guid id)
        {
            var errorMessage = "Couldn't create game";
            DataBundel<Game> errorResult = new(null, false, errorMessage);

            try
            {
                var result = await _databaseContext.Game
                                                    .Include(g => g.Players)
                                                        .ThenInclude(p => p.User)
                                                    .Include(g => g.Cards)
                                                    .FirstOrDefaultAsync(g => g.Players.FirstOrDefault(p => p.UserId == id) != null);

                return result != null ? new(result, true, null) : new(null, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        #endregion

        #region Player

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<List<Player>>> AddPlayers(List<Player> players)
        {
            var errorMessage = "Couldn't add player";
            DataBundel<List<Player>> errorResult = new(null, false, errorMessage);

            try
            {
                await _databaseContext.Player.AddRangeAsync(players);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Player>> UpdatePlayer(Player player)
        {
            var errorMessage = "Couldn't update player";
            DataBundel<Player> errorResult = new(null, false, errorMessage);

            try
            {
                player.Game = null;
                player.User = null;
                _databaseContext.Player.Update(player);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Player>> UpdatePlayers(List<Player> players)
        {
            var errorMessage = "Couldn't update players";
            DataBundel<Player> errorResult = new(null, false, errorMessage);

            try
            {
                players.ForEach(p => { p.Game = null; p.User = null; });
                _databaseContext.Player.UpdateRange(players);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<Player>> RemovePlayer(Player player)
        {
            var errorMessage = "Couldn't remove player";
            DataBundel<Player> errorResult = new(null, false, errorMessage);

            try
            {
                var result = await _databaseContext.Game
                                                   .Include(g => g.Players)
                                                   .Include(g => g.Cards)
                                                   .FirstOrDefaultAsync(x => x.Players.Any(p => p.Id == player.Id));

                if (result == null) return errorResult;

                _databaseContext.Card.RemoveRange(result.Cards.Where(c => c.PlayerId == player.Id));
                _databaseContext.Player.Remove(result.Players.FirstOrDefault(p => p.Id == player.Id));

                if (result.Players.Count == 1)
                {
                    result.Players = null;
                    result.Cards = null;

                    _databaseContext.Game.Remove(result);
                }

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        #endregion

        #region Card

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<List<Card>>> AddCards(List<Card> cards)
        {
            var errorMessage = "Couldn't add cards";
            DataBundel<List<Card>> errorResult = new(null, false, errorMessage);

            try
            {
                await _databaseContext.Card.AddRangeAsync(cards);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<List<Card>>> UpdateCards(List<Card> cards)
        {
            var errorMessage = "Couldn't update cards";
            DataBundel<List<Card>> errorResult = new(null, false, errorMessage);

            try
            {
                var result = _databaseContext.Card.Where(c => c.GameId == cards[0].GameId).ToList();
                cards.ForEach(c => 
                { 
                    c.Player = null;
                    c.Game = null;

                    //Origin: https://stackoverflow.com/a/26752977/6572268
                    int i = cards.IndexOf(c);

                    if (i != -1)
                    {
                        result[i].State = c.State;
                        result[i].Seven = c.Seven;
                        result[i].Excluded = c.Excluded;
                        result[i].Order = c.Order;
                        result[i].PlayerId = c.PlayerId;
                    }
                });

                _databaseContext.Card.UpdateRange(result);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<List<Card>>> RemoveCards(List<Card> cards)
        {
            var errorMessage = "Couldn't remove cards";
            DataBundel<List<Card>> errorResult = new(null, false, errorMessage);

            try
            {
                _databaseContext.Card.RemoveRange(cards);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        #endregion
    }
}
