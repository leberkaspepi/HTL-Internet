namespace htlgkr;

public enum Command {
    connect,
    config,
}

internal static class Extensions {
    public static string Description(this Command command)
        => command switch {
            Command.connect => "Connect to the internet using saved credentials",
            Command.config => "Change credentials",
            _ => "gack",
        };

    public static string Name(this Command command)
        => char.ToUpper(command.ToString()[0]) + command.ToString().Substring(1);
}