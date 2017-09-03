using h2hBrainGames.GameRules;
using h2hBrainGames.Models;
using h2hBrainGames.Repositories;
using h2hBrainGames.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace h2hBrainGames.Controllers
{
    public class ChessGamesController : Controller
    {
        // GET: ChessGames
        public ActionResult Index()
        {

            return View();
        }

        // GET: ChessGames/NewGame
        public ActionResult NewGame(string player1Id, string player2Id)
        {
            ChessGame game = ChessGameRepo.RetrieveNewGame(player1Id, player2Id);
            PlayChessGame viewModel = new PlayChessGame(game);
            return View("PlayGame", viewModel);           
        }

        // GET: ChessGames/RefreshGame
        public ActionResult RefreshGame(int? gameId)
        {
            if ((gameId == null) || (gameId == 0))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ChessGame game = ChessGameRepo.RetrieveGame((int)gameId);
            PlayChessGame viewModel = new PlayChessGame(game);
            return View("PlayGame", viewModel);
        }

        // GET: ChessGames/PlayGame
        [OutputCache(Duration = 600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public ActionResult PlayGame(int? gameId)
        {
            if ((gameId == null) || (gameId == 0))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ChessGame game = ChessGameRepo.RetrieveGame((int)gameId);
            PlayChessGame viewModel = new PlayChessGame(game);
            return View(viewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PlayGame([Bind (Include = "gameId, from, to")] int? gameId, string from, string to)
        {
            if ((gameId == null) || (gameId == 0))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // "from" and "to" parameters are expected to be in format XN, e.g. A1, C3 etc
            from = from.ToUpper();
            to = to.ToUpper();
            var fromRow = (int)(from.First() - 'A');
            var fromCol = (int)(from.Last() - '1');
            var toRow = (int)(to.First() - 'A');
            var toCol = (int)(to.Last() - '1');

            ChessGame game = ChessGameRepo.RetrieveGame((int)gameId);

            var qResult = ChessGameRules.QualifyMove(game.Id, fromRow, fromCol, toRow, toCol, game.NextPlayerColor);
            if (qResult == MoveResult.Success)
            {
                qResult = ChessGameRules.QualifyNotSelfChecked(game.Id, fromRow, fromCol, toRow, toCol, game.NextPlayerColor);
            }

            if (qResult == MoveResult.Success)
            {            
                ChessGameRepo.UpdateGame(game, fromRow, fromCol, toRow, toCol);
            }
           
            PlayChessGame viewModel = new PlayChessGame(game);
            viewModel.QResult = qResult;
            viewModel.MoveFrom = from;
            viewModel.MoveTo = to;
            return View(viewModel);
        }
    }
}