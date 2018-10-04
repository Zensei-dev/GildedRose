using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GildedRose.Tests
{
    public class InventoryManagementRulesTests
    {
        /*
         * NOTES ABOUT TESTS IN THIS FILE: 
         * Aren't we duplicating const's and logic from InventoryManagementRules, almost writing out the code twice?
         * 1. We shouldn't reference internals of that class because the point is to test them
         *      e.g. if a const in that class got changed to a wrong value, the tests will pick that up quickly
         * 2. It's mission critical business logic, we definitely want this stuff rigorously tested with separate code
         */

        public class TheGetUpdatedSellInMethod
        {
            [Theory]
            [InlineData(int.MaxValue)]
            [InlineData(36500)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)] // This rolls over to int.MaxValue. At nearly 6 million years past it's SellIn date it's an edge case we can ignore
            public void ReturnsUpdatedSellIn(int sellIn)
            {
                const int SELLIN_STANDARD_ADJUSTMENT = 1;

                int updatedSellIn = InventoryManagementRules.GetUpdatedSellIn(sellIn);

                Assert.Equal(sellIn - SELLIN_STANDARD_ADJUSTMENT, updatedSellIn);
            }
        }

        public class TheHasSellInPassedMethod
        {
            [Theory]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]

            public void ReturnsTrueWhenSellInPassed(int sellIn)
            {
                bool sellInPassed = InventoryManagementRules.HasSellInPassed(sellIn);

                Assert.True(sellInPassed);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(36500)]
            [InlineData(int.MaxValue)]
            public void ReturnsFalseWhenSellInNotPassed(int sellIn)
            {
                bool sellInPassed = InventoryManagementRules.HasSellInPassed(sellIn);

                Assert.False(sellInPassed);
            }
        }

        public class TheGetDegradedQualityMethod
        {
            public const int QUALITY_DEGRADES_BY = 1;
            public const int SELLIN_PASSED_MULTIPLIER = 2;

            [Theory]
            [InlineData(int.MaxValue)]
            [InlineData(36500)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void QualityDegradesNormallyWhenSellInNotPassed(int quality)
            {
                bool sellInPassed = false;

                int exectedDegradedQuality = quality - QUALITY_DEGRADES_BY;

                int degradedQuality = InventoryManagementRules.GetDegradedQuality(quality, sellInPassed);

                Assert.Equal(exectedDegradedQuality, degradedQuality);
            }

            [Theory]
            [InlineData(int.MaxValue)]
            [InlineData(36500)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void QualityDegradesDoubleWhenSellInPassed(int quality)
            {
                bool sellInPassed = true;

                int expectedDegradedQuality = quality - (QUALITY_DEGRADES_BY * SELLIN_PASSED_MULTIPLIER);

                int degradedQuality = InventoryManagementRules.GetDegradedQuality(quality, sellInPassed);

                Assert.Equal(expectedDegradedQuality, degradedQuality);
            }
        }

        public class TheGetMaturedQualityMethod
        {
            public const int QUALITY_MATURES_BY = 1;

            [Theory]
            [InlineData(int.MaxValue)]
            [InlineData(36500)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void IncreasesQuality(int quality)
            {
                int expectedMaturedQuality = quality + QUALITY_MATURES_BY;

                var maturedQuality = InventoryManagementRules.GetMaturedQuality(quality);

                Assert.Equal(expectedMaturedQuality, maturedQuality);
            }
        }

        public class TheGetEventQualityMethod
        {
            [Theory] // EVENT MATURES BEFORE OTHER RULES: SellIn > 10
            [InlineData(int.MinValue, 11)]
            [InlineData(-100, 11)]
            [InlineData(-1, 11)]
            [InlineData(0, 11)]
            [InlineData(1, 11)]
            [InlineData(100, 36500)]
            [InlineData(int.MaxValue, int.MaxValue)]
            public void QualityIncreasesBeforeEvent(int quality, int sellIn)
            {
                int expectedQuality = quality + TheGetMaturedQualityMethod.QUALITY_MATURES_BY;

                var updatedQuality = InventoryManagementRules.GetEventQuality(quality, sellIn);

                Assert.Equal(expectedQuality, updatedQuality);
            }

            [Theory] // EVENT CLOSE : SellIn = 6 - 10 days
            [InlineData(int.MinValue, 10)]
            [InlineData(-100, 9)]
            [InlineData(-1, 8)]
            [InlineData(0, 7)]
            [InlineData(1, 6)]
            [InlineData(100, 6)]
            [InlineData(int.MaxValue, 6)]
            public void QualityIncreasesMoreWhenEventClose(int quality, int sellIn)
            {
                const int EVENT_CLOSE_QUALITY_INCREASE = 2;

                int expectedQuality = quality + EVENT_CLOSE_QUALITY_INCREASE;

                int updatedQuality = InventoryManagementRules.GetEventQuality(quality, sellIn);

                Assert.Equal(expectedQuality, updatedQuality);
            }

            [Theory] // EVENT IMMINENT : SellIn = 0 - 5 days
            [InlineData(int.MinValue, 5)]
            [InlineData(-100, 4)]
            [InlineData(-1, 3)]
            [InlineData(0, 2)]
            [InlineData(1, 1)]
            [InlineData(100, 0)]
            [InlineData(int.MaxValue, 0)]
            public void QualityIncreasesMostWhenEventImminent(int quality, int sellIn)
            {
                const int EVENT_IMMINENT_QUALITY_INCREASE = 3;

                int expectedQuality = quality + EVENT_IMMINENT_QUALITY_INCREASE;

                int updatedQuality = InventoryManagementRules.GetEventQuality(quality, sellIn);

                Assert.Equal(expectedQuality, updatedQuality);
            }

            [Theory] // EVENT PASSED : SellIn < 0 (or explicitly InventoryManagementRules.HasSellInPassed = True)
            [InlineData(int.MinValue, -1)]
            [InlineData(-100, -10)]
            [InlineData(-1, -100)]
            [InlineData(0, -1000)]
            [InlineData(1, -365000)]
            [InlineData(100, -365000)]
            [InlineData(int.MaxValue, int.MinValue)]
            public void ReturnsZeroWhenSellInPassed(int quality, int sellIn)
            {
                const int SELLIN_PASSED_QUALITY = 0;

                int expectedQuality = SELLIN_PASSED_QUALITY;

                int updatedQuality = InventoryManagementRules.GetEventQuality(quality, sellIn);

                Assert.Equal(expectedQuality, updatedQuality);
            }
        }

        public class TheGetConjuredQualityMethod
        {
            const int CONJURED_ADDITIONAL_DEGRADES_MULTIPLIER = 2;

            [Theory]
            [InlineData(int.MaxValue)]
            [InlineData(36500)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void DegradesAtConjuredRateDuringSellIn(int quality)
            {
                bool sellInPassed = false;

                int exectedDegradedQuality = quality - (TheGetDegradedQualityMethod.QUALITY_DEGRADES_BY * CONJURED_ADDITIONAL_DEGRADES_MULTIPLIER);

                int degradedQuality = InventoryManagementRules.GetConjuredQuality(quality, sellInPassed);

                Assert.Equal(exectedDegradedQuality, degradedQuality);
            }

            [Theory]
            [InlineData(int.MaxValue)]
            [InlineData(36500)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void DegradesExtraConjuredRatePastSellIn(int quality)
            {
                bool sellInPassed = true;

                int expectedDegradedQuality = quality - (TheGetDegradedQualityMethod.QUALITY_DEGRADES_BY * TheGetDegradedQualityMethod.SELLIN_PASSED_MULTIPLIER * CONJURED_ADDITIONAL_DEGRADES_MULTIPLIER);

                int degradedQuality = InventoryManagementRules.GetConjuredQuality(quality, sellInPassed);

                Assert.Equal(expectedDegradedQuality, degradedQuality);
            }
        }

        public class TheGetSulfurasQualityMethod
        {
            public void NothingHappens()
            {
                // Method does nothing but serve as explicit confirmation that nothing happens
                InventoryManagementRules.GetSulfurasQuality();

                Assert.True(true); // Check we made it past Sulfuras I guess!
            }
        }

        public class TheGetMinimumAdjustedQualityMethod
        {
            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(100)]
            [InlineData(36500)]
            [InlineData(int.MaxValue)]
            public void QualityUnchangedAboveMinimum(int quality)
            {
                int expectedQuality = quality;

                int potentiallyUpdatedQuality = InventoryManagementRules.GetMinimumAdjustedQuality(quality);

                Assert.Equal(expectedQuality, potentiallyUpdatedQuality);
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(-100)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void QualityAdjustedBelowMinimum(int quality)
            {
                const int MINIMUM_QUALITY = 0;

                int expectedQuality = MINIMUM_QUALITY;

                int updatedQuality = InventoryManagementRules.GetMinimumAdjustedQuality(quality);

                Assert.Equal(expectedQuality, updatedQuality);
            }
        }

        public class TheGetMaximumAdjustedQualityMethod
        {
            [Theory]
            [InlineData(50)]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-36500)]
            [InlineData(int.MinValue)]
            public void QualityUnchangedBelowMaximum(int quality)
            {
                int expectedQuality = quality;

                int potentiallyUpdatedQuality = InventoryManagementRules.GetMaximumAdjustedQuality(quality);

                Assert.Equal(expectedQuality, potentiallyUpdatedQuality);
            }

            [Theory]
            [InlineData(51)]
            [InlineData(500)]
            [InlineData(36500)]
            [InlineData(int.MaxValue)]
            public void QualityAdjustedAboveMaximum(int quality)
            {
                const int MAXIMUM_QUALITY = 50;

                int expectedQuality = MAXIMUM_QUALITY;

                int updatedQuality = InventoryManagementRules.GetMaximumAdjustedQuality(quality);

                Assert.Equal(expectedQuality, updatedQuality);
            }
        }
    }
}
