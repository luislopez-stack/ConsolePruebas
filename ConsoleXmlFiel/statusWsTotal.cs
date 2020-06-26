using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleXmlFiel
{
    class statusWsTotal
    {
        public int Correctos { get; set; }
        public int Fallidos { get; set; }
        public int SinRespuesta { get; set; }
        public string errores { get; set; }
        public string NumLote { get; set; }
        public int TotalTitulos { get; set; }

        public string descripcion { get; set; }
    }
}
