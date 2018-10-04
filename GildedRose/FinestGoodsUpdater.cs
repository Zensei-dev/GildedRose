using System;
using System.Collections.Generic;
using System.Text;

namespace GildedRose
{
    /// <summary>
    /// Responsible for applying daily inventory updates to Finest Goods
    /// </summary>
    public class FinestGoodUpdater
    {
        /// <summary>
        /// Apply daily inventory updates to a provided IEnumerable collection of Finest Goods
        /// </summary>
        /// <param name="finestGoods">An IEnumerable collection of FinestGood</param>
        public void UpdateGoods(IEnumerable<FinestGood> finestGoods)
        {
            foreach (var good in finestGoods)
                UpdateGood(good);
        }

        /// <summary>
        /// Apply daily inventory updates to a provided Finest Good
        /// </summary>
        /// <param name="finestGood">The Finest Good to apply inventory updates to</param>
        public void UpdateGood(FinestGood finestGood)
        {
            // Find and use the correct ItemUpdater for finestGood
            switch (finestGood.ItemName.ToLower()) // Allow case inconsistencies
            {
                case "aged brie":
                    new AgedBrieItemUpdaterRuleset().ApplyUpdates(finestGood);
                    break;
                case "backstage passes":
                    new BackstagePassesItemUpdaterRuleset().ApplyUpdates(finestGood);
                    break;
                case "sulfuras":
                    new SulfurasItemUpdaterRuleset().ApplyUpdates(finestGood);
                    break;
                case "normal item":
                    new NormalItemUpdaterRuleset().ApplyUpdates(finestGood);
                    break;
                case "conjured":
                    new ConjuredItemUpdaterRuleset().ApplyUpdates(finestGood);
                    break;
                default:
                    finestGood.ItemName = "NO SUCH ITEM"; // 2 strikes. See OutFinestGoods.cs note
                    break;
            }
        }
    }
}
