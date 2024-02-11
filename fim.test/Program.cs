using fim.celestia;
using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim.test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string CURRENT_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;
            string FILE = "conditional.fim";
            string PATH = Path.GetFullPath(Path.Combine(CURRENT_DIRECTORY, @"..\..\..\Reports", FILE));

            string letter = File.ReadAllText(PATH);
            var tokens = Lexer.Parse(letter);

            Console.WriteLine(string.Join("\n", tokens.Select(t => $"{t.Value,-32} - {t.Type}")));

            var report = Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);


            Console.WriteLine(new string('-', Console.BufferWidth));

            Console.WriteLine($"\"{i.ReportName}\" by {i.ReportAuthor}");
            Console.WriteLine(new string('-', Console.BufferWidth));
            Console.WriteLine(letter);
            Console.WriteLine(new string('-', Console.BufferWidth));
            i.MainParagraph?.Execute();
            Console.WriteLine(new string('-', Console.BufferWidth));
        }
    }
}