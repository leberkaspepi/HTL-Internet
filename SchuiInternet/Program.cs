namespace SchuiInternet;

public class Program
{
    private const string URL = "http://10.10.0.251:8002/index.php?zone=cp_htl";

    private readonly string username;
    private readonly string password;

    public static void Main(string[] args) => new Program().Init();

    public Program()
    {
        string winUser = Environment.UserName;

        try
        {
            string[] fileText = File.ReadAllLines($@"C:\Users\{winUser}\.conconfig.con");

            username = fileText[0];
            password = fileText[1];
        }
        catch
        {
            Console.WriteLine("Error");
            Environment.Exit(1);
        }
    }

    private void Init()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (input == "connect")
            {
                Connect();
            }
        }
    }

    private bool Connect()
    {
        HttpClient client = new();

        Dictionary<string, string> values = new()
        {
            {"auth_user", username },
            {"auth_pass", password},
        };

        var content = new FormUrlEncodedContent(values);

        var response = client.PostAsync(URL, content).Result;

        var responseString = response.Content.ReadAsStringAsync();

        Console.WriteLine(responseString.Result);

        return true;
    }
}