namespace SchuiInternet;

public class Program
{
    private const string URL = "http://10.10.0.251:8002/index.php?zone=cp_htl";

    private string username;
    private string password;

    private readonly string filePath;

    public static void Main(string[] args) => new Program().Init();

    public Program()
    {
        string winUser = Environment.UserName;
        filePath = $@"C:\Users\{winUser}\.conconfig";

        try
        {
            string[] fileText = File.ReadAllLines(filePath);

            username = fileText[0];
            password = fileText[1];
        }
        catch (Exception)
        {
            Console.WriteLine("Irgendwos passt an deim file ned");

            Console.WriteLine("Username:");
            username = Console.ReadLine();

            Console.WriteLine("Password:");
            password = Console.ReadLine();


            ReConfigure();
        }
        finally
        {
            Console.WriteLine("Irgend wos hot _wirklich_ ned highaud, start neich");
            Environment.Exit(1);
        }
    }

    private void Init()
    {
        Console.WriteLine("To see list of availabe commands, type 'IAmAUselessMemberOfSocietyAndRequireALobotomyMarker'");

        while (true)
        {
            Console.WriteLine();
            string input = Console.ReadLine();

            switch (input)
            {
                case "connect":
                    Connect();
                    break;
                case "reconfig":
                    ReConfigure();
                    break;
                case "IAmAUselessMemberOfSocietyAndRequireALobotomyMarker":
                case "help":
                    DisplayCommands();
                    break;
            }
        }
    }

    private void DisplayCommands()
    {
        Console.WriteLine("connect: try to connect with saved credentials");
        Console.WriteLine("reconfig: re-enter credentials\n");
    }
    
    private void ReConfigure()
    {
        File.Delete(filePath);
        File.Create(filePath).Close();

        Console.WriteLine("Username:");
        username = Console.ReadLine();

        Console.WriteLine("Password:");
        password = Console.ReadLine();

        if (username == null || username == "" || password == null || password == "")
        {
            Console.WriteLine("konnst du wortwörtlich kan string eigebn?");
            Console.WriteLine("kumm geh scheißn und moch da dei config file söwa (oda start afoch donwengung neich)");

            Environment.Exit(1);
        }

        File.WriteAllLines(filePath, new string[] { username, password });

        Console.WriteLine("perfekt, fetzt");
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