using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Question_Getter_Front_End
{
    class DictionaryInterface
    {
        public static string[] types = {"noun", "verb", "adj", "adv"};
        public DictionaryInterface()
        {
        }

        public static void findType(Word wrd)
        {
            WnLexicon.WordInfo wrdinf = WnLexicon.Lexicon.FindWordInfo(wrd.word, true);
            if (wrdinf.partOfSpeech.ToString().Equals("Unknown"))
                wrd.type = "";
            else
                wrd.type = wrdinf.partOfSpeech.ToString();
        }
    }
}
