namespace htlgkr;

public class Program {
    public static void Main(string[] args)
        => new HTLGKR().Init();
}

public class HTLGKR {
    public const string URL = "http://10.10.0.251:8002/index.php?zone=cp_htl";
    public const string REDIURL = "http://10.10.0.2/captiveportal/cp_logon_done.html";

    public readonly string dirPath;
    public readonly string filePath;

    private string username;
    private string password;

    public HTLGKR() {
        dirPath = @"C:\ProgramData\htlgkr";
        filePath = dirPath + @"\.htl";
    }

    public void Init() {
        try {
            Directory.CreateDirectory(dirPath);

            string[] fileText = File.ReadAllLines(filePath);

            username = fileText[0];
            password = fileText[1];
        }
        catch (Exception) {
            Console.WriteLine("Invalid credentials\n");

            ReConfigure();
        }

        Connect();
    }

    public void ReConfigure() {
        Console.Write("Username: ");
        username = Console.ReadLine();

        Console.Write("Password: ");
        password = GetObscuredPassword();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            Console.WriteLine("Invalid credentials\n");

            ReConfigure();
            return;
        }

        Console.WriteLine();

        File.WriteAllLines(filePath, new string[] { username, password });

        Console.WriteLine("Attempt connection? [Y|n]");

        ConsoleKeyInfo input = Console.ReadKey();

        Console.WriteLine();

        if (input.Key == ConsoleKey.Enter || char.ToUpperInvariant(input.KeyChar) == 'Y')
            Connect();
    }

    private static string GetObscuredPassword() {
        string pass = "";
        ConsoleKeyInfo key = Console.ReadKey(true);

        do {
            pass += key.KeyChar;
            Console.Write("*");

            key = Console.ReadKey(true);
        }
        while (key.Key != ConsoleKey.Enter);

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

            if (page == "You are connected." || page.Contains("erfolgreich")) {
                Console.WriteLine("Connected");
                return;
            }
        }

        Console.WriteLine("Connection attempt failed");

        Console.WriteLine("Re-enter credentials? [Y|n]");

        ConsoleKeyInfo input = Console.ReadKey();

        Console.WriteLine();

        if (input.Key == ConsoleKey.Enter || char.ToUpperInvariant(input.KeyChar) == 'Y')
            ReConfigure();
    }
}
