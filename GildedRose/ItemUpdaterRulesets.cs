using System;
using System.Collections.Generic;
using System.Text;

namespace GildedRose
{
    /// <summary>
    /// Base abstract class for an ItemUpdaterRuleset
    /// Defines some common default behaviour that can be overridden for different Item rulesets
    /// </summary>
    public abstract class ItemUpdaterRuleset
    {
        /// <summary>
        /// Apply the updates relating to the ItemUpdaterRuleset
        /// </summary>
        /// <param name="finestGood">The FinestGood to apply updates to</param>
        public virtual void ApplyUpdates(FinestGood finestGood)
        {
            ApplyPreUpdates(finestGood);
            ApplyGeneralUpdates(finestGood);
            ApplyPostUpdates(finestGood);
        }

        // Updates that must be applied first. Default behaviour defined
        protected virtual void ApplyPreUpdates(FinestGood finestGood)
        {
            // 1. Daily SellIn Adjustment
            finestGood.SellIn = InventoryManagementRules.GetUpdatedSellIn(finestGood.SellIn);
        }

        // Assign item specific rules here
        protected abstract void ApplyGeneralUpdates(FinestGood finestGood);

        // Updates that must be applied last. Default behaviour defined
        protected virtual void ApplyPostUpdates(FinestGood finestGood)
        {
            // 1. Verify minimum Quality
            finestGood.Quality = InventoryManagementRules.GetMinimumAdjustedQuality(finestGood.Quality);
            // 2. Verify Maxium Quality
            finestGood.Quality = InventoryManagementRules.GetMaximumAdjustedQuality(finestGood.Quality);
        }
    }

    /// <summary>
    /// Define the Ruleset that should be applied to items of type "Aged Brie"
    /// </summary>
    public class AgedBrieItemUpdaterRuleset : ItemUpdaterRuleset
    {
        protected override void ApplyGeneralUpdates(FinestGood finestGood)
        {
            finestGood.Quality = InventoryManagementRules.GetMaturedQuality(finestGood.Quality);
        }
    }

    /// <summary>
    /// Define the Ruleset that should be applied to items of type "Backstage Passes"
    /// </summary>
    public class BackstagePassesItemUpdaterRuleset : ItemUpdaterRuleset
    {
        protected override void ApplyGeneralUpdates(FinestGood finestGood)
        {
            finestGood.Quality = InventoryManagementRules.GetEventQuality(finestGood.Quality, finestGood.SellIn);
        }
    }

    /// <summary>
    /// Define the Ruleset that should be applied to items of type "Sulfuras"
    /// </summary>
    public class SulfurasItemUpdaterRuleset : ItemUpdaterRuleset
    {
        protected override void ApplyPreUpdates(FinestGood finestGood)
        {
            // Sulfuras values are never changed
        }

        protected override void ApplyGeneralUpdates(FinestGood finestGood)
        {
            // Sulfuras values are never changed
        }

        protected override void ApplyPostUpdates(FinestGood finestGood)
        {
            // Sulfuras values are never changed
        }
    }

    /// <summary>
    /// Define the Ruleset that should be applied to items of type "Normal Item"
    /// </summary>
    public class NormalItemUpdaterRuleset : ItemUpdaterRuleset
    {
        protected override void ApplyGeneralUpdates(FinestGood finestGood)
        {
            var sellInPassed = InventoryManagementRules.HasSellInPassed(finestGood.SellIn);
            finestGood.Quality = InventoryManagementRules.GetDegradedQuality(finestGood.Quality, sellInPassed);
        }
    }

    /// <summary>
    /// Define the Ruleset that should be applied to items of type "Conjured"
    /// </summary>
    public class ConjuredItemUpdaterRuleset : ItemUpdaterRuleset
    {
        protected override void ApplyGeneralUpdates(FinestGood finestGood)
        {
            var sellInPassed = InventoryManagementRules.HasSellInPassed(finestGood.SellIn);
            finestGood.Quality = InventoryManagementRules.GetConjuredQuality(finestGood.Quality, sellInPassed);
        }
    }
}
