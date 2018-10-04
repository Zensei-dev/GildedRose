using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GildedRose
{
    // Keep this simple for now in regard to error checking etc
    // Could also consider interface if we expect there to be additional input sources
    class GoodsFileReader
    {
        public List<FinestGood> GetGoods(string goodsFile)
        {
            var rawGoods = File.ReadAllLines(goodsFile);

            var finestGoods = new List<FinestGood>();

            foreach (var rawGood in rawGoods)
            {
                FinestGood finestGood = CreateGood(rawGood);
                finestGoods.Add(finestGood);
            }

            return finestGoods;
        }

        private FinestGood CreateGood(string rawFoodData)
        {
            // EXPECTED FORMAT (space delimited): rawFoodData = { [string GOODTYPE] [int SELLIN] [int QUALITY] }

            // GOODTYPE may have multiple spaces, we will parse backwards to grab ints
            var rawFoodDataSplit = rawFoodData.Split(' ');

            // Must have at least 3 elements, 2 ints and a name (could have > 3 if name with spaces)
            if (rawFoodData.Length < 3)
            {
                // I would tend to hard error until getting more feedback from customer about error handling (and normally define custom exceptions)
                throw new Exception("Invalid data for Finest Good: " + rawFoodData);
            }

            int qualityArrayLocation = rawFoodDataSplit.Length - 1; // Quality = last in array

            int Quality; // Should be last int
            if (!int.TryParse(rawFoodDataSplit[qualityArrayLocation], out Quality))
            {
                // I would tend to hard error until getting more feedback from customer about error handling (and normally define custom exceptions)
                throw new Exception("Unable to parse Quality from Finest Good data: " + rawFoodData);
            }

            int sellInArrayLocation = rawFoodDataSplit.Length - 2; // SellIn = second to last in array

            int SellIn; // Should be second to last int
            if (!int.TryParse(rawFoodDataSplit[sellInArrayLocation], out SellIn))
            {
                // I would tend to hard error until getting more feedback from customer about error handling (and normally define custom exceptions)
                throw new Exception("Unable to parse SellIn from Finest Good data: " + rawFoodData);
            }

            int itemNameStartsAt = 0;
            int itemNameEndsAt = rawFoodDataSplit.Length - 2; // 2 elements at end of array
            string ItemName = string.Join(" ", rawFoodDataSplit, itemNameStartsAt, itemNameEndsAt);

            return new FinestGood(ItemName, SellIn, Quality);
        }
    }
}
