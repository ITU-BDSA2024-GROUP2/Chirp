using System.Data;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private readonly string _sqlDBFilePath;

    public DBFacade(string sqlDBFilePath = "/tmp/chirp.db")
    {
        _sqlDBFilePath = sqlDBFilePath;
    }

    public List<CheepViewModel> ReadCheeps(int pageNumber, int pageSize) // Skal denne laves async??
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        
        var queryString = @"SELECT u.username, m.text, m.pub_date
                            FROM message m
                            JOIN user u ON m.author_id = u.user_id
                            ORDER BY m.pub_date DESC
                            LIMIT @PageSize OFFSET @Offset";
        
        using (var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = queryString;
            
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;

                var cheep = ReadSingleRow(dataRecord);
                cheeps.Add(cheep);
            }
            reader.Close();
        }
        return cheeps;
    }
    
    private static CheepViewModel ReadSingleRow(IDataRecord dataRecord)
    {
        var author = dataRecord["username"].ToString();
        var message = dataRecord["text"].ToString();
        var pubDate = Convert.ToInt64(dataRecord["pub_date"]);
        
        var Cheep = CheepViewModel.CreateCheep(author, message, pubDate);
        return Cheep;
    }
}