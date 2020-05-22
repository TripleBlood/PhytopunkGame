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
    }
}