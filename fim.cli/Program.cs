using System.Runtime.InteropServices;
using System.Reflection;
using Mono.Options;

namespace fim.cli
{
    internal class Program
    {
        static bool FileExists(string path)
        {
            return File.Exists(Path.GetFullPath(path));
        }
        static string GetVersion()
        {
            var version = Assembly.GetAssembly(typeof(Letter))!.GetName().Version;
            return $"{version!.Major}.{version.Minor}.{version.Build}";
        }
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string report_path = "";
                bool pretty = false;
                bool show_help = false;

                OptionSet p = new OptionSet()
                    .Add("p|pretty", "Prettify console output.", v => pretty = true)
                    .Add("h|help", "Show this message and exit.", v => show_help = true);
                List<string> extra = p.Parse(args);

                if (show_help)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        Console.WriteLine("Usage: fim [options] <report_path>");
                    else
                        Console.WriteLine("Usage: ./fim [options] <report_path>");

                    Console.WriteLine("Interprets the specified FiM++ report.");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    p.WriteOptionDescriptions(Console.Out);
                    return;
                }

                if (extra.Count > 0) report_path = extra[0];
                if (string.IsNullOrWhiteSpace(report_path) || !FileExists(report_path))
                    throw new FileNotFoundException($"Cannot find report '{report_path}'");

#if DEBUG
                var debugTime = new System.Diagnostics.Stopwatch();
                debugTime.Start();
#endif

                var interpreter = Letter.WriteLetter(File.ReadAllText(report_path));

                if (pretty)
                {
                    Console.WriteLine($"[ FiM v{GetVersion()} ]");
                    Console.WriteLine($"Report Name: {interpreter.ReportName}");
                    Console.WriteLine($"Student Name: {interpreter.ReportAuthor}");
                    Console.WriteLine("[@]===[@]");
                }

                interpreter.MainParagraph?.Execute();

                if (pretty)
                {
                    Console.WriteLine("[@]===[@]");
                }

#if DEBUG
                Console.WriteLine($"[Debug] Code execution took {debugTime!.Elapsed:d\\.hh\\:mm\\:ss\\:fff}.");
#endif
            }
        }
    }
}