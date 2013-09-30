using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public interface Action
    {
        bool isLegal(Position p);
        Position transform(Position p);
        void LMtransform(Position p, Position basePosition);
        string showNotation(Position p);
    }
}
