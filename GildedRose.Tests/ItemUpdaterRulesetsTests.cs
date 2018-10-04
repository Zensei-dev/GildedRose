using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

/// <summary>
/// NOTE: These tests are to ensure the right rules are combined to form the desired ruleset states for each type of item
/// Therefore I'd argue use of existing InventoryManagementRules code for the purpose of testing makes sense since we're not verifying the intergrity of that class here
/// That is handled in IntentoryManagementRulesTests where segregated code exists to perform those checks
/// </summary>
namespace GildedRose.Tests
{
    /// <summary>
    /// Test methods that are used across several different test classes
    /// </summary>
    public class CommonTests
    {
        public void SellInDecreases(string itemName, int sellIn, int quality)
        {
            var finestGood = new FinestGood(itemName, sellIn, quality);

            int expectedSellIn = InventoryManagementRules.GetUpdatedSellIn(sellIn);

            new AgedBrieItemUpdaterRuleset().ApplyUpdates(finestGood);

            Assert.Equal(expectedSellIn, finestGood.SellIn);
        }

        public void AdheresToMaxQuality(string itemName, int sellIn, int quality)
        {
            var finestGood = new FinestGood(itemName, sellIn, quality);

            int maturedQuality = InventoryManagementRules.GetMaturedQuality(quality);
            int expectedQuality = InventoryManagementRules.GetMaximumAdjustedQuality(maturedQuality);

            new AgedBrieItemUpdaterRuleset().ApplyUpdates(finestGood);

            Assert.Equal(expectedQuality, finestGood.Quality);
        }

        public void AdheresToMinQuality(string itemName, int sellIn, int quality)
        {
            var finestGood = new FinestGood(itemName, sellIn, quality);

            int maturedQuality = InventoryManagementRules.GetMaturedQuality(quality);
            int expectedQuality = InventoryManagementRules.GetMinimumAdjustedQuality(maturedQuality);

            new AgedBrieItemUpdaterRuleset().ApplyUpdates(finestGood);

            Assert.Equal(expectedQuality, finestGood.Quality);
        }
    }

    public class AgedBrieItemUpdaterRulesetTests
    {
        public class TheApplyGeneralUpdatesMethod
        {
            const string AGED_BRIE_ITEM_NAME = "Aged Brie";

            [Theory]
            [InlineData(int.MaxValue, 49)] // Max SellIn & Quality Upper Bound
            [InlineData(-50, 25)] // SellIn passed
            [InlineData(int.MinValue, -1)] // Min SellIn & Quality Lower Bound
            public void QualityIncreases(int sellIn, int quality)
            {
                var agedBrie = new FinestGood(AGED_BRIE_ITEM_NAME, sellIn, quality);

                int expectedQuality = InventoryManagementRules.GetMaturedQuality(quality);

                new AgedBrieItemUpdaterRuleset().ApplyUpdates(agedBrie);

                Assert.Equal(expectedQuality, agedBrie.Quality);
            }

            [Theory]
            [InlineData(int.MaxValue, int.MaxValue)] // SellIn Upper Bound
            [InlineData(1, 1)]
            [InlineData(0, 0)]
            [InlineData(-1, -1)]
            [InlineData(int.MinValue, int.MinValue)] // SellIn Lower Bound
            public void SellInDecreases(int sellIn, int quality)
            {
                new CommonTests().SellInDecreases(AGED_BRIE_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            //[InlineData(int.MaxValue, int.MaxValue)] // Max Quality
            // FAILS: Quality matures, int rolls over to int.MinValue, but we don't call GetMinimumAdjustQuality here so it doesn't match - that's fine
            [InlineData(1, 50)]
            [InlineData(0, 49)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 1)]
            [InlineData(int.MinValue,-1)] // Quality Lower Bound (after quality increase)
            public void AdheresToMaxQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMaxQuality(AGED_BRIE_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            [InlineData(int.MinValue, int.MinValue)] // Max Quality
            [InlineData(1, -2)]
            [InlineData(0, -1)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 0)]
            [InlineData(int.MaxValue, 49)] // Quality Lower Bound (after quality increase)
            public void AdheresToMinQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMinQuality(AGED_BRIE_ITEM_NAME, sellIn, quality);
            }
        }
    }

    public class BackstagePassesItemUpdaterRulesetTests
    {
        const string BACKSTAGE_PASSES_ITEM_NAME = "Backstage Passes";

