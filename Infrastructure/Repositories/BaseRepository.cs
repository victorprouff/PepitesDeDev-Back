using Npgsql;

namespace Infrastructure.Repositories;

public abstract class BaseRepository
{
    private readonly string _connectionString;

    protected BaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected NpgsqlConnection GetConnection() => new(_connectionString);
}