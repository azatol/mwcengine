using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public static class StUtility
    {
        public const string colLetters = "abcdefgh";

        public static string getAlgebraicCoords(int coor)
        {
            string alg;

            int row = (coor / 8) + 1;
            int col = coor % 8;

            alg = colLetters[col].ToString() + row;

            return alg;
        }

        public static int getOrdinalCoords(string alg)
        {
            if (alg == null || alg.Length != 2)
            {
                throw new ArgumentException("Invalid algebraic chess coordinates.");
            }

            char c = alg[0];

            int col = colLetters.IndexOf(c);

            int r = int.Parse(alg[1].ToString()) - 1;

            int coor = r * 8 + col;

            return coor;
        }
    }
}
