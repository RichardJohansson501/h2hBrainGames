using h2hBrainGames.GameRules;
using h2hBrainGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace h2hBrainGames.ViewModels
{
    public enum PresentPiece
    {
        blackKing,
        blackQueen,
        blackRook,
        blackBishop,
        blackKnight,
        blackPawn,
        whiteKing,
        whiteQueen,
        whiteRook,
        whiteBishop,
        whiteKnight,
        whitePawn,
        emptySpot,
    }

    public enum SquareColor
    {
        Black,
        White,
    }

    public struct ChessSquare
    {
        public SquareColor squareColor;
        public PresentPiece presentPiece;
    }

    public class PlayChessGame
    {
        public int GameId { get; set; }
        public string NextPlayer { get; set; }
        public Color NextPlayerColor { get; set; }
        public ChessSquare[,] ChessSquare { get; set; }
        public MoveResult Checked { get; set; }
        public MoveResult CheckMate { get; set; }
        public MoveResult QResult { get; set; }
        public string MoveFrom { get; set; }
        public string MoveTo { get; set; }

        public PlayChessGame(ChessGame game)
        {
            GameId = game.Id;
            NextPlayer = game.NextPlayer;
            NextPlayerColor = game.NextPlayerColor;

            ChessSquare = new ChessSquare[8, 8];
            // Color the chess board
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    ChessSquare[i * 2, j * 2].squareColor = SquareColor.Black;
                    ChessSquare[i * 2, j * 2 + 1].squareColor = SquareColor.White;

                    ChessSquare[i * 2 + 1, j * 2].squareColor = SquareColor.White;
                    ChessSquare[i * 2 + 1, j * 2 + 1].squareColor = SquareColor.Black;

                }
            }

            // Init the chess board without any chess pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessSquare[row,col].presentPiece = PresentPiece.emptySpot;
                }
            }

            // Place any chess pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    foreach (var piece in game.ChessPieces)
                    {
                        if (piece.Color == Color.White)
                        {
                            ChessSquare[piece.Row, piece.Column].presentPiece = (PresentPiece)((int)piece.Piece + 6);
                        }
                        else
                        {
                            ChessSquare[piece.Row, piece.Column].presentPiece = (PresentPiece)piece.Piece;
                        }
                    }
                }
            }

            Checked = ChessGameRules.QualifyNotChecked(game.Id, game.NextPlayerColor);
            if (Checked == MoveResult.Checked)
            {
                CheckMate = ChessGameRules.QualifyNotCheckMate(game.Id, game.NextPlayerColor);
            }
        }


    }
}