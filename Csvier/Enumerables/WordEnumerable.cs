using System;
using System.Collections;
using System.Collections.Generic;

namespace Csvier.Enumerables
{
    public class WordEnumerable : IEnumerable<string>
    {
        private char separator;
        private string line;

        public WordEnumerable(string line, char separator)
        {
            this.separator = separator;
            this.line = line;
        }

        public IEnumerator<string> GetEnumerator()
        {
            string word = "";
            int i = 0;
            foreach (char c in line)
            {
                if (c == separator || c == '\n' || c == '\r' || line.Length == i+1)
                {
                    if (line.Length == i+1)
                        word += c;
                    yield return word;
                    word = "";
                }
                else
                {
                    word += c;
                }
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string GetWord(int index)
        {
            //index--;
            foreach (string str in this)
            {
                if (index > 0)
                    index--;
                else
                    return str;
            }
            throw new IndexOutOfRangeException();
        }
    }
}