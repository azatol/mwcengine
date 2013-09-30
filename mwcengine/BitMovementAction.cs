using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public class BitMovementAction
    {
        public int current;
        public int desired;
        public int pieceVal;

        public static ulong eastOne(ulong b) { return (b << 1) & notAFile; }
        public static ulong noEaOne(ulong b) { return (b << 9) & notAFile; }
        public static ulong soEaOne(ulong b) { return (b >> 7) & notAFile; }
        public static ulong westOne(ulong b) { return (b >> 1) & notHFile; }
        public static ulong soWeOne(ulong b) { return (b >> 9) & notHFile; }
        public static ulong noWeOne(ulong b) { return (b << 7) & notHFile; }

        public static ulong soutOne(ulong b) { return b >> 8; }
        public static ulong nortOne(ulong b) { return b << 8; }

        public static ulong kingAttacks(ulong kingSet)
        {
            ulong attacks = eastOne(kingSet) | westOne(kingSet);
            kingSet |= attacks;
            attacks |= nortOne(kingSet) | soutOne(kingSet);
            return attacks;
        }

        const ulong notAFile = 0xfefefefefefefefeUL; // ~0x0101010101010101
        const ulong notHFile = 0x7f7f7f7f7f7f7f7fUL; // ~0x8080808080808080

        public BitMovementAction(int current, int desired, int pieceVal)
        {
            this.current = current;
            this.desired = desired;

            this.pieceVal = pieceVal;
        }


    }


}
