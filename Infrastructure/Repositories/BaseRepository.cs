using Npgsql;

namespace Infrastructure.Repositories;

public abstract class BaseRepository
{
    private readonly string connectionString;

    protected BaseRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    protected NpgsqlConnection GetConnection() => new(connectionString);
}