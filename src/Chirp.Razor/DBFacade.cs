using System.Data;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private readonly string _sqlDbFilePath;

    public DBFacade()
    {
        _sqlDbFilePath  = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");
    }
    
    public List<CheepViewModel> ReadCheeps(int pageNumber, int pageSize) 
    {
        var queryString = @"SELECT u.username, m.text, m.pub_date
                            FROM message m
                            JOIN user u ON m.author_id = u.user_id
                            ORDER BY m.pub_date DESC
                            LIMIT @pageSize OFFSET @offset";
        
        int offset = (pageNumber - 1) * pageSize;
        var parameter = new Dictionary<string, object>
        {
            { "@pageSize", pageSize},
            { "@offset", offset}
        };

        try
        {
            return ExecuteQuery(queryString, parameter);
        }
        catch (SqliteException e)
        {
            Console.WriteLine("An error occured: ", e.Message);
        }
        return new List<CheepViewModel>();
    }

    public List<CheepViewModel> ReadCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        var queryString = @"SELECT u.username, m.text, m.pub_date
                                FROM message m
                                JOIN user u ON m.author_id = u.user_id
                                WHERE u.username = @author
                                ORDER BY m.pub_date DESC
                                LIMIT @pageSize OFFSET @offset";
        
        int offset = (pageNumber - 1) * pageSize;
        var parameter = new Dictionary<string, object>
        {
            { "@author", author },
            { "@pageSize", pageSize},
            { "@offset", offset}
        };

        try
        {
            return ExecuteQuery(queryString, parameter);
        }
        catch (SqliteException e)
        {
            Console.WriteLine("An error occured: ", e.Message);
        }
        return new List<CheepViewModel>();
    }

    public List<CheepViewModel> ExecuteQuery(string queryString, Dictionary<string, object>? parameters = null)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();

        
        using (var connection = new SqliteConnection($"Data Source={_sqlDbFilePath}"))
        {
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = queryString;
            
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
            
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
        
        var cheep = CheepViewModel.CreateCheep(author, message, pubDate);
        return cheep;
    }
}