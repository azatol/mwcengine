using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public interface IEngine
    {
        Action findBestAction(Position node, int depth);
        decimal getPieceScore(int piece);
        decimal getPawnNeighborWeight();
        decimal getRankAdvancedWeight();
    }
}
