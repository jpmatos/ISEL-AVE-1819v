using System;

namespace Mocky.Test
{
    public class Calculator : ICalculator
    {
        public Calculator(MockMethod[] arr)
        {
            
        }

        public int Add(int a, int b)
        {
            throw new NotImplementedException();
        }

        public int Sub(int a, int b)
        {
            throw new NotImplementedException();
        }

        public int Mul(int a, int b)
        {
            throw new NotImplementedException();
        }

        public int Div(int a, int b)
        {
            throw new NotImplementedException();
        }
    }
}