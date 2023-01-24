namespace htlgkr;

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
    public static void Main(string[] args)
        => new Program().Init(args);

    public Program() {
        commands = new() {
            [Command.connect] = Connect,
            [Command.config] = ReConfigure,
        };

        string winUser = Environment.UserName;
        filePath = $@"C:\Users\{winUser}\.htl";

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
            Connect();
            return;
        }

        bool parsed = Enum.TryParse(args[0], out Command c);

        if (!parsed) {
            Console.WriteLine("Invalid command");
            return;
        }
        else {
            commands[c].Invoke();
        }
    }
    #endregion

    #region ConsoleFunctions
    public void ReConfigure() {
        Console.WriteLine("Username:");
        username = Console.ReadLine();

        Console.WriteLine("Password:");
        password = GetObscuredPassword();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            Console.WriteLine("Invalid credentials\n");

            ReConfigure();
            return;
        }

        Console.WriteLine();

        File.WriteAllLines(filePath, new string[] { username, password });
    }

    private string GetObscuredPassword() {
        string pass = "";

        ConsoleKey key;
        do {
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Backspace & pass.Length > 0) {
                Console.Write("\b \b");
                pass = pass[0..^1];
                continue;
            }
            else if (!char.IsControl(Console.ReadKey(true).KeyChar)) {
                Console.Write("*");
                pass += key;
            }
        } while (key != ConsoleKey.Enter);

        return pass;
    }

    public void Connect() {
        using (HttpClient client = new()) {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            Dictionary<string, string> values = new() {
                { "auth_user", username },
                { "auth_pass", password },
                { "accept", "Anmelden" },
                { "rediurl", REDIURL }
            };

            FormUrlEncodedContent content = new(values);

            var reply = client.PostAsync(URL, content).Result;

            string page = reply.Content.ReadAsStringAsync().Result;

            if (page == "You are connected.") {
                Console.WriteLine("Already connected");
                return;
            }

            if (page.Contains("erfolgreich")) {
                Console.WriteLine("Connected");
                return;
            }
        }

        Console.WriteLine("fetzt ned");

        Console.Write("Passwort numoi eigebn? [Y|n]");

        string input = Console.ReadLine();

        if (input == "" || input.ToUpperInvariant() == "Y")
            ReConfigure();
    }
    #endregion
}