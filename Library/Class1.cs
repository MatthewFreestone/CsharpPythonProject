namespace Library;

public readonly record struct Class1Record(int Value);

public class Class1
{
    public Class1Record Record { get; }

    public Class1(int value)
    {
        Record = new Class1Record(value);
    }

    public int GetValue()
    {
        return Record.Value;
    }
}
