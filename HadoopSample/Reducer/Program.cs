using System;
using System.Collections.Generic;

namespace reducer
{
    class Program
    {
        /// <summary>
        /// Mapper project has extracted all words into the STDOUT in a key-value form word\t1.
        /// Hodoop cluster reads that output (STDOUT of mapper) and writes it to the STDIN of Reducer (this project).
        /// Reducer counts all words and writes the counter result in STDOUT in the same key-value form: "WordABC\t281"
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Dictionary for holding a count of words
            Dictionary<string, int> words = new Dictionary<string, int>();

            string line;
            // Read from STDIN
            while ((line = Console.ReadLine()) != null)
            {
                // Data from Hadoop is tab-delimited key/value pairs
                var sArr = line.Split('\t');
               
                // Get the word
                string word = sArr[0];
                
                // Get the count
                int count = Convert.ToInt32(sArr[1]);

                // Do we already have a count for the word?
                if (words.ContainsKey(word))
                {
                    //If so, increment the count
                    words[word] += count;
                }
                else
                {
                    //Add the key to the collection
                    words.Add(word, count);
                }
            }
            //
            // Finally, emit each word and count
            foreach (var word in words)
            {
                // Emit tab-delimited key/value pairs.
                // In this case, a word and a word count.
                Console.WriteLine($"{word.Key}\t{word.Value}");
            }
        }
    }
}