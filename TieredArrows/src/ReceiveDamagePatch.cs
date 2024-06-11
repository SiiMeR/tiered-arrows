using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace TieredArrows;

public class ReceiveDamagePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityAgent), "ReceiveDamage")]
    public static void EntityReceivedDamage(EntityAgent __instance, DamageSource damageSource, float damage)
    {
        if (damageSource?.SourceEntity is not EntityProjectile entityProjectile)
        {
            return;
        }

        if (entityProjectile.ProjectileStack.ItemAttributes == null)
        {
            return;
        }
        
        var tier = entityProjectile.ProjectileStack.ItemAttributes?["tier"]?.AsInt(0) ?? 0;
        damageSource.DamageTier = tier;
    }
}