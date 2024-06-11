using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace TieredArrows
{
    public class TieredArrowsModSystem : ModSystem
    {
        public override void StartClientSide(ICoreClientAPI api)
        {
            var harmony = new Harmony("tieredarrows");

            var original = AccessTools.Method(typeof(ItemArrow), nameof(ItemArrow.GetHeldItemInfo));
            var prefix = AccessTools.Method(typeof(ArrowGetHeldItemInfoPatch), nameof(ArrowGetHeldItemInfoPatch.HeldItemInfoPatch));

            harmony.Patch(original, new HarmonyMethod(prefix));
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            var harmony = new Harmony("tieredarrows");

            var original = AccessTools.Method(typeof(EntityAgent), nameof(EntityAgent.ReceiveDamage));
            var prefix = AccessTools.Method(typeof(ReceiveDamagePatch), nameof(ReceiveDamagePatch.EntityReceivedDamage));

            harmony.Patch(original, new HarmonyMethod(prefix));
        }
    }
}