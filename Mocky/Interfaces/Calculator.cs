using System;
using System.Collections.Generic;

namespace Mocky
{
    public class Calculator : ICalculator
    {
        private MockBase mockBase;
        
        
        public Calculator(MockMethod[] arr)
        {
            mockBase = new MockBase(arr);
        }

        public int Add(int a, int b)
        {
            return (int)mockBase.Invoke("Add", a, b);
        }

        public int Sub(int a, int b)
        {
            throw new NotImplementedException();
        }

        public int Mul(int a, int b)
        {
            return (int)mockBase.Invoke("Mul", a, b);
        }

        public int Div(int a, int b)
        {
            throw new NotImplementedException();
        }
    }
}