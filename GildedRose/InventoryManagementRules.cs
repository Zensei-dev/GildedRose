using System;
using System.Collections.Generic;
using System.Text;

namespace GildedRose
{
    /// <summary>
    /// Contains the individual business rules for applying inventory management daily updates
    /// </summary>
    public static class InventoryManagementRules
    {
        // NOTE: Arguably we should list our const's here but I've chosen to keep them scoped to where they are needed for now

        /// <summary>
        /// Get an updated SellIn value according to the standard daily adjustment rule
        /// </summary>
        /// <param name="sellIn">The initial SellIn value</param>
        /// <returns>An updated SellIn value after applying the standard daily adjustment rule</returns>
        public static int GetUpdatedSellIn(int sellIn)
        {
            const int SELLIN_ADJUSTMENT = 1;

            return sellIn - SELLIN_ADJUSTMENT;
        }

        /// <summary>
        /// Determine if a SellIn date has passed or not
        /// </summary>
        /// <param name="sellIn">The SellIn value to check</param>
        /// <returns>True if the SellIn date has passed, false otherwise</returns>
        public static bool HasSellInPassed(int sellIn)
        {
            const int SELLIN_CUTOFF = 0;

            if (sellIn < SELLIN_CUTOFF)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get an updated Quality value for an item that degrades over time
        /// </summary>
        /// <param name="initialQuality">The initial Quality value</param>
        /// <param name="sellInPassed">Whether the SellIn date has passed or not</param>
        /// <returns>An updated Quality value after applying the quality degrades rule</returns>
        public static int GetDegradedQuality(int initialQuality, bool sellInPassed)
        {
            return initialQuality - getStandardDegradesByAmount(sellInPassed);
        }
        // We need the raw value in multiple places so let's break this out
        private static int getStandardDegradesByAmount(bool sellInPassed)
        {
            const int QUALITY_DEGRADES_BY = 1;
            const int SELLIN_PASSED_MULTIPLIER = 2;

            if (sellInPassed)
                return (QUALITY_DEGRADES_BY * SELLIN_PASSED_MULTIPLIER);
            else
                return QUALITY_DEGRADES_BY;
        }

        /// <summary>
        /// Get an updated Quality value for an item that matures over time
        /// </summary>
        /// <param name="initialQuality">The initial Quality value</param>
        /// <returns>An updated Quality value after applying the quality matures rule</returns>
        public static int GetMaturedQuality(int initialQuality)
        {
            const int QUALITY_MATURES_BY = 1;

            return initialQuality + QUALITY_MATURES_BY;
        }

        /// <summary>
        /// Get an updated Quality for an item that is closely tied to an event and varies based on SellIn date
        /// </summary>
        /// <param name="initialQuality">The initial Quality value</param>
        /// <param name="sellIn">The SellIn date of the item</param>
        /// <returns>An updated Quality value after applying the quality event rule</returns>
        public static int GetEventQuality(int initialQuality, int sellIn)
        {
            // Constants related to events
            const int EVENT_CLOSE_THRESHOLD = 10;
            const int EVENT_CLOSE_QUALITY_INCREASE = 2;

            const int EVENT_IMMINENT_THRESHOLD = 5;
            const int EVENT_IMMINENT_QUALITY_INCREASE = 3;

            const int SELLIN_PASSED_QUALITY = 0;

            // Adjust quality based on how close to event (or event passed)
            bool sellInPassed = HasSellInPassed(sellIn);

            if (sellInPassed)
                return SELLIN_PASSED_QUALITY;

            if (sellIn <= EVENT_IMMINENT_THRESHOLD)
                return initialQuality + EVENT_IMMINENT_QUALITY_INCREASE;

            if (sellIn <= EVENT_CLOSE_THRESHOLD)
                return initialQuality + EVENT_CLOSE_QUALITY_INCREASE;

            // ASSUMPTION: Quality increase like Aged Brie if we are far out from event (> eventCloseThreshold) (interpretation of rule)
            return GetMaturedQuality(initialQuality);
        }

        /// <summary>
        /// Get an updated Quality for a conjured item
        /// </summary>
        /// <param name="initialQuality">The initial Quality value</param>
        /// <param name="sellInPassed">Whether the SellIn date has passed or not</param>
        /// <returns>An updated Quality value after applying conjured quality rule</returns>
        public static int GetConjuredQuality(int initialQuality, bool sellInPassed)
        {
            const int CONJURED_ADDITIONAL_DEGRADES_MULTIPLIER = 2;

            int conjuredDegradeAmount = getStandardDegradesByAmount(sellInPassed) * CONJURED_ADDITIONAL_DEGRADES_MULTIPLIER;

            return initialQuality - conjuredDegradeAmount;
        }

        /// <summary>
        /// Sulfuras values do not change. This is here for clarification. This method does nothing.
        /// </summary>
        public static void GetSulfurasQuality()
        {
            // RULE: Sulfuras values do not change
        }
        
        /// <summary>
        /// Adjust the Quality value if it is below the minimum value
        /// </summary>
        /// <param name="finalDailyQuality">The final Quality value after other daily rules have been applied</param>
        /// <returns>A quality value that satisfies the minimum quality rule</returns>
        public static int GetMinimumAdjustedQuality(int finalDailyQuality)
        {
            const int MINIMUM_QUALITY = 0;

            if (finalDailyQuality < MINIMUM_QUALITY)
                return MINIMUM_QUALITY;
            else
                return finalDailyQuality;
        }

        /// <summary>
        /// Adjust the Quality value if it is above the maximum value
        /// </summary>
        /// <param name="finalDailyQuality">The final Quality value after other daily rules have been applied</param>
        /// <returns>A quality value that satisfies the maximum quality rule</returns>
        public static int GetMaximumAdjustedQuality(int finalDailyQuality)
        {
            const int MAXIMUM_QUALITY = 50;

            if (finalDailyQuality > MAXIMUM_QUALITY)
                return MAXIMUM_QUALITY;
            else
                return finalDailyQuality;
        }
    }
}
