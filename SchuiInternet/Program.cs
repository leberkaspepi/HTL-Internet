using HtmlAgilityPack;

namespace SchuiInternet;

public class Program {
    private const string URL = "http://10.10.0.251:8002/index.php?zone=cp_htl";
    private const string REDIURL = "http://10.10.0.2/captiveportal/cp_logon_done.html";

    private string username;
    private string password;

    private readonly string filePath;

    public static void Main(string[] args) => new Program().Init();

    public Program() {
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

    private void Init() {
        DisplayCommands();

        while (true) {
            Console.WriteLine();
            string input = Console.ReadLine();

            switch (input) {
                case "connect":
                ConnectPost();
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

    private void DisplayCommands() {
        Console.WriteLine("connect: try to connect with saved credentials");
        Console.WriteLine("reconfig: re-enter credentials\n");
    }

    private void ReConfigure() {
        File.Delete(filePath);
        File.Create(filePath).Close();

        Console.WriteLine("Username:");
        username = Console.ReadLine();

        Console.WriteLine("Password:");
        password = Console.ReadLine();

        if (username == null || username == "" || password == null || password == "") {
            Console.WriteLine("konnst du wortwörtlich kan string eigebn?");
            Console.WriteLine("kumm geh scheißn und moch da dei config file söwa (oda start afoch donwengung neich)");

            Environment.Exit(1);
        }

        File.WriteAllLines(filePath, new string[] { username, password });

        Console.Clear();
        
        Console.WriteLine("perfekt, fetzt");
        DisplayCommands();
    }

    private void ConnectPost() {
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

        var content = new FormUrlEncodedContent(values);

        var response = client.PostAsync(URL, content);

        var responseString = response.Result.Content.ReadAsStringAsync().Result;

        HtmlDocument doc = new();
        doc.LoadHtml(responseString);

        var p = doc.DocumentNode.Descendants("p");

        if (p.Any())
            Console.WriteLine(p.First().InnerHtml);
        else
            Console.WriteLine(responseString);
    }
}