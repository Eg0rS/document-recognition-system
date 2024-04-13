namespace Database.Interfaces;

public interface IQueryObject
{
    string Sql { get; }

    object Params { get; }
}
