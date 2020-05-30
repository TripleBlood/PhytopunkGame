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
            }
            return typeof(MoveAndAttackTargetingController);
        }

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
            }
            return "AbilityIcons/FlareAbIcon";
        }
    }
}