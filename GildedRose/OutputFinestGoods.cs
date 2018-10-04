using System;
using System.Collections.Generic;
using System.Text;

namespace GildedRose
{
    // Keep this simple for now, could consider interface etc if we expect there to be multiple output sources
    // Already we could consider CLI output / file output - but I'll just go with CLI for now to get it up and running faster
    public class OutputFinestGoodsCli
    {
        public void OutputGoods(IEnumerable<FinestGood> finestGoods)
        {
            Console.WriteLine("");
            Console.WriteLine(""); // Whitespace to buffer output
            Console.WriteLine("=========================================");
            Console.WriteLine("RESULTS: Finest Goods After 1 Elapsed Day");
            Console.WriteLine("=========================================");
            Console.WriteLine("ITEMNAME | SELLIN | QUALITY");
            Console.WriteLine("---------------------------");
            foreach(var finestGood in finestGoods)
            {
                // I'm not overly happy with the "NO SUCH ITEM" repetition. We're on 2 strikes
                // I'd look for a round of app feedback first and seek to potentially implement a better error handling system (inc. Exceptions in parse class)
                if (finestGood.ItemName == "NO SUCH ITEM") 
                    Console.WriteLine(finestGood.ItemName); // Don't print values in this case
                else
                    Console.WriteLine(string.Format("{0} {1} {2}", finestGood.ItemName, finestGood.SellIn, finestGood.Quality));
            }
            Console.WriteLine("---------------------------");
            Console.WriteLine("");
        }
    }
}
