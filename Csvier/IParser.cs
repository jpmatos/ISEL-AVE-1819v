namespace Csvier
{
    public interface IParser<T>
    {
        AbstractParser<T> CtorArg(string arg, int col);
        AbstractParser<T> PropArg(string arg, int col);
        AbstractParser<T> FieldArg(string arg, int col);
        T[] Parse();
    }
}