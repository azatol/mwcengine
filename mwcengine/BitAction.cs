using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWCChessEngine
{
    interface BitAction
    {
        bool isLegal(BitBoard b);
        BitBoard transform(BitBoard b);
    }
}
