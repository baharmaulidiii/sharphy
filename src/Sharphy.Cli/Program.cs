using Sharphy.Core;
using Sharphy.Server;

namespace Sharphy.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
        {
            PrintUsage();
            return 0;
        }

        if (args[0] is "-v" or "--version" or "version")
        {
            var version = typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "1.1.0";
            Console.WriteLine($"Sharphy CLI v{version} [{RuntimeEnvironment.FrameworkVersion}]");
            return 0;
        }

        if (args[0].Equals("mcp", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length > 1 && args[1].Equals("start", StringComparison.OrdinalIgnoreCase))
            {
                // Run Model Context Protocol (MCP) server over pure Stdio transport
                var server = new SharphyMcpServer();
                using var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                await server.RunAsync(cts.Token);
                return 0;
            }

            Console.Error.WriteLine("Unknown MCP subcommand. Usage: sharphy mcp start");
            return 1;
        }

        Console.Error.WriteLine($"Unrecognized command '{args[0]}'. Run 'sharphy --help' for available commands.");
        return 1;
    }

    private static void PrintUsage()
    {
        Console.WriteLine(@"Sharphy: CLR Intelligence & Runtime Forensic Bridge
Usage:
  sharphy mcp start          Run Model Context Protocol (MCP) server host over Stdio
  sharphy --version          Show Sharphy CLI version information
  sharphy --help             Display this help documentation
");
    }
}
