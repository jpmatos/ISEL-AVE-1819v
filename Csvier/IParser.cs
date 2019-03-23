namespace Csvier
{
    public interface IParser
    {
        AbstractParser CtorArg(string arg, int col);
        AbstractParser PropArg(string arg, int col);
        AbstractParser FieldArg(string arg, int col);
        object[] Parse();
    }
}