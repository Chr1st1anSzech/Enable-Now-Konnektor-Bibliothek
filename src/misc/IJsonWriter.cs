using System;
using System.Collections.Generic;
using System.Text;

namespace Enable_Now_Konnektor_Bibliothek.src.misc
{
    public interface IJsonWriter
    {
        public string Write(object obj, string path = null);
    }
}
