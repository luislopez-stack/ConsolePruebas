using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleXmlFiel
{
    class StringWriterEncode : StringWriter
    {
        // Añadimos un atributo que almacenara la nueva codificacion
    private Encoding encoding;

        // Creamos un nuevo constructor que permita asociar una nueva codificacion
        public StringWriterEncode(Encoding e) : base()
        {
            this.encoding = e;
        }

        // Sobrecargamos el getter que devuelve la codificacion
        public override Encoding Encoding
        {
            get
            {
                return encoding;
            }
        }

        // Añadimos un nuevo getter que permita recuperar la codificacion por defecto
        public Encoding DefaultEncoding
        {
            get
            {
                return base.Encoding;
            }

        }
    }
}
