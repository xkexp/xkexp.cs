namespace DelegateDi;

internal class Decoder
{
    public delegate bool HeadChecker(object message);

    private readonly HeadChecker _checker = _ => true;

    public Decoder()
    {
        Console.WriteLine("Decoder non args Constructor.");
    }

    public Decoder(HeadChecker checker)
    {
        Console.WriteLine("Decoder checker args Constructor.");
        _checker = checker;
    }

    public void Decode(object message)
    {
        if (_checker.Invoke(message))
        {
            Console.WriteLine("Header Check success");
        }
        else
        {
            Console.WriteLine("Header Check failed");
        }
    }
}
