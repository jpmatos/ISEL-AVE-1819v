
using System;

namespace Mocky
{
    public interface IHttpRequest : IDisposable
    {
        string GetBody(string url);
    }
}
