using System;
using System.Text.RegularExpressions;

namespace mapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            
            //
            // Hadoop passes data to the mapper on STDIN
            while ((line = Console.ReadLine()) != null)
            {
                // We only want words, so strip out punctuation, numbers, etc.
                var onlyText = Regex.Replace(line, @"\.|;|:|,|[0-9]|'", "");
                
                // Split at whitespace.
                var words = Regex.Matches(onlyText, @"[\w]+");
                
                //
                // Here we loop over all parsed words.
                foreach (var word in words)
                {
                    // Emit tab-delimited key/value pairs.
                    // In this case, a word and a default count of 1.
                    Console.WriteLine($"{word}\t1");
                }
            }
        }
    }
}