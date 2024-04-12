using System.Data;
using Npgsql;

namespace Database;

public class DataContext
{
    public DataContext()
    {
        
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = $"Host=postgres; Database=postgres_db; Username=postgres; Password=postgres;";
        return new NpgsqlConnection(connectionString);
    }
}
