using System.Text;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace TieredArrows;

public class ArrowGetHeldItemInfoPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemArrow), "GetHeldItemInfo")]
    public static void HeldItemInfoPatch(ItemArrow __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world,
        bool withDebugInfo)
    {
        dsc.AppendLine($"Attack tier: {inSlot.Itemstack.ItemAttributes?["tier"]?.AsInt(0) ?? 0}");
    }
}