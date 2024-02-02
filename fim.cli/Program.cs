using fim;

namespace fim.cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Letter(
                """
                Dear Princess Celestia: Hello World!
                Today I learned how to say hello world!
                    Did you know that Spike is the word "Hello World"? (Spike will hold our variable "Hello World".)
                    I said Spike.
                That's all about how to say hello world.
                Your faithful student, Twilight Sparkle.
                P.S. This is a comment!
                """
            );
        }
    }
}