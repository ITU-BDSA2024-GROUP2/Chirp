namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        throw new NotImplementedException();
    }

    public void Store(T record)
    {
        throw new NotImplementedException();
    }
}
