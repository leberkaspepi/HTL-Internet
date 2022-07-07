using HtmlAgilityPack;

namespace SchuiInternet;

public class Program {
    #region Vars
    private const string URL = "http://10.10.0.251:8002/index.php?zone=cp_htl";
    private const string REDIURL = "http://10.10.0.2/captiveportal/cp_logon_done.html";

    private string username;
    private string password;

    private readonly string filePath;

    private readonly Dictionary<Command, Action> commands;
    #endregion


    #region Ctor
    public static void Main(string[] args) => new Program().Init(args);
    public Program() {
        commands = new() {
            [Command.display] = DisplayCommands,
            [Command.connect] = Connect,
            [Command.reconfigure] = ReConfigure,
        };

        string winUser = Environment.UserName;
        filePath = $@"C:\Users\{winUser}\.conconfig";

        try {
            string[] fileText = File.ReadAllLines(filePath);

            username = fileText[0];
            password = fileText[1];
        }
        catch (Exception) {
            Console.WriteLine("Irgendwos passt an deim file ned");

            ReConfigure();
        }
    }

    private void Init(string[] args) {
        if (args.Length == 0) {
            DisplayCommands();
            Environment.Exit(1);
        }

        bool parsed = Enum.TryParse(args[0], out Command c);

        if (!parsed) {
            Console.WriteLine("Invalid command");
            DisplayCommands();
            Environment.Exit(1);
        }
        else {
            commands[c].Invoke();
        }
    }
    #endregion

    #region ConsoleFunctions
    public void DisplayCommands() {
        Console.WriteLine("connect: try to connect with saved credentials");
        Console.WriteLine("reconfig: re-enter credentials");

        Console.WriteLine();
    }

    public void ReConfigure() {
        if (!File.Exists(filePath))
            File.Create(filePath).Close();

        Console.WriteLine("Username:");
        username = Console.ReadLine();

        Console.WriteLine("Password:");
        password = Console.ReadLine();

        if (username == null || username == "" || password == null || password == "") {
            Console.WriteLine("Invalid credentials\n");

            ReConfigure();
            return;
        }

        File.WriteAllLines(filePath, new string[] { username, password });
    }

    public void Connect() {
        HttpClient client = new();
        //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

        Dictionary<string, string> values = new()
        {
             { "auth_user", username },
             { "auth_pass", password },
             { "accept", "Anmelden" },
             { "rediurl", REDIURL }
        };

        FormUrlEncodedContent content = new(values);

        Task<HttpResponseMessage> response = client.PostAsync(URL, content);

        string responseString = response.Result.Content.ReadAsStringAsync().Result;

        Console.WriteLine(responseString);

        HtmlDocument doc = new();
        doc.LoadHtml(responseString);

        //IEnumerable<HtmlNode> p = doc.DocumentNode.Descendants("p");

        //if (p.Any())
        //    Console.WriteLine(p.First().InnerHtml);
        //else
        //    Console.WriteLine(responseString);
    }
    #endregion
}

public enum Command {
    display,
    connect,
    reconfigure,
}