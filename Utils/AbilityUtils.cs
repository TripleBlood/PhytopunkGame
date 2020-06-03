using System;

namespace DefaultNamespace.Utils
{
    public class AbilityUtils
    {
        public static Type GetTargetingControllerType(string abilityName)
        {
            switch (abilityName)
            {
                case "MoveAndAttack":
                    return typeof(MoveAndAttackTargetingController);
                case "Overload":
                    return typeof(OverloadTargetingController);
                case "Adrenaline":
                    return typeof(AdrenalineTargetingController);
            }
            return typeof(MoveAndAttackTargetingController);
        }
        

        // Following methods better to pair with file readings to make balancing easier?
        public static string GetAbilityIcon(string abilityName)
        {
            switch (abilityName)
            {
                case "MoveAndAttack":
                    return "AbilityIcons/FlareAbIcon";
                case "Overload":
                    return "AbilityIcons/OverloadAbIcon";
                case "Flare":
                    return "AbilityIcons/FlareAbIcon";
                case "Adrenaline":
                    return "AbilityIcons/AdrenalineAbIcon";
            }
            return "AbilityIcons/FlareAbIcon";
        }

        public static int[] GetAbilityCost(string abilityName)
        {
            int[] result = new int[2];
            
            switch (abilityName)
            {
                case "MoveAndAttack":
                    result[0] = -1;
                    result[1] = -1;
                    break;
                case "Overload":
                    result[0] = 2;
                    result[1] = 1;
                    break;
                case "Flare":
                    result[0] = 2;
                    result[1] = 1;
                    break;
            }

            return result;
        }
    }
}