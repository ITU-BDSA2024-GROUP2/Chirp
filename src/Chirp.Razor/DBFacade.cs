/*using System.Data;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;

public class DBFacade
{
    private readonly string _sqlDbFilePath;

    public DBFacade()
    {
        _sqlDbFilePath  = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        if (_sqlDbFilePath == null)
        {
            _sqlDbFilePath = Path.Combine(Path.GetTempPath(), "chirp.db");
            InitDB();
        }
    }

    private void InitDB()
    {
        try
        {
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            // Read the schema file
            using var schemaReader = embeddedProvider.GetFileInfo("data/schema.sql").CreateReadStream();
            using var schemaStreamReader = new StreamReader(schemaReader);
            var schema = schemaStreamReader.ReadToEnd();
        
            ExecuteNonQuery(schema);

            // Read the dump file
            using var dumpReader = embeddedProvider.GetFileInfo("data/dump.sql").CreateReadStream();
            using var dumpStreamReader = new StreamReader(dumpReader);
            var dump = dumpStreamReader.ReadToEnd();
            
            ExecuteNonQuery(dump);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while initializing the database: {e.Message}");
        }
    }
    
    public List<CheepDTO> ReadCheeps(int pageNumber, int pageSize) 
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
            Console.WriteLine($"An error occured: {e.Message}");
        }
        return new List<CheepDTO>();
    }

    public List<CheepDTO> ReadCheepsFromAuthor(string author, int pageNumber, int pageSize)
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
            Console.WriteLine($"An error occured: {e.Message}");
        }
        return new List<CheepDTO>();
    }
    
    public void ExecuteNonQuery(string nonQueryString)
    {
        using (var connection = new SqliteConnection($"Data Source={_sqlDbFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = nonQueryString;

            command.ExecuteNonQuery();
        }
    }

    public List<CheepDTO> ExecuteQuery(string queryString, Dictionary<string, object>? parameters = null)
    {
        List<CheepDTO> cheeps = new List<CheepDTO>();

        
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
}*/