using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace TieredArrows;

public class ItemTieredArrow : Item
{
    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        if (inSlot.Itemstack.Collectible.Attributes == null) return;

        float dmg = inSlot.Itemstack.Collectible.Attributes["damage"].AsFloat(0);
        if (dmg != 0) dsc.AppendLine(Lang.Get("arrow-piercingdamage", (dmg > 0 ? "+" : "") + dmg));

        dsc.AppendLine($"Attack tier: {inSlot.Itemstack.ItemAttributes?["tier"]?.AsInt(0) ?? 0}");
        
        float breakChanceOnImpact = inSlot.Itemstack.Collectible.Attributes["breakChanceOnImpact"].AsFloat(0.5f);
        dsc.AppendLine(Lang.Get("breakchanceonimpact", (int)(breakChanceOnImpact * 100)));
    }
}