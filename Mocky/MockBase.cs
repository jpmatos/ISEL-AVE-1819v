using System;
using System.Linq;

namespace Mocky
{
    public class MockBase
    {
        private MockMethod[] arr;

        public MockBase(MockMethod[] arr)
        {
            this.arr = arr;
        }

        public object Invoke(string method, params object[] args)
        {
            MockMethod foundMeth = arr.FirstOrDefault(meth => meth.Method.Name == method);
            if(foundMeth is null) throw new NotImplementedException();
            return foundMeth.Call(args);
        }
    }
}