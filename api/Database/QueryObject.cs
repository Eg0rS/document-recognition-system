using Database.Interfaces;

namespace Database;

public class QueryObject : IQueryObject
{
    public QueryObject(string sql, object parameters = null)
    {
        if (string.IsNullOrEmpty(sql))
        {
            throw new ArgumentNullException(nameof(sql));
        }

        Sql = sql;
        Params = parameters;
    }

    public string Sql { get; }
    public object Params { get; }
}
