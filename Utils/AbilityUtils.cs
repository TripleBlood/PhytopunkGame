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
                case "Flare":
                    return typeof(FlareTargetingController); 
                case "Adrenaline":
                    return typeof(AdrenalineTargetingController);
                case "Stimulator":
                    return typeof(StimulatorTargetingController);
                case "Cryoblast":
                    return typeof(CryoblastTargetingController);
                case "Sniper Shot":
                    return typeof(SniperShotTargetingController); 
                case "Splitshot":
                    return typeof(SplitshotTargetingController);
                case "Vulnerability Scan":
                    return typeof(VulnerabilityScanTargetingController); 
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
                case "Stimulator":
                    return "AbilityIcons/StimulatorAbIcon";
                case "Cryoblast":
                    return "AbilityIcons/CryoblastAbIcon";
                case "Sniper Shot":
                    return "AbilityIcons/SniperShotAbIcon";
                case "Splitshot":
                    return "AbilityIcons/SplitshotAbIcon";
                case "Vulnerability Scan":
                    return "AbilityIcons/VulnerabilityScanAbIcon";
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
                    result[1] = 2;
                    break;
                case "Adrenaline":
                    result[0] = -1;
                    result[1] = -1;
                    break;
                case "Stimulator":
                    result[0] = 2;
                    result[1] = -1;
                    break;
                case "Cryoblast":
                    result[0] = 2;
                    result[1] = 2;
                    break;
                case "Sniper Shot":
                    result[0] = 4;
                    result[1] = 0;
                    break;
                case "Splitshot":
                    result[0] = 4;
                    result[1] = 0;
                    break;
                case "Vulnerability Scan":
                    result[0] = 2;
                    result[1] = 3;
                    break;
            }
            return result;
        }
    }
}