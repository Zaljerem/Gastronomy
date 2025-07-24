using HarmonyLib;
using Verse;
using System;
using System.Reflection;
using Gastronomy.Dining;

namespace Gastronomy
{

    //This patch is for compatibility with AnimalControls
    [HarmonyPatch(typeof(ThingListGroupHelper), nameof(ThingListGroupHelper.Includes))]
    public static class Patch_ThingListGroupHelper_AnimalControls
    {
        private static bool AllowFeedingWithPlants()
        {
            // Get the AnimalControls.Settings type without referencing the assembly
            Type settingsType = Type.GetType("AnimalControls.Settings, AnimalControls");
            if (settingsType == null)
                return false;

            // Get the static property or field 'allow_feeding_with_plants'
            var field = settingsType.GetField("allow_feeding_with_plants", BindingFlags.Static | BindingFlags.Public);
            if (field == null)
                return false;

            return field.GetValue(null) as bool? ?? false;
        }

        static void Postfix(ThingRequestGroup group, ThingDef def, ref bool __result)
        {
            // Only modify if the original result is true and the group is relevant
            if (!__result || group != ThingRequestGroup.FoodSourceNotPlantOrTree)
                return;

            // Respect the AnimalControls setting (via reflection)
            if (!AllowFeedingWithPlants())
                return;

            // If the def is a DiningSpot, exclude it from being considered a food source
            if (typeof(DiningSpot).IsAssignableFrom(def.thingClass))
            {
                __result = false;
            }
        }
    }
}
