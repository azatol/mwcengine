using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class Engine : Agent, IEngine
    {
        public Dictionary<int, List<int>> neighbors;

        public decimal pawnNeighborWeight;
        public decimal advancedRankWeight;
        public decimal[] pieceScore;

        public Engine(decimal[] pieceScore, decimal pawnNeighborWeight, decimal advancedRankWeight)
        {
            this.pieceScore = pieceScore;
            this.pawnNeighborWeight = pawnNeighborWeight;
            this.advancedRankWeight = advancedRankWeight;
        }

        public Action chooseMove(Position node)
        {
            return(findBestAction(node, 6));
        }

        public Action findBestAction(Position node, int depth)
        {
            decimal alpha = decimal.MinValue;
            decimal beta = decimal.MaxValue;

            int activeColor = node.getPriorityColor();

            List<Action> genActions;
            Position p;
            Action bestAction = null;

            decimal s;

            genActions = node.generateActions();

            if (activeColor == 0)
            {
                for (int i = 0; i < genActions.Count; i++)
                {
                    p = genActions[i].transform(node);

                    s = AlphaBeta(p, depth - 1, alpha, beta);

                    if (s > alpha)
                    {
                        bestAction = genActions[i];
                        alpha = s;
                    }

                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < genActions.Count; i++)
                {
                    p = genActions[i].transform(node);

                    s = AlphaBeta(p, depth - 1, alpha, beta);

                    if (s < beta)
                    {
                        bestAction = genActions[i];
                        beta = s;
                    }

                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            return bestAction;
        }

        public decimal getPieceScore(int piece)
        {
            return pieceScore[piece];
        }

        public decimal getPawnNeighborWeight()
        {
            return pawnNeighborWeight;
        }

        public decimal getRankAdvancedWeight()
        {
            return advancedRankWeight;
        }

        private decimal AlphaBeta(Position node, int depth, decimal alpha, decimal beta)
        {
            int activeColor = node.getPriorityColor();

            List<Position> genStates;

            decimal s;

            if(depth == 0)
            {
                return (node.evaluate(this));
            }

            EndState e = node.getGameStatus();

            if (e == EndState.WhiteWins)
            {
                return (decimal.MaxValue - 1.0m); // to ensure that alpha beta pruning doesn't fail, we make the best possible score < initial alpha.
            }

            if (e == EndState.BlackWins)
            {
                return (decimal.MinValue + 1.0m);
            }

            if (e == EndState.Draw)
            {
                return 0.0m;
            }

            genStates = node.generatePositions();

            if (activeColor == 0)
            {
                for (int i = 0; i < genStates.Count; i++)
                {
                    s = AlphaBeta(genStates[i], depth - 1, alpha, beta);
                    alpha = Math.Max(alpha, s);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return alpha;
            }
            else
            {

                for (int i = 0; i < genStates.Count(); i++)
                {
                    s = AlphaBeta(genStates[i], depth - 1, alpha, beta);

                    beta = Math.Min(beta, s);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return beta;
            }
        }

    }
}