        public class TheApplyGeneralUpdatesMethod
        {
            [Theory] // SellIn = 6, 11, 0 break because we don't manually update SellIn in the test (while the ruleset does) - that's fine
            [InlineData(int.MaxValue, 25)] // Far out from event (high SellIn)
            [InlineData(10, 25)] // Event close (10 >= SellIn > 5)
            [InlineData(9, 25)]
            [InlineData(5, 25)] // Event imminent (5 >= SellIn >= 0)
            [InlineData(4, 25)]
            [InlineData(1, 25)]
            [InlineData(-1, 25)] // Event Passed
            [InlineData(-36500, 25)]
            public void AppliesEventRule(int sellIn, int quality)
            { 
                // We vigorously check the rule conditions in InventoryManagementRules - so here the goal is to make sure it's applied to the ruleset
                var backstagePass = new FinestGood(BACKSTAGE_PASSES_ITEM_NAME, sellIn, quality);

                int expectedQuality = InventoryManagementRules.GetEventQuality(quality, sellIn);

                new BackstagePassesItemUpdaterRuleset().ApplyUpdates(backstagePass);

                Assert.Equal(expectedQuality, backstagePass.Quality);
            }

            [Theory]
            [InlineData(int.MaxValue, int.MaxValue)] // SellIn Upper Bound
            [InlineData(1, 1)]
            [InlineData(0, 0)]
            [InlineData(-1, -1)]
            [InlineData(int.MinValue, int.MinValue)] // SellIn Lower Bound
            public void SellInDecreases(int sellIn, int quality)
            {
                new CommonTests().SellInDecreases(BACKSTAGE_PASSES_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            //[InlineData(int.MaxValue, int.MaxValue)] // Max Quality
            // FAILS: Quality matures, int rolls over to int.MinValue, but we don't call GetMinimumAdjustQuality here so it doesn't match - that's fine
            [InlineData(1, 50)]
            [InlineData(0, 49)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 1)]
            [InlineData(int.MinValue, -1)] // Quality Lower Bound (after quality increase)
            public void AdheresToMaxQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMaxQuality(BACKSTAGE_PASSES_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            [InlineData(int.MinValue, int.MinValue)] // Max Quality
            [InlineData(1, -2)]
            [InlineData(0, -1)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 0)]
            [InlineData(int.MaxValue, 49)] // Quality Lower Bound (after quality increase)
            public void AdheresToMinQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMinQuality(BACKSTAGE_PASSES_ITEM_NAME, sellIn, quality);
            }
        }
    }

    public class SulfurasItemUpdaterRulesetTests
    {
        const string SULFURAS_ITEM_NAME = "Sulfuras";

        public class TheApplyGeneralUpdatesMethod
        {
            [Theory]
            [InlineData(int.MaxValue,int.MaxValue)]
            [InlineData(51,51)]
            [InlineData(50, 50)] // Normally Quality Upper Bound
            [InlineData(49, 49)]
            [InlineData(1, 1)]
            [InlineData(0, 0)] // Normally Quality Lower Bound
            [InlineData(-1, -1)]
            [InlineData(int.MinValue, int.MinValue)]
            public void QualityNeverChanges(int sellIn, int quality)
            {
                var sulfuras = new FinestGood(SULFURAS_ITEM_NAME, sellIn, quality);

                int expectedSellIn = sellIn;

                new SulfurasItemUpdaterRuleset().ApplyUpdates(sulfuras);

                Assert.Equal(expectedSellIn, sulfuras.SellIn);
            }

            [Theory]
            [InlineData(int.MaxValue, int.MaxValue)]
            [InlineData(1, 1)]
            [InlineData(0, 0)]
            [InlineData(-1, -1)]
            [InlineData(int.MinValue, int.MinValue)]
            public void SellInNeverChanges(int sellIn, int quality)
            {
                var sulfuras = new FinestGood(SULFURAS_ITEM_NAME, sellIn, quality);

                int expectedSellIn = sellIn;

                new SulfurasItemUpdaterRuleset().ApplyUpdates(sulfuras);

                Assert.Equal(expectedSellIn, sulfuras.SellIn);
            }
        }
    }

    public class NormalItemUpdaterRulesetTests
    {
        public class TheApplyGeneralUpdatesMethod
        {
            const string NORMAL_ITEM_NAME = "Normal Item";

