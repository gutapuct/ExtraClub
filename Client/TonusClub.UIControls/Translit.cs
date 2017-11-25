﻿using System.Collections.Generic;
using System.Text;

namespace TonusClub.UIControls
{
    public static class Translit
    {
        static Dictionary<char, string> words;

        public static string ToLatin(this string src)
        {
            if (src == null) return string.Empty;
            StringBuilder res = new StringBuilder();

            for (int i = 0; i < src.Length; i++)
            {
                if (words.ContainsKey(src[i]))
                {
                    res.Append(words[src[i]]);
                }
                else
                {
                    res.Append(src[i]);
                }
            }

            return res.ToString();
        }

        static Translit()
        {
            words = new Dictionary<char, string>();
            words.Add('а', "a");
            words.Add('б', "b");
            words.Add('в', "v");
            words.Add('г', "g");
            words.Add('д', "d");
            words.Add('е', "e");
            words.Add('ё', "yo");
            words.Add('ж', "zh");
            words.Add('з', "z");
            words.Add('и', "i");
            words.Add('й', "j");
            words.Add('к', "k");
            words.Add('л', "l");
            words.Add('м', "m");
            words.Add('н', "n");
            words.Add('о', "o");
            words.Add('п', "p");
            words.Add('р', "r");
            words.Add('с', "s");
            words.Add('т', "t");
            words.Add('у', "u");
            words.Add('ф', "f");
            words.Add('х', "h");
            words.Add('ц', "c");
            words.Add('ч', "ch");
            words.Add('ш', "sh");
            words.Add('щ', "sch");
            words.Add('ъ', "j");
            words.Add('ы', "i");
            words.Add('ь', "j");
            words.Add('э', "e");
            words.Add('ю', "yu");
            words.Add('я', "ya");
            words.Add('А', "A");
            words.Add('Б', "B");
            words.Add('В', "V");
            words.Add('Г', "G");
            words.Add('Д', "D");
            words.Add('Е', "E");
            words.Add('Ё', "Yo");
            words.Add('Ж', "Zh");
            words.Add('З', "Z");
            words.Add('И', "I");
            words.Add('Й', "J");
            words.Add('К', "K");
            words.Add('Л', "L");
            words.Add('М', "M");
            words.Add('Н', "N");
            words.Add('О', "O");
            words.Add('П', "P");
            words.Add('Р', "R");
            words.Add('С', "S");
            words.Add('Т', "T");
            words.Add('У', "U");
            words.Add('Ф', "F");
            words.Add('Х', "H");
            words.Add('Ц', "C");
            words.Add('Ч', "Ch");
            words.Add('Ш', "Sh");
            words.Add('Щ', "Sch");
            words.Add('Ъ', "J");
            words.Add('Ы', "I");
            words.Add('Ь', "J");
            words.Add('Э', "E");
            words.Add('Ю', "Yu");
            words.Add('Я', "Ya");
        }
    }
}