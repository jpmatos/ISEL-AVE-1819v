using System;
using System.Collections;
using System.Collections.Generic;

namespace Csvier.Enumerables
{
    public class RowEnumerable : IEnumerable<string>
    {
        private readonly string src;
        private int remove = 0;
        private readonly List<string> removeWords = new List<string>();
        private bool removeEmpties = false;
        private bool removeOdds = false;
        private bool removeEvens = false;
        private bool isOdd = true;
        private int evensRemoved = 0;
        private int linesRemoved = 0;

        public RowEnumerable(string src)
        {
            this.src = src;
        }

        public IEnumerator<string> GetEnumerator()
        {
            string line = "";
            linesRemoved = remove;
            evensRemoved = 0;
            isOdd = true;
            foreach (char c in src)
            {
                if (c == '\n' || c == '\r')
                {
                    if (!VerifyConditions(line))
                    {
                        isOdd = !isOdd;
                        line = "";
                        continue;
                    }

                    isOdd = !isOdd;
                    yield return line;
                    line = "";
                }
                else
                {
                    line += c;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool VerifyConditions(string res)
        {

            foreach (string word in removeWords)
            {
                if (res.StartsWith(word))
                    return false;
            }

            if (removeEmpties == true)
            {
                if (string.IsNullOrEmpty(res))
                    return false;
            }
            
            if (linesRemoved > 0)
            {
                linesRemoved--;
                return false;
            }

            if (removeEvens == true)
            {
                if (!isOdd)
                {
                    evensRemoved++;
                    return false;
                }
            }

            if (removeOdds == true)
            {
                if (evensRemoved % 2 == 0 && isOdd)
                    return false;

                if (evensRemoved % 2 != 0 && !isOdd)
                    return false;
            }

            return true;
        }

        public void Remove(int count)
        {
            remove = count;
        }

        public void RemoveWith(string word)
        {
            removeWords.Add(word);
        }

        public void RemoveEmpties()
        {
            removeEmpties = true;
        }

        public void RemoveEvens()
        {
            removeEvens = true;
        }

        public void RemoveOdds()
        {
            removeOdds = true;
        }

        public string GetLine(int index)
        {
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