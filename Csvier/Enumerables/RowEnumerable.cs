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
        
        public RowEnumerable(string src)
        {
            this.src = src;
        }

        public IEnumerator<string> GetEnumerator()
        {
            string line = "";
            Console.Write(line);
            foreach (char c in src)
            {
                if (c == '\n' || c == '\r')
                {
                    if (!VerifyConditions(line))
                    {
                        isOdd = !isOdd;
                        continue;
                    }
                    yield return line;
                    line = "";
                }
                else
                {
                    line += c;
                }
            }
            //What to do if empty?
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool VerifyConditions(string res)
        {
            Console.Write(res);
            
            Console.Write("Remove");
            if (remove > 0)
            {
                remove--;
                return false;
            }
            
            Console.Write("RemoveWords");
            foreach (string word in removeWords)
            {
                if (res.StartsWith(word))
                    return false;
                
            }

            Console.Write("RemoveEmpties");
            if (removeEmpties == true)
            {
                if (string.IsNullOrEmpty(res))
                    return false;
            }

            Console.Write("RemoveEvens");
            if (removeEvens == true)
            {
                if (!isOdd) return false;
            }

            Console.Write("RemoveOdds");
            if (removeOdds == true)
            {
                if (isOdd) return false;
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
    }
}