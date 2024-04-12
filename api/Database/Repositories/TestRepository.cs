using Dapper;
using Database.Interfaces;

namespace Database.Repositories;

public class TestRepository : ITestRepository
{
    private readonly DataContext context;
    
    public TestRepository(DataContext context)
    {
        this.context = context;
    }
    
    public void Execute()
    {
        var sql = $"INSERT INTO public.test (id, value) VALUES (@id, @value)";
        var parameters = new { id = 1, value = "test" };
        
        var connection = context.CreateConnection();
        connection.Open();
        connection.Execute(sql, parameters);
        connection.Close();
    }
}
