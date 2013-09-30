using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class BitMeleeAction : BitAction
    {
        public int targetLocation;  // (0-63)

        public int pieceVal;        // (0, 8 colors. 1-6 pieces)

        public static int[] attackStrength = new int[] { 0, 0, 3, 4, 0, 5, 0 };
        public static int[] defenseStrength = new int[] { 0, 1, 6, 4, 1, 6, 4 };

        public const int pawnAS = 0;
        public const int pawnDS = 1;
        public const int knightAS = 3;
        public const int knightDS = 6;
        public const int bishopAS = 4;
        public const int bishopDS = 4;
        public const int rookAS = 0;
        public const int rookDS = 1;
        public const int queenAS = 5;
        public const int queenDS = 6;
        public const int kingAS = 0;
        public const int kingDS = 4;

        public BitMeleeAction(int targetLocation, int pieceVal)
        {
            this.targetLocation = targetLocation;
            this.pieceVal = pieceVal;
        }

        public bool isLegal(BitBoard b)
        {
            ulong colorBoard;
            ulong targetBoard;
            ulong occupancy;

            if ((pieceVal & 8) == 0)
            {
                colorBoard = b.white;
                targetBoard = b.black;
            }
            else
            {
                colorBoard = b.black;
                targetBoard = b.white;
            }

            occupancy = targetBoard & BitBoard.powers[targetLocation];

            if (occupancy == 0UL)      // enemy piece not present at square.
            {
                return false;
            }

            ulong neighbors = BitBoard.neighbors[targetLocation] & colorBoard;
            ulong wrk;
            int index = 0;

            int begIndex = targetLocation - 9;

            if(begIndex < 0)
            {
                begIndex = 0;
            }

            wrk = neighbors >> begIndex;
            index = begIndex;

            ulong pawnNeighbors;

            ulong pa = (neighbors & b.pawns) >> begIndex;
            ulong kn = (neighbors & b.knights) >> begIndex;
            ulong bi = (neighbors & b.bishops) >> begIndex;
            ulong rk = (neighbors & b.rooks) >> begIndex;
            ulong q = (neighbors & b.queens) >> begIndex;
            ulong k = (neighbors & b.kings) >> begIndex;

            int attackStrength = 0;
            int defenseStrength = 0;

            while (wrk != 0UL)
            {
                if ((pa & 1) == 1UL)
                {
                    pawnNeighbors = BitBoard.neighbors[index] & colorBoard & b.pawns;

                    attackStrength += pawnAS;
                    attackStrength += BitBoard.count(pawnNeighbors);
                }
                else
                {
                    attackStrength += knightAS * (int)(kn & 1);
                    attackStrength += bishopAS * (int)(bi & 1);
                    attackStrength += rookAS * (int)(rk & 1);
                    attackStrength += queenAS * (int)(q & 1);
                    attackStrength += kingAS * (int)(k & 1);
                }

                index++;
                wrk >>= 1;
                pa >>= 1;
                kn >>= 1;
                bi >>= 1;
                rk >>= 1;
                q >>= 1;
                k >>= 1;
            }

            pa = b.pawns & targetBoard >> targetLocation;
            kn = b.knights & targetBoard >> targetLocation;
            bi = b.bishops & targetBoard >> targetLocation;
            rk = b.rooks & targetBoard >> targetLocation;
            q = b.queens & targetBoard >> targetLocation;
            k = b.kings & targetBoard >> targetLocation;

            if ((pa & 1) != 0)
            {
                pawnNeighbors = BitBoard.neighbors[targetLocation] & targetBoard & b.pawns;

                defenseStrength += pawnDS;
                defenseStrength += BitBoard.count(pawnNeighbors);
            }
            else
            {
                defenseStrength += knightDS * (int)(kn & 1);
                defenseStrength += bishopDS * (int)(bi & 1);
                defenseStrength += rookDS * (int)(rk & 1);
                defenseStrength += queenDS * (int)(q & 1);
                defenseStrength += kingDS * (int)(k & 1);
            }

            return (attackStrength >= defenseStrength);
        }

        public BitBoard transform(BitBoard b)
        {
            int piece = pieceVal & (~8);
            int color = pieceVal & 8;

            BitBoard m = b.removeAt(targetLocation, piece, color);

            return m;
        }

        public override string ToString()
        {
            string res = "melee " + StUtility.getAlgebraicCoords(targetLocation);

            return res;
        }
    }
}
