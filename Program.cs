using System.CommandLine;
using Microsoft.Extensions.Configuration;

// ReSharper disable StringLiteralTypo

namespace Emm.AppSettingsToEnv;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand($"Convert appsettings.json into environment variables");

        var appSettingsOption = new Option<FileInfo>(
            name: "--json",
            description: "appsettings.json");

        var levelOption = new Option<string>(
            name: "--level",
            description: "[user or machine]", getDefaultValue: () => "user");

        rootCommand.AddOption(appSettingsOption);
        rootCommand.AddOption(levelOption);

        rootCommand.SetHandler(Run, appSettingsOption, levelOption);

        return await rootCommand.InvokeAsync(args);

    }

    private static EnvironmentVariableTarget GetEnvTarget(string target) => target switch
    {
        "machine" => EnvironmentVariableTarget.Machine,
        _ => EnvironmentVariableTarget.User
    };

    private static string EncodeKey(string key)
    {
        return key.Replace(":", "__");
    }
    
    private static void Run(FileInfo appSettingsFile, string target)
    {
        if (!appSettingsFile.Exists)
        {
            Console.WriteLine($"{appSettingsFile.FullName} doesnt exist");
        }

        try
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsFile.FullName, optional: false, reloadOnChange: false)
                .Build();

            var envTarget = GetEnvTarget(target);

            foreach (var kv in configuration.AsEnumerable())
            {
                if (kv.Value != null)
                {
                    Console.WriteLine($"{EncodeKey(kv.Key)}={kv.Value}");
                }
            }
            Console.Write($"Apply These variables to the current environmen({target})? [Y/N]: ");
            var result = Console.ReadKey();
            Console.WriteLine();
            if (result.Key == ConsoleKey.Y)
            {
                foreach (var kv in configuration.AsEnumerable())
                {
                    if (kv.Value != null)
                    {
                        Environment.SetEnvironmentVariable(EncodeKey(kv.Key), kv.Value, envTarget);
                    }
                }
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load appsettings variables: {ex.Message}");
        }
    }
}