using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CBBL.src.AI;
using CBBL.src.Board;
using CBBL.src.Implementation;
using CBBL.src.Interfaces;
using CBBL.src.Pieces;
using CBBL.src.Util;

namespace CBBTop.AI;

public class Player(BoardState boardState, PlayerColor color, int maxDepth = 3, 
    float pieceValueMult = 1f, float positionValMult = 1f) : IAIPlayer
{
    public PlayerColor Color { get; } = color;
    public int MaxDepth { get; } = maxDepth;

    private readonly BoardState _boardState = boardState;
    private readonly float _pieceValueMult = pieceValueMult;
    private readonly float _positionalValueMult = positionValMult;

    public async Task<Move?> GetBestMoveAsync()
    {
        return await Task.Run(GetBestMove);
    }

    private IEnumerable<Move> OrderMoves(ref IEnumerable<Move> moves)
    {
        List<(Move move, float score)> scoredMoves = [];

        foreach (var move in moves)
        {
            float score = 0f;

            int attackerTypeInt = _boardState.GetPieceAtSquare(move.FromSquare);
            int victimTypeInt = _boardState.GetPieceAtSquare(move.ToSquare);

            if (move.Flag.HasFlag(MoveFlag.Capture) && victimTypeInt != -1 && attackerTypeInt != -1)
            {
                var victim = (PieceType)victimTypeInt;
                var attacker = (PieceType)attackerTypeInt;

                int victimValue = (int)(GetPieceValue(victim) * 10);
                int attackerValue = (int)GetPieceValue(attacker);

                // MVV-LVA: most valuable victim - least valuable attacker
                score += victimValue - attackerValue;
            }

            if (move.Flag.HasFlag(MoveFlag.Promotion))
            {
                score += move.PromotionPieceType switch
                {
                    PieceType.WhiteQueen or PieceType.BlackQueen => 900,
                    PieceType.WhiteRook or PieceType.BlackRook => 500,
                    PieceType.WhiteBishop or PieceType.BlackBishop => 300,
                    PieceType.WhiteKnight or PieceType.BlackKnight => 300,
                    _ => 0
                };
            }

            // Positional bonus
            if (attackerTypeInt != -1)
            {
                var pieceType = (PieceType)attackerTypeInt;
                score += PositionTable.GetValue(pieceType, move.ToSquare);
            }

            scoredMoves.Add((move, score));
        }

        // Order best moves first
        moves = scoredMoves.OrderByDescending(m => m.score).Select(m => m.move).ToList();
        return moves;
    }

    public Move? GetBestMove()
    {
        string fen = _boardState.GetFen();
        float alpha = float.MinValue, beta = float.MaxValue;
        Move? result = null;

        var moves = _boardState.GetMoves();
        OrderMoves(ref moves);

        var copy = new CBBLBoardState(fen);

        foreach (var move in moves)
        {
            copy.ExecuteMove(move);
            copy.UpdateMoves();

            float eval = Minmax(copy, MaxDepth - 1, alpha, beta);
            copy.UndoMove();

            if (copy.ColorToMove == PlayerColor.White)
            {
                if (eval > alpha) { alpha = eval; result = move; }
            }
            else
            {
                if (eval < beta) { beta = eval; result = move; }
            }
            if (beta <= alpha) break; 
        }

        return result;
    }

    private float Minmax(CBBLBoardState copy, int depth, float alpha, float beta)
    {
        if (depth == 0 || copy.IsKingInCheckmate(copy.ColorToMove))
        {
            return EvaluateBoard(copy, copy.ColorToMove);
        }

        bool isWhite = copy.ColorToMove == PlayerColor.White;

        IEnumerable<Move> moves = copy.GetMoves();
        OrderMoves(ref moves);
        foreach (var move in moves)
        {
            if (!copy.ExecuteMove(move))
                continue;
            copy.UpdateMoves();
            float score = Minmax(copy, depth - 1, alpha, beta);
            copy.UndoMove();

            if (isWhite)
            {
                alpha = Math.Max(alpha, score);
                if (beta <= alpha)
                    break; // Beta cut-off
            }
            else
            {
                beta = Math.Min(beta, score);
                if (beta <= alpha)
                    break; // Alpha cut-off
            }
        }

        return isWhite ? alpha : beta;
    }

    private float EvaluateBoard(CBBLBoardState copy, PlayerColor perspective)
    {
        float score = 0f;

        for (int i = 0; i < BoardGlobals.Instance.NumPieces; i++)
        {
            var pieceType = (PieceType)i;
            ulong bitboard = copy.GetBitboardFor(pieceType);

            var pieceValue = GetPieceValue(pieceType, bitboard) * _pieceValueMult;

            var positionalValue = GetPositionalValue(pieceType, bitboard) * _positionalValueMult;

            // var captureValue = GetCaptureValue(pieceType, copy, bitboard);

            bool piece_is_white = Pieces.WhitePieces.Contains(pieceType);

            if (perspective == PlayerColor.White)
            {
                score += piece_is_white ? (pieceValue + positionalValue) : -(pieceValue + positionalValue);
            }
            else
            {
                score += piece_is_white ? -(pieceValue + positionalValue) : (pieceValue + positionalValue);
            }
        }

        if (copy.IsKingInCheckmate(perspective))
            return float.NegativeInfinity;
        else if (copy.IsKingInCheckmate(BoardUtils.Opposite(perspective)))
            return float.PositiveInfinity;
        else if (copy.IsStalemate())
            return 0f;

        return score;
    }

    private float GetPieceValue(PieceType pieceType, ulong occupancy)
    {
        float score = 0f;
        while (occupancy != 0)
        {
            BoardOps.PopLsb(ref occupancy);
            score += GetPieceValue(pieceType);
        }
        return score;
    }

    private float GetPositionalValue(PieceType pieceType, ulong occupancy)
    {
        float score = 0f;
        while (occupancy != 0)
        {
            int square = BoardOps.PopLsb(ref occupancy);
            score += PositionTable.GetValue(pieceType, square);
        }
        return score;
    }

    private float GetCaptureValue(PieceType pieceType, BoardState copy, ulong occupancy)
    {
        float score = 0f;
        var enemyColor = copy.ColorToMove == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        var enemyMoves = Context.Instance.MoveGenerator.GenerateMoves(copy, MoveType.LEGAL, enemyColor);
        var moves = copy.GetMoves();

        List<int> squares = [];
        while (occupancy != 0)
        {
            int square = BoardOps.PopLsb(ref occupancy);
            squares.Add(square);
        }

        foreach (var move in enemyMoves)
        {
            if (squares.Contains(move.ToSquare))
            {
                score -= GetPieceValue(pieceType);
            }
        }

        foreach (var move in moves)
        {
            var enemyType = copy.GetPieceAtSquare(move.ToSquare);
            if (enemyType != -1)
            {
                score += GetPieceValue((PieceType)enemyType);
            }
        }
        return score;
    }

    private float GetPieceValue(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.WhitePawn => 1f,
            PieceType.BlackPawn => 1f,
            PieceType.WhiteKnight => 3f,
            PieceType.BlackKnight => 3f,
            PieceType.WhiteBishop => 3f,
            PieceType.BlackBishop => 3f,
            PieceType.WhiteRook => 5f,
            PieceType.BlackRook => 5f,
            PieceType.WhiteQueen => 9f,
            PieceType.BlackQueen => 9f,
            PieceType.WhiteKing => 120f,
            PieceType.BlackKing => 120f,
            _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null)
        };
    }
}