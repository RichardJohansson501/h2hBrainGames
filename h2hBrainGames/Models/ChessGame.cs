using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace h2hBrainGames.Models
{
    public enum Piece
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn,
        Empty,
    }

    public enum Color
    {
        Black,
        White,
        None,
    }

    public class ChessPiece
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Color Color { get; set; }
        public Piece Piece { get; set; }
        public int ChessGameId { get; set; }

        public ChessPiece()
        {
            // Default contstructor without arguments, used for queries about ongoing games
        }

        public ChessPiece(int row, int column, Color color, Piece piece, int gameid)
        {
            // Constructor used for new games to initialize a chess piece on the board
            Row = row;
            Column = column;
            Color = color;
            Piece = piece;
            ChessGameId = gameid;
        }
    }


    public class ChessGame
    {
        public int Id { get; set; }
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
        public Color Player1Color { get; set; }
        public Color Player2Color { get; set; }
        public string NextPlayer { get; set; }
        public Color NextPlayerColor { get; set; }
        public virtual ICollection<ChessPiece> ChessPieces { get; set; }

        public ChessGame()
        {
            // Default contstructor without arguments, used for queries about ongoing games
        }

        public ChessGame(string player1Id, string player2Id)
        {
            // Constructor used for new games to initialize the chess board
            Player1Id = player1Id;
            Player1Color = Color.White;
            Player2Id = player2Id;
            Player2Color = Color.Black;
            NextPlayer = player1Id;
            NextPlayerColor = Color.White;
        }


    }
}