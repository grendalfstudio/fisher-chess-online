using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core
{
    public enum PlayerState
    {
        Regular,
        Check,
        Checkmate,
        Stalemate
    }
}
