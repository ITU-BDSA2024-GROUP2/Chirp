namespace Chirp.CLI;

public class Chirp
{
    private String message;
    private String time;
    private String author;

    public Chirp(String message, String time, String author)
    {
        this.message = message;
        this.time = time;
        this.author = author;
    }

    public String GetMessage()
    {
        return message;
    }
    public String GetTime()
    {
        return time;
    }
    public String GetAuthor()
    {
        return author;
    }

}