using h2hBrainGames.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace h2hBrainGames.Repositories
{
    public class ChessGameRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // ADD all chess pieces for a new chess game
        public static void AddChessPieces(int gameId)
        {
            ChessPiece chessPiece;
            for (int i = 0; i < 8; i++)
            {
                chessPiece = new ChessPiece(1, i, Color.White, Piece.Pawn, gameId);
                db.ChessPieces.Add(chessPiece);
                db.SaveChanges();
       
                chessPiece = new ChessPiece(6, i, Color.Black, Piece.Pawn, gameId);
                db.ChessPieces.Add(chessPiece);
                db.SaveChanges();             
            }

            chessPiece = new ChessPiece(0, 0, Color.White, Piece.Rook, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 1, Color.White, Piece.Knight, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 2, Color.White, Piece.Bishop, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 3, Color.White, Piece.Queen, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 4, Color.White, Piece.King, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 5, Color.White, Piece.Bishop, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 6, Color.White, Piece.Knight, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(0, 7, Color.White, Piece.Rook, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 0, Color.Black, Piece.Rook, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 1, Color.Black, Piece.Knight, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 2, Color.Black, Piece.Bishop, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 3, Color.Black, Piece.Queen, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 4, Color.Black, Piece.King, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 5, Color.Black, Piece.Bishop, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 6, Color.Black, Piece.Knight, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();

            chessPiece = new ChessPiece(7, 7, Color.Black, Piece.Rook, gameId);
            db.ChessPieces.Add(chessPiece);
            db.SaveChanges();
        }

        //RETRIEVE a chess piece at a defined location
        public static ChessPiece RetrieveChessPiece(int gameId, int row, int column)
        {
            var chessPiece = db.ChessPieces.Where(cp => cp.ChessGameId == gameId).Where(cp => cp.Row == row)
                                .Where(cp => cp.Column == column).ToList();

            if (chessPiece.Count() == 0)
                return null;
            return chessPiece.First();
        }

        //RETRIEVE a chess piece at a defined location and of a specific color
        public static ChessPiece RetrieveChessPieceColor(int gameId, int row, int column, Color color)
        {
            var chessPiece = db.ChessPieces.Where(cp => cp.ChessGameId == gameId).Where(cp => cp.Row == row)
                                .Where(cp => cp.Column == column).Where(cp => cp.Color == color).ToList();

            if (chessPiece.Count() == 0)
                return null;
            return chessPiece.First();
        }

        //RETRIEVE a king chess piece of a specific color
        public static ChessPiece RetrieveKingPiece(int gameId, Color color)
        {
            var chessPiece = db.ChessPieces.Where(cp => cp.ChessGameId == gameId).Where(cp => cp.Color == color)
                                .Where(cp => cp.Piece == Piece.King).ToList();

            if (chessPiece.Count() == 0)
                return null;
            return chessPiece.First();
        }

        //RETRIEVE a list of the chess pieces of a specific color
        public static List<ChessPiece> RetrieveChessPiecesOfColor(int gameId, Color color)
        {
            var chessPieces = db.ChessPieces.Where(cp => cp.ChessGameId == gameId).Where(cp => cp.Color == color).ToList();

            if (chessPieces.Count() == 0)
                return null;
            return chessPieces;
        }


        // RETRIEVE a new chess game
        public static ChessGame RetrieveNewGame(string player1Id, string player2Id)
        {
            var game = new ChessGame(player1Id, player2Id);
            db.ChessGames.Add(game);
            db.SaveChanges();

            AddChessPieces(game.Id);
            return game;
        }

        // RETRIEVE an existing game
        public static ChessGame RetrieveGame(int gameId)
        {
            var game = db.ChessGames.Find(gameId);         
            return game;
        }

        // UPDATE an existing game
        public static void UpdateGame(ChessGame game, int fromRow, int fromCol, int toRow, int toCol)
        {
            var chessPiece = RetrieveChessPiece(game.Id, toRow, toCol);
            if (chessPiece != null)
            {
                db.ChessPieces.Remove(chessPiece);          // Remove a chesspice that is hit.
                db.SaveChanges();
            }

            chessPiece = RetrieveChessPiece(game.Id, fromRow, fromCol);
            chessPiece.Row = toRow;
            chessPiece.Column = toCol;
            db.Entry(chessPiece).State = EntityState.Modified;
            db.SaveChanges();

            if (game.NextPlayerColor == Color.White)
            {
                game.NextPlayerColor = Color.Black;
            }
            else
            {
                game.NextPlayerColor = Color.White;
            }
            if (game.NextPlayer == game.Player1Id)
            {
                game.NextPlayer = game.Player2Id;
            }
            else
            {
                game.NextPlayer = game.Player1Id;
            }
            db.Entry(game).State = EntityState.Modified;
            db.SaveChanges();
        }

        // UPDATE an existing chess piece temporarily. Used for checked testing.
        public static ChessPiece UpdateChessPiece(int gameId, int fromRow, int fromCol, int toRow, int toCol)
        {
            //var chessPiece = RetrieveChessPieceColor(gameId, fromRow, fromCol, color);
            var chessPiece = RetrieveChessPiece(gameId, fromRow, fromCol);
            if (chessPiece != null)
            {
                chessPiece.Row = toRow;
                chessPiece.Column = toCol;
                db.Entry(chessPiece).State = EntityState.Modified;
                db.SaveChanges();
            }

            return chessPiece;
        }
    }
}