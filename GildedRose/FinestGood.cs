using System;
using System.Collections.Generic;
using System.Text;

namespace GildedRose
{
    public class FinestGood
    {
        public string ItemName { get; set; }
        public int SellIn { get; set; }
        public int Quality { get; set; }

        public FinestGood(string itemName, int sellIn, int quality)
        {
            ItemName = itemName;
            SellIn = sellIn;
            Quality = quality;
        }
    }
}
