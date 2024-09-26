using Microsoft.Data.Sqlite;

public class DBFacade
{
    private readonly string sqlDBFilePath = "/tmp/chirp.db";

    public List<CheepViewModel> GetCheeps()
    {
        var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ...
            }
        }
    }
}