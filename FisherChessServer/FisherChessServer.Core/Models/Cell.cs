using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core.Models
{
    public record Cell(int Row, int Column)
    {
        public bool IsValid() => (Row >= 0 && Row <= 7 && Column >= 0 && Column <= 7);
    }
}
