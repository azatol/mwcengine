using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class MeleeAction : Action
    {
        public const int pawnNeighborBonus = 1;

        // Pawn -   0 att.  , 1 def.
        // Knight - 3 att.  , 6 def.
        // Bishop - 4 att.  , 4 def.
        // Rook -   0 att.  , 1 def.
        // Queen -  5 att.  , 6 def.
        // King -   0 att.  , 4 def.

        // n/a  - pawn - knight - bishop - rook - queen - king
        public static int[] attackStrength = new int[] { 0, 0, 3, 4, 0, 5, 0 };
        public static int[] defenseStrength = new int[] { 0, 1, 6, 4, 1, 6, 4 };

        public int targetCoordinates;
        public int actingColor;

        public MeleeAction(int targetCoordinate, int actingColor)
        {
            this.targetCoordinates = targetCoordinate;
            this.actingColor = actingColor;
        }

        public int countFriendlyPawnNeighbors(Position p, int coor, int color)
        {
            List<int> ne = p.getNeighbors(coor);

            int ideal = color + 1;
            int val;
            int count = 0;

            for (int i = 0; i < ne.Count; i++)
            {
                val = p.getValue(ne[i]);

                if (val == ideal)
                {
                    count++;
                }
            }

            return count;
        }

        public bool isLegal(Position p)
        {
            if (p.getPriorityColor() != actingColor)
            {
                return false;
            }

            int opposingColor = Position.getOpposingColor(actingColor);

            if (!p.isOccupiedByColor(targetCoordinates, opposingColor))
            {
                return false;
            }

            int attackScore = 0;
            int defenseScore = 0;

            int targetPiece = p.getPiece(targetCoordinates);

            defenseScore = defenseStrength[targetPiece];

            // if target is a pawn
            if (targetPiece == 0x1)
            {
                defenseScore += pawnNeighborBonus * countFriendlyPawnNeighbors(p, targetCoordinates, opposingColor);
            }

            List<int> neighborC = p.getNeighbors(targetCoordinates);
                
            int pawn = actingColor + 1;
            int val;
            int piece;
            int pcolor;

            for (int i = 0; i < neighborC.Count; i++)
            {
                val = p.getValue(neighborC[i]);

                if (val == 0)
                {
                    continue;
                }

                piece = val & (~0x8);
                pcolor = val & 0x8;

                if (pcolor == actingColor)
                {
                    attackScore += attackStrength[piece];
                }

                if (piece == 0x1)
                {
                    attackScore += pawnNeighborBonus * countFriendlyPawnNeighbors(p, neighborC[i], actingColor);
                }
            }

            if (attackScore >= defenseScore)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Position transform(Position p)
        {
            Position n = p.removeAt(targetCoordinates);
            n.advanceParity();
            n.resetQuietTime();

            return n;
        }

        public void LMtransform(Position p, Position basePosition)
        {
            p.LMremoveAt(targetCoordinates, basePosition);
            p.advanceParity();
            p.resetQuietTime();
        }

        public string showNotation(Position p)
        {
            string targetAlg = StUtility.getAlgebraicCoords(targetCoordinates);

            List<string> attackAlg = new List<string>();

            string notation = "melee " + targetAlg;

            return notation;
        }

        public static List<Action> getLegalMeleeActions(Position p)
        {
            int color = p.getPriorityColor();

            List<int> enemiesInContact = p.getEnemiesInContact(color);

            List<Action> m = new List<Action>();
            MeleeAction temp;

            for (int i = 0; i < enemiesInContact.Count; i++)
            {
                temp = new MeleeAction(enemiesInContact[i], color);

                if (temp.isLegal(p))
                {
                    m.Add(temp);
                }
            }

            return m;
        }

        public static int LMgetMeleeActions(Position p, Action[] actions)
        {
            int index = 0;

            int color = p.getPriorityColor();
            int enemyColor = 8 - color;
            MeleeAction m;

            for (int i = 0; i < 64; i++)
            {
                if (p.isOccupiedByColor(i, enemyColor))
                {
                    m = (MeleeAction)actions[index];
                    m.actingColor = color;
                    m.targetCoordinates = i;

                    if (m.isLegal(p))
                    {
                        index++;
                    }
                }
            }

            return index;
        }
    }
}
