using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Question_Getter_Front_End
{
    class WordChecker
    {
        public WordChecker()
        {
        }

        public string classifyWords(string text)
        {
            int x = 0;
            int output;
            string strtest;
            string[] seperate;
            Word wrd;
            text = text.Trim();
            while(true)
            {
                if (x >= text.Length)
                    return text;
                if (x != 0)
                {
                    strtest = text.Substring(x, 1);
                    while (!strtest.Equals(" "))
                    {
                        x++;
                        if (x == text.Length)
                            break;
                        strtest = text.Substring(x, 1);
                    }
                    if (x == text.Length)
                        return text;
                }
                seperate = seperateNextWord(x, text);
                if (seperate == null)
                {
                    x++;
                    continue;
                }
                else if (seperate[1].Contains("href"))
                {
                    x++;
                    continue;
                }
                else if (seperate[1].Equals(""))
                {
                    x++;
                    continue;
                }
                else if (int.TryParse(seperate[1],out output))
                {
                    x += seperate.Length;
                    continue;
                }
                wrd = new Word(seperate[1]);
                DictionaryInterface.findType(wrd);
                if (wrd.type.Equals("unknown"))
                {
                    text = seperate[0] +" "+ wrd.word +"() "+ seperate[2];
                    x += wrd.word.Length;
                }
                else
                {
                    text = seperate[0] +" "+ wrd.word + "(" + wrd.type + ") " + seperate[2];
                    x += wrd.word.Length + wrd.type.Length + 2;
                }
            }
        }

        private string[] seperateNextWord(int start,string text)
        {
            string[] ret = new string[3];
            string wrd;
            text.Trim();
            if (start == 0)
            {
                ret[1] = text.Substring(0, (text.IndexOf(" ")));
                ret[0] = "";
                ret[2] = text.Substring(ret[1].Length);
                return ret;
            }
            int end = text.IndexOf(" ", start+1);
            if (end == -1)
                end = text.Length;
            ret[0] = text.Substring(0, start).Trim();
            wrd = text.Substring(start + 1, (end - start)-1);
            if (hasPunc(wrd))
            {
                ret[2] = (wrd.Substring(wrd.Length - 1, 1) + ret[2]).Trim();
                ret[1] = wrd.Substring(0, wrd.Length - 1).Trim();
            }
            else
                ret[1] = text.Substring(start+1, (end - start)-1).Trim().Trim();
            ret[2] = text.Substring(end).Trim();
            return ret;
        }

        private bool hasPunc(string wrd)
        {
            string[] punctuation = { "!", ",", "?", ".", "(", ")", "\\", "/", ":", ";"};
            bool ispunc = false;
            foreach (string s in punctuation)
            {
                if (wrd.Contains(s))
                    ispunc = true;
            }
            return ispunc;
        }
    }
}
