using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    // White: 0x0
    // Black: 0x8
    // Pawn: 0x1
    // Knight: 0x2
    // Bishop: 0x3
    // Rook: 0x4
    // Queen: 0x5
    // King: 0x6

    public class BitBoard
    {
        internal ulong white;
        internal ulong black;
        internal ulong pawns;
        internal ulong knights;
        internal ulong bishops;
        internal ulong rooks;
        internal ulong queens;
        internal ulong kings;

        internal int parity;
        internal int quietTime;

        public static ulong[] powers = new ulong[] {0x1UL, 0x2UL, 0x4UL, 0x8UL, 0x10UL, 0x20UL, 0x40UL, 0x80UL, 0x100UL, 0x200UL, 0x400UL, 0x800UL, 0x1000UL, 0x2000UL, 0x4000UL, 0x8000UL, // 15
                                                    0x10000UL, 0x20000UL, 0x40000UL, 0x80000UL, 0x100000UL, 0x200000UL, 0x400000UL, 0x800000UL, 0x1000000UL, 0x2000000UL, 0x4000000UL,      // 26
                                                    0x8000000UL, 0x10000000UL, 0x20000000UL, 0x40000000UL, 0x80000000UL, 0x100000000UL, 0x200000000UL, 0x400000000UL, 0x800000000UL,        // 35
                                                    0x1000000000UL, 0x2000000000UL, 0x4000000000UL, 0x8000000000UL, 0x10000000000UL, 0x20000000000UL, 0x40000000000UL, 0x80000000000UL,     // 43
                                                    0x100000000000UL, 0x200000000000UL, 0x400000000000UL, 0x800000000000UL, 0x1000000000000UL, 0x2000000000000UL, 0x4000000000000UL,        // 50
                                                    0x8000000000000UL, 0x10000000000000UL, 0x20000000000000UL, 0x40000000000000UL, 0x80000000000000UL, 0x100000000000000UL,                 // 56
                                                    0x200000000000000UL, 0x400000000000000UL, 0x800000000000000UL, 0x1000000000000000UL, 0x2000000000000000UL, 0x4000000000000000UL,        // 62
                                                    0x8000000000000000UL };

        public static ulong[] neighbors = new ulong[] 
            {   0x302,
                0x705,
                0xE0A,
                0x1C14,
                0x3828,
                0x7050,
                0xE0A0,
                0xC040,
                0x30203,
                0x70507,
                0xE0A0E,
                0x1C141C,
                0x382838,
                0x705070,
                0xE0A0E0,
                0xC040C0,
                0x3020300,
                0x7050700,
                0xE0A0E00,
                0x1C141C00,
                0x38283800,
                0x70507000,
                0xE0A0E000,
                0xC040C000,
                0x302030000,
                0x705070000,
                0xE0A0E0000,
                0x1C141C0000,
                0x3828380000,
                0x7050700000,
                0xE0A0E00000,
                0xC040C00000,
                0x30203000000,
                0x70507000000,
                0xE0A0E000000,
                0x1C141C000000,
                0x382838000000,
                0x705070000000,
                0xE0A0E0000000,
                0xC040C0000000,
                0x3020300000000,
                0x7050700000000,
                0xE0A0E00000000,
                0x1C141C00000000,
                0x38283800000000,
                0x70507000000000,
                0xE0A0E000000000,
                0xC040C000000000,
                0x302030000000000,
                0x705070000000000,
                0xE0A0E0000000000,
                0x1C141C0000000000,
                0x3828380000000000,
                0x7050700000000000,
                0xE0A0E00000000000,
                0xC040C00000000000,
                0x203000000000000,
                0x507000000000000,
                0xA0E000000000000,
                0x141C000000000000,
                0x2838000000000000,
                0x5070000000000000,
                0xA0E0000000000000,
                0x40C0000000000000};

        public BitBoard()
        {
            white =     0x000000000000ffffUL;
            black =     0xffff000000000000UL;
            pawns =     0x00ff00000000ff00UL;
            knights =   0x4200000000000042UL;
            bishops =   0x2400000000000024UL;
            rooks =     0x8100000000000081UL;
            queens =    0x0800000000000008UL;
            kings =     0x1000000000000010UL;

            parity = 0;
            quietTime = 0;
        }

        public BitBoard(ulong white, ulong black, ulong pawns, ulong knights, ulong bishops, ulong rooks, ulong queens, ulong kings, int parity, int quietTime)
        {
            this.white = white;
            this.black = black;
            this.pawns = pawns;
            this.knights = knights;
            this.bishops = bishops;
            this.rooks = rooks;
            this.queens = queens;
            this.kings = kings;
            this.parity = parity;
            this.quietTime = quietTime;
        }

        public List<Action> generateActions()
        {
            return null;
        }

        public int getPriorityColor()
        {
            if (parity >= 3)
            {
                return 0x8;
            }
            else
            {
                return 0x0;
            }
        }

        public int getPieceValAtLocation(int coor)
        {
            int pieceVal = 0;

            pieceVal += 8 * (int)((black & powers[coor]) >> coor);
            pieceVal += 1 * (int)((pawns & powers[coor]) >> coor);
            pieceVal += 2 * (int)((knights & powers[coor]) >> coor);
            pieceVal += 3 * (int)((bishops & powers[coor]) >> coor);
            pieceVal += 4 * (int)((rooks & powers[coor]) >> coor);
            pieceVal += 5 * (int)((queens & powers[coor]) >> coor);
            pieceVal += 6 * (int)((kings & powers[coor]) >> coor);

            return pieceVal;
        }

        public static int count(ulong board)
        {
            int count = 0;
            while (board != 0UL) 
            {
                count++;
                board &= board - 1; // reset LS1B
            }
            
            return count;
        }

        public BitBoard removeAt(int coor, int piece, int color)
        {
            BitBoard b = new BitBoard(white, black, pawns, knights, bishops, rooks, queens, kings, parity + 1, 0);

            if (color == 0)
            {
                b.white &= (~powers[coor]);
            }
            else
            {
                b.black &= (~powers[coor]);
            }

            switch (piece)
            {
                case 1:
                    b.pawns &= (~powers[coor]);
                    break;
                case 2:
                    b.knights &= (~powers[coor]);
                    break;
                case 3:
                    b.bishops &= (~powers[coor]);
                    break;
                case 4:
                    b.rooks &= (~powers[coor]);
                    break;
                case 5:
                    b.queens &= (~powers[coor]);
                    break;
                case 6:
                    b.kings &= (~powers[coor]);
                    break;
            }

            return b;
        }

        public BitBoard moveTo(int currentCoor, int desiredCoor, int piece, int color)
        {
            BitBoard b = new BitBoard(white, black, pawns, knights, bishops, rooks, queens, kings, parity + 1, quietTime + 1);

            ulong mask = powers[currentCoor] | powers[desiredCoor];

            if (color == 0)
            {
                b.white ^= mask;
            }
            else
            {
                b.black ^= mask;
            }

            switch (piece)
            {
                case 1:
                    b.pawns ^= mask;
                    break;
                case 2:
                    b.knights ^= mask;
                    break;
                case 3:
                    b.bishops ^= mask;
                    break;
                case 4:
                    b.rooks ^= mask;
                    break;
                case 5:
                    b.queens ^= mask;
                    break;
                case 6:
                    b.kings ^= mask;
                    break;
            }

            return b;
        }
    }
}
