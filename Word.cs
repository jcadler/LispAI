using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Question_Getter_Front_End
{
    class Word
    {
        public string word;
        public string type;

        public Word()
        {
            word = "";
            type = "";
        }

        public Word(string wrd, string tp)
        {
            word = wrd;
            type = tp;
        }

        public Word(string wrd)
        {
            word = wrd;
            type = null;
        }
    }
}