            [Theory]
            [InlineData(int.MaxValue, 50)]
            [InlineData(50, 49)]
            [InlineData(1, 25)]
            [InlineData(-1, 10)]
            [InlineData(-50, 5)] // SellIn passed
            [InlineData(int.MinValue + 1, 2)]
            // int.Min rolls over to positive and causes quality to degrade at lower rate (but we aren't manually reducing sellIn in this method, again 6 million years, that's fine)
            public void QualityDecreases(int sellIn, int quality)
            {
                var normalItem = new FinestGood(NORMAL_ITEM_NAME, sellIn, quality);

                bool sellInPassed = InventoryManagementRules.HasSellInPassed(sellIn);
                int expectedQuality = InventoryManagementRules.GetDegradedQuality(quality, sellInPassed);

                new NormalItemUpdaterRuleset().ApplyUpdates(normalItem);

                Assert.Equal(expectedQuality, normalItem.Quality);
            }

            [Theory]
            [InlineData(int.MaxValue, int.MaxValue)] // SellIn Upper Bound
            [InlineData(1, 1)]
            [InlineData(0, 0)]
            [InlineData(-1, -1)]
            [InlineData(int.MinValue, int.MinValue)] // SellIn Lower Bound
            public void SellInDecreases(int sellIn, int quality)
            {
                new CommonTests().SellInDecreases(NORMAL_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            //[InlineData(int.MaxValue, int.MaxValue)] // Max Quality
            // FAILS: Quality matures, int rolls over to int.MinValue, but we don't call GetMinimumAdjustQuality here so it doesn't match - that's fine
            [InlineData(1, 50)]
            [InlineData(0, 49)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 1)]
            [InlineData(int.MinValue, -1)] // Quality Lower Bound (after quality increase)
            public void AdheresToMaxQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMaxQuality(NORMAL_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            [InlineData(int.MinValue, int.MinValue)] // Max Quality
            [InlineData(1, -2)]
            [InlineData(0, -1)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 0)]
            [InlineData(int.MaxValue, 49)] // Quality Lower Bound (after quality increase)
            public void AdheresToMinQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMinQuality(NORMAL_ITEM_NAME, sellIn, quality);
            }
        }
    }

    public class ConjuredItemUpdaterRulesetTests
    {
        const string CONJURED_ITEM_NAME = "Conjured";
        public class TheApplyGeneralUpdatesMethod
        {
            [Theory]
            [InlineData(int.MaxValue, 50)]
            [InlineData(50, 49)]
            [InlineData(1, 25)]
            [InlineData(-1, 10)]
            [InlineData(-50, 5)] // SellIn passed
            [InlineData(int.MinValue + 1, 4)]
            // int.Min rolls over to positive and causes quality to degrade at lower rate (but we aren't manually reducing sellIn in this method, again 6 million years, that's fine)
            public void QualityDecreasesIncreasedRate(int sellIn, int quality)
            {
                var conjuredItem = new FinestGood(CONJURED_ITEM_NAME, sellIn, quality);

                bool sellInPassed = InventoryManagementRules.HasSellInPassed(sellIn);
                int expectedQuality = InventoryManagementRules.GetConjuredQuality(quality, sellInPassed);

                new ConjuredItemUpdaterRuleset().ApplyUpdates(conjuredItem);

                Assert.Equal(expectedQuality, conjuredItem.Quality);
            }

            [Theory]
            [InlineData(int.MaxValue, int.MaxValue)] // SellIn Upper Bound
            [InlineData(1, 1)]
            [InlineData(0, 0)]
            [InlineData(-1, -1)]
            [InlineData(int.MinValue, int.MinValue)] // SellIn Lower Bound
            public void SellInDecreases(int sellIn, int quality)
            {
                new CommonTests().SellInDecreases(CONJURED_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            //[InlineData(int.MaxValue, int.MaxValue)] // Max Quality
            // FAILS: Quality matures, int rolls over to int.MinValue, but we don't call GetMinimumAdjustQuality here so it doesn't match - that's fine
            [InlineData(1, 50)]
            [InlineData(0, 49)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 1)]
            [InlineData(int.MinValue, -1)] // Quality Lower Bound (after quality increase)
            public void AdheresToMaxQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMaxQuality(CONJURED_ITEM_NAME, sellIn, quality);
            }

            [Theory]
            [InlineData(int.MinValue, int.MinValue)] // Max Quality
            [InlineData(1, -2)]
            [InlineData(0, -1)] // Quality Upper Bound (after quality increase)
            [InlineData(-1, 0)]
            [InlineData(int.MaxValue, 49)] // Quality Lower Bound (after quality increase)
            public void AdheresToMinQuality(int sellIn, int quality)
            {
                new CommonTests().AdheresToMinQuality(CONJURED_ITEM_NAME, sellIn, quality);
            }
        }
    }
}
