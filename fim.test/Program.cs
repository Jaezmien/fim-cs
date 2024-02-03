using fim.celestia;

namespace fim.test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string letter =
                """
                Dear Princess Celestia: Hello World!
                Did you know that Spike is the word "Hello World!"? (Spike will hold our variable "Hello World".)
                Today I learned how to say hello world!
                    Did you know that Owlowiscious is the word "Hello Equestria!"?
                    I quickly said Spike.
                    Spike became Owlowiscious!
                    I quickly said Spike.
                That's all about how to say hello world.
                Your faithful student, Twilight Sparkle.
                P.S. This is a comment!
                """;
            var i = Letter.WriteLetter(letter);

            Console.WriteLine($"\"{i.ReportName}\" by {i.ReportAuthor}");
            Console.WriteLine(new string('-', Console.BufferWidth));
            Console.WriteLine(letter);
            Console.WriteLine(new string('-', Console.BufferWidth));
            i.MainParagraph?.Execute();
            Console.WriteLine(new string('-', Console.BufferWidth));

            Variable? v = i.Variables.Get("Spike");
            if( v != null )
            {
                Console.WriteLine(v.Value);
            }
        }
    }
}