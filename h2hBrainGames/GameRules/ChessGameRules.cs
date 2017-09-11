//using h2hBrainGames.Migrations;
using h2hBrainGames.Models;
using h2hBrainGames.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace h2hBrainGames.GameRules
{
    public enum MoveResult
    {
        Success,
        NoMove,
        MoveFromOutside,
        MoveToOutside,
        EmptySpot,
        WrongTurn,
        MoveOnOwn,
        MovePatternErr,
        MovePathErr,
        SelfChecked,       
    }

    public enum CheckStatus
    {
        NotChecked,
        Checked,
        NotCheckMate,
        CheckMate,
    }

    public class ChessGameRules
    {
        public static MoveResult QualifyMove(int gameId, int fromRow, int fromCol, int toRow, int toCol, Color nextPlayerColor)
        {
            var qResult = QualifyBoardMove(fromRow, fromCol, toRow, toCol);
            if (qResult != MoveResult.Success)
                return qResult;

            var chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, fromRow, fromCol);
            qResult = QualifyPiece(chessPiece, nextPlayerColor);
            if (qResult != MoveResult.Success)
                return qResult;

            qResult = QualifyFreeMove(fromRow, fromCol, toRow, toCol, chessPiece);
            if (qResult != MoveResult.Success)
                return qResult;

            qResult = QualifyPath(fromRow, fromCol, toRow, toCol, gameId);
            if (qResult != MoveResult.Success)
                return qResult;

            chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, toRow, toCol);
            qResult = QualifyDest(chessPiece, nextPlayerColor);
            if (qResult != MoveResult.Success)
                return qResult;

            return MoveResult.Success;
        }

        private static MoveResult QualifyBoardMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            // Check against moving from outside the chess board
            if (fromRow < 0 || fromRow > 7 || fromCol < 0 || fromCol > 7)
                return MoveResult.MoveFromOutside;

            // Check against moving outside the chess board
            if (toRow < 0 || toRow > 7 || toCol < 0 || toCol > 7)
                return MoveResult.MoveToOutside;

            // Check for actual move
            if (fromRow == toRow && fromCol == toCol)
                return MoveResult.NoMove;

            return MoveResult.Success;
        }

        private static MoveResult QualifyPiece(ChessPiece piece, Color nextPlayerColor)
        {
            // Check that concerned square has a piece
            if (piece == null)
                return MoveResult.EmptySpot;

            // Check that the piece is of correct color
            if (piece.Color != nextPlayerColor)
                return MoveResult.WrongTurn;

            return MoveResult.Success;
        }

        private static MoveResult QualifyFreeMove(int fromRow, int fromCol, int toRow, int toCol, ChessPiece piece)
        {
            MoveResult result = MoveResult.Success;

            switch (piece.Piece)
            {
                case Piece.King:
                    if ((Math.Abs(toCol - fromCol) > 1) || (Math.Abs(toRow - fromRow) > 1))
                    {
                        result = MoveResult.MovePatternErr;
                    }
                    break;
                case Piece.Queen:
                    if ((Math.Abs(toCol - fromCol) > 0) && (Math.Abs(toRow - fromRow) > 0) &&
                        (Math.Abs(toCol - fromCol) != Math.Abs(toRow - fromRow)))
                    {
                        result = MoveResult.MovePatternErr;
                    }
                    break;
                case Piece.Rook:
                    if ((Math.Abs(toCol - fromCol) > 0) && (Math.Abs(toRow - fromRow) > 0))
                    {
                        result = MoveResult.MovePatternErr;
                    }
                    break;
                case Piece.Bishop:
                    if (Math.Abs(toCol - fromCol) != Math.Abs(toRow - fromRow))
                    {
                        result = MoveResult.MovePatternErr;
                    }
                    break;
                case Piece.Knight:
                    if ((Math.Abs(toCol - fromCol) == 0) || (Math.Abs(toRow - fromRow) == 0) ||
                        (Math.Abs(toCol - fromCol) == 1) && (Math.Abs(toRow - fromRow) != 2) ||
                        (Math.Abs(toCol - fromCol) == 2) && (Math.Abs(toRow - fromRow) != 1) ||
                        (Math.Abs(toCol - fromCol) > 2) || (Math.Abs(toRow - fromRow) > 2))
                    {
                        result = MoveResult.MovePatternErr;
                    }
                    break;
                case Piece.Pawn:
                    result = MoveResult.MovePatternErr;
                    if (Math.Abs(toCol - fromCol) == 0)
                    {
                        if (piece.Color == Color.White)
                        {
                            if ((toRow - fromRow == 1) || ((fromRow == 1) && (toRow - fromRow == 2)))
                                result = MoveResult.Success;                                                
                        }
                        if (piece.Color == Color.Black)
                        {                           
                            if ((fromRow - toRow == 1) || ((fromRow == 6) && (fromRow - toRow == 2)))
                                result = MoveResult.Success;                             
                        }
                    }
                    if (Math.Abs(toCol - fromCol) == 1)
                    {
                        if ((piece.Color == Color.White) && (toRow - fromRow == 1))
                        {
                            result = MoveResult.Success;
                        }
                        if ((piece.Color == Color.Black) && (fromRow - toRow == 1))
                        {
                            result = MoveResult.Success;
                        }
                    }
                    break;
            }
            return result;
        }

        private static MoveResult QualifyRowPath(int row, int fromCol, int toCol, int gameId)
        {
            ChessPiece chessPiece;
            if (fromCol > toCol)
            {
                for (int col = toCol + 1; col < fromCol; col++)
                {
                    chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, row, col);
                    if (chessPiece != null)
                        return MoveResult.MovePathErr;
                }
            }
            else
            {
                for (int col = fromCol + 1; col < toCol; col++)
                {
                    chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, row, col);
                    if (chessPiece != null)
                        return MoveResult.MovePathErr;
                }
            }
            return MoveResult.Success;
        }

        private static MoveResult QualifyColumnPath(int col, int fromRow, int toRow, int gameId)
        {
            ChessPiece chessPiece;
            if (fromRow > toRow)
            {
                for (int row = toRow + 1; row < fromRow; row++)
                {
                    chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, row, col);
                    if (chessPiece != null)
                        return MoveResult.MovePathErr;
                }
            }
            else
            {
                for (int row = fromRow + 1; row < toRow; row++)
                {
                    chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, row, col);
                    if (chessPiece != null)
                        return MoveResult.MovePathErr;
                }
            }
            return MoveResult.Success;
        }

        private static MoveResult QualifyDiagonalPath(int fromRow, int fromCol, int toRow, int toCol, int gameId)
        {
            ChessPiece chessPiece;
            if (fromRow > toRow)
            {
                for (int row = toRow + 1; row < fromRow; row++)
                {
                    if (toCol < fromCol)
                    {
                        toCol++;
                    }
                    else
                    {
                        toCol--;
                    }
                    chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, row, toCol);
                    if (chessPiece != null)
                        return MoveResult.MovePathErr;
                }
            }
            else
            {
                for (int row = fromRow + 1; row < toRow; row++)
                {
                    if (fromCol < toCol)
                    {
                        fromCol++;
                    }
                    else
                    {
                        fromCol--;
                    }
                    chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, row, fromCol);
                    if (chessPiece != null)
                        return MoveResult.MovePathErr;
                }
            }
            return MoveResult.Success;
        }


        private static MoveResult QualifyPath(int fromRow, int fromCol, int toRow, int toCol, int gameId)
        {
            MoveResult result = MoveResult.Success;

            var chessPiece = ChessGameRepo.RetrieveChessPiece(gameId, fromRow, fromCol);
            switch (chessPiece.Piece)
            {
                case Piece.King:
                    // The king has no path move limitations
                    break;
                case Piece.Queen:
                    if ((Math.Abs(toCol - fromCol) > 1) && (Math.Abs(toRow - fromRow) > 1))
                    {
                        result = QualifyDiagonalPath(fromRow, fromCol, toRow, toCol, gameId);
                    }
                    else
                    {
                        if (Math.Abs(toCol - fromCol) > 1)
                        {
                            result = QualifyRowPath(fromRow, fromCol, toCol, gameId);
                        }
                        if (Math.Abs(toRow - fromRow) > 1)
                        {
                            result = QualifyColumnPath(fromCol, fromRow, toRow, gameId);
                        }
                    }
                    break;
                case Piece.Rook:
                    if (Math.Abs(toCol - fromCol) > 1)
                    {
                        result = QualifyRowPath(fromRow, fromCol, toCol, gameId);
                    }
                    if (Math.Abs(toRow - fromRow) > 1)
                    {
                        result = QualifyColumnPath(fromCol, fromRow, toRow, gameId);
                    }
                    break;
                case Piece.Bishop:
                    if ((Math.Abs(toCol - fromCol) > 1) && (Math.Abs(toRow - fromRow) > 1))
                    {
                        result = QualifyDiagonalPath(fromRow, fromCol, toRow, toCol, gameId);
                    }
                    break;
                case Piece.Knight:
                    // The knight has no path move limitations
                    break;
                case Piece.Pawn:
                    if (Math.Abs(toCol - fromCol) == 0)
                    {
                        var otherChessPiece = ChessGameRepo.RetrieveChessPiece(gameId, toRow, toCol);
                        if (otherChessPiece != null)
                            result = MoveResult.MovePathErr;

                        if ((chessPiece.Color == Color.White) && (toRow - fromRow == 2))
                        {
                            otherChessPiece = ChessGameRepo.RetrieveChessPiece(gameId, fromRow + 1, fromCol);
                            if (otherChessPiece != null)
                                result = MoveResult.MovePathErr;
                        }
                        if ((chessPiece.Color == Color.Black) && (fromRow - toRow == 2))
                        {
                            otherChessPiece = ChessGameRepo.RetrieveChessPiece(gameId, fromRow - 1, fromCol);
                            if (otherChessPiece != null)
                                result = MoveResult.MovePathErr;                         
                        }
                    }
                    if (Math.Abs(toCol - fromCol) == 1)
                    {
                        var otherChessPiece = ChessGameRepo.RetrieveChessPiece(gameId, toRow, toCol);
                        if (otherChessPiece == null)
                            result = MoveResult.MovePathErr;
                    }
                    break;
            }

            return result;
        }

        private static MoveResult QualifyDest(ChessPiece piece, Color nextPlayerColor)
        {
            // Check for concerned square is empty
            if (piece == null)
                return MoveResult.Success;

            // Check that concerned square has no own piece
            if (piece.Color == nextPlayerColor)
                return MoveResult.MoveOnOwn;

            return MoveResult.Success;
        }

        public static CheckStatus QualifyNotChecked(int gameId, Color nextPlayerColor)
        {          
            var myKing = ChessGameRepo.RetrieveKingPiece(gameId, nextPlayerColor);
            
            Color otherPlayerColor = Color.White;
            if (nextPlayerColor == Color.White)
                otherPlayerColor = Color.Black;
            var otherPlayerPieces = ChessGameRepo.RetrieveChessPiecesOfColor(gameId, otherPlayerColor);

            CheckStatus result = CheckStatus.NotChecked;
            foreach (var piece in otherPlayerPieces)
            {
                if (QualifyMove(gameId, piece.Row, piece.Column, myKing.Row, myKing.Column, otherPlayerColor) == MoveResult.Success)
                    result = CheckStatus.Checked;
            }

            return result;
        }

        public static MoveResult QualifyNotSelfChecked(int gameId, int fromRow, int fromCol, int toRow, int toCol, Color nextPlayerColor)
        {
            MoveResult testChecked = MoveResult.Success;
            // Temporary removal of any other chess piece on concerned square
            var hitChessPiece = ChessGameRepo.UpdateChessPiece(gameId, toRow, toCol, -1, -1);

            // Temporary move of concerned chess piece
            var myChessPiece = ChessGameRepo.UpdateChessPiece(gameId, fromRow, fromCol, toRow, toCol);

            var myKing = ChessGameRepo.RetrieveKingPiece(gameId, nextPlayerColor);

            Color otherPlayerColor = Color.White;
            if (nextPlayerColor == Color.White)
                otherPlayerColor = Color.Black;
            var otherPlayerPieces = ChessGameRepo.RetrieveChessPiecesOfColor(gameId, otherPlayerColor);

            foreach (var piece in otherPlayerPieces)
            {
                // A temporarily moved chess piece will not qualify due to moving from outside the chess board
                if (QualifyMove(gameId, piece.Row, piece.Column, myKing.Row, myKing.Column, otherPlayerColor) == MoveResult.Success)
                    testChecked = MoveResult.SelfChecked;
            }

            // Roll-back of concerned chess piece
            myChessPiece = ChessGameRepo.UpdateChessPiece(gameId, toRow, toCol, fromRow, fromCol);

            // Roll-back of any hit chess piece to concerned square
            if (hitChessPiece != null)
            {
                hitChessPiece = ChessGameRepo.UpdateChessPiece(gameId, -1, -1, toRow, toCol);
            }
            return testChecked;
        }

        public static CheckStatus QualifyNotCheckMate(int gameId, Color nextPlayerColor)
        {
            // 1. Try to move the king to avoid being checked
            var myKing = ChessGameRepo.RetrieveKingPiece(gameId, nextPlayerColor);

            for (int rowAdj = -1; rowAdj < 2; rowAdj++)
            {
                for (int colAdj = -1; colAdj < 2; colAdj++)
                {
                    if (QualifyMove(gameId, myKing.Row, myKing.Column, myKing.Row + rowAdj, myKing.Column + colAdj, nextPlayerColor) == MoveResult.Success)
                    {
                        var result = QualifyNotSelfChecked(gameId, myKing.Row, myKing.Column, myKing.Row + rowAdj, myKing.Column + colAdj, nextPlayerColor);
                        if (result == MoveResult.Success)
                        {
                            return CheckStatus.NotCheckMate;
                        }
                    }
                }
            }

            // 2. Check if more than one chess piece checks the king
            Color otherPlayerColor = Color.White;
            if (nextPlayerColor == Color.White)
                otherPlayerColor = Color.Black;
            var otherPlayerPieces = ChessGameRepo.RetrieveChessPiecesOfColor(gameId, otherPlayerColor);

            int noOfCheckers = 0;
            int checkerIndex = 0;
            int index = 0;
            foreach (var piece in otherPlayerPieces)
            {
                if (QualifyMove(gameId, piece.Row, piece.Column, myKing.Row, myKing.Column, otherPlayerColor) == MoveResult.Success)
                {
                    noOfCheckers++;
                    checkerIndex = index;
                }
                index++;
            }
            if (noOfCheckers > 1)       // If more than one, impossible to hit back or cover king. i.e. checkmate
                return CheckStatus.CheckMate;

            // 3. Try to hit the single chess piece that checks the king or put own chess piece in between
            ChessPiece checker = otherPlayerPieces.ElementAt(checkerIndex);
            var myPieces = ChessGameRepo.RetrieveChessPiecesOfColor(gameId, nextPlayerColor);

            int row = checker.Row;
            int rowStep = 0;
            if (checker.Row < myKing.Row)
                rowStep = 1;
            if (checker.Row > myKing.Row)
                rowStep = -1;
            int col = checker.Column;
            int colStep = 0;
            if (checker.Column < myKing.Column)
                colStep = 1;
            if (checker.Column > myKing.Column)
                colStep = -1;

            while ((row != myKing.Row) || (col != myKing.Column))
            {
                foreach (var piece in myPieces)
                {
                    if (piece.Piece != Piece.King)
                    {
                        if (QualifyMove(gameId, piece.Row, piece.Column, row, col, nextPlayerColor) == MoveResult.Success)
                        {
                            return CheckStatus.NotCheckMate;
                        }
                    }
                }
                row += rowStep;
                col += colStep;
            }
            return CheckStatus.CheckMate;
        }

    }
}