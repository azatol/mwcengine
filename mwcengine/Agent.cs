using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    public interface Agent
    {
        Action chooseMove(Position node);
    }
}
