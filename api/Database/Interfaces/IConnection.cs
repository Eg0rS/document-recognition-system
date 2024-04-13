namespace Database.Interfaces;

public interface IConnection
{
    Task<T?> FirstOrDefault<T>(IQueryObject queryObject);
    Task<List<T>> ListOrEmpty<T>(IQueryObject queryObject);
    Task Command(IQueryObject queryObject);
    Task<T> CommandWithResponse<T>(IQueryObject queryObject);
}
