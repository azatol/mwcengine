using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class LowMemoryEngine : Agent, IEngine
    {
        public decimal pawnNeighborWeight;
        public decimal advancedRankWeight;
        public decimal[] pieceScore;

        private Position[,] storedPositions;
        private Action[] storedActions;

        public LowMemoryEngine(decimal[] pieceScore, decimal pawnNeighborWeight, decimal advancedRankWeight)
        {
            this.pieceScore = pieceScore;
            this.pawnNeighborWeight = pawnNeighborWeight;
            this.advancedRankWeight = advancedRankWeight;

            storedPositions = new Position[7, 200];
            storedActions = new Action[200];

            // 0-15 - Melee
            // 16-63 - Cannon
            // 64-199 - Movement

            for (int i = 0; i <= 15; i++)
            {
                storedActions[i] = new MeleeAction(0, 0);
            }

            for (int i = 16; i <= 63; i++)
            {
                storedActions[i] = new CannonAction(0, 0, 0);
            }

            for (int i = 64; i < 200; i++)
            {
                storedActions[i] = new MovementAction(0, 0, 0);
            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    storedPositions[i, j] = new Position();
                }
            }
        }

        public Action FindBestAction(Position node, int depth)
        {
            decimal alpha = decimal.MinValue;
            decimal beta = decimal.MaxValue;

            decimal s;

            int activeColor = node.getPriorityColor();

            Action bestAction = null;

            int meleeStart = 0;
            int cannonStart = 16;
            int movementStart = 64;

            int meleeEnd;
            int cannonEnd;
            int movementEnd;

            node.LMgenerateActions(storedActions, out movementEnd, out meleeEnd, out cannonEnd);

            MeleeAction me;
            MovementAction mo;
            CannonAction ca;
            Position active;

            Action bestAction;

            int bestIndex;

            for (int i = 0; i < movementEnd; i++)
            {
                if ((i >= meleeEnd && i < cannonStart) || (i >= cannonEnd && i < movementStart))
                {
                    continue;
                }

                active = storedPositions[depth, i];

                if (i < cannonStart)
                {
                    me = (MeleeAction)storedActions[i];
                    me.LMtransform(active, node);
                }
                else if (i < movementStart)
                {
                    ca = (CannonAction)storedActions[i];
                    ca.LMtransform(active, node);
                }
                else
                {
                    mo = (MovementAction)storedActions[i];
                    mo.LMtransform(active, node);
                }

                s = AlphaBeta(active, depth - 1, alpha, beta);

                if (activeColor == 0 && s > alpha)
                {
                    bestAction = storedActions[i];
                    alpha = s;
                }
                else if (activeColor == 8 && s < beta)
                {
                    bestAction = storedActions[i];
                    beta = s;
                }

                if (beta <= alpha)
                {
                    break;
                }
            }
        }

        private decimal AlphaBeta(Position node, int depth, decimal alpha, decimal beta)
        {

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

    }
}
