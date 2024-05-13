using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace TieredArrows;

public class ItemTieredBow : ItemBow
{
    private string aimAnimation;

    public override void OnLoaded(ICoreAPI api)
    {
        aimAnimation = Attributes["aimAnimation"].AsString();
    }

    public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity,
        BlockSelection blockSel, EntitySelection entitySel)
    {
        if (byEntity.Attributes.GetInt("aimingCancel") == 1) return;
        byEntity.Attributes.SetInt("aiming", 0);
        byEntity.AnimManager.StopAnimation(aimAnimation);

        if (byEntity.World is IClientWorldAccessor)
        {
            slot.Itemstack.TempAttributes.RemoveAttribute("renderVariant");
        }

        slot.Itemstack.Attributes.SetInt("renderVariant", 0);
        (byEntity as EntityPlayer)?.Player?.InventoryManager.BroadcastHotbarSlot();

        if (secondsUsed < 0.65f) return;

        ItemSlot arrowSlot = GetNextArrow(byEntity);
        if (arrowSlot == null) return;

        float damage = 0;
        float accuracyBonus = 0f;

        // Bow damage
        if (slot.Itemstack.Collectible.Attributes != null)
        {
            damage += slot.Itemstack.Collectible.Attributes["damage"].AsFloat(0);

            accuracyBonus = 1 - slot.Itemstack.Collectible.Attributes["accuracyBonus"].AsFloat(0);
        }

        // Arrow damage
        if (arrowSlot.Itemstack.Collectible.Attributes != null)
        {
            damage += arrowSlot.Itemstack.Collectible.Attributes["damage"].AsFloat(0);
        }

        ItemStack stack = arrowSlot.TakeOut(1);
        arrowSlot.MarkDirty();

        IPlayer byPlayer = null;
        if (byEntity is EntityPlayer) byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
        byEntity.World.PlaySoundAt(new AssetLocation("sounds/bow-release"), byEntity, byPlayer, false, 8);

        float breakChance = 0.5f;
        if (stack.ItemAttributes != null) breakChance = stack.ItemAttributes["breakChanceOnImpact"].AsFloat(0.5f);

        EntityProperties type =
            byEntity.World.GetEntityType(new AssetLocation("arrow-" + stack.Collectible.Variant["material"]));
        var entityarrow = byEntity.World.ClassRegistry.CreateEntity(type) as EntityTieredProjectile;
        entityarrow.FiredBy = byEntity;
        entityarrow.Damage = damage;
        entityarrow.ProjectileStack = stack;
        entityarrow.DropOnImpactChance = 1 - breakChance;
        entityarrow.DamageTier = stack.ItemAttributes?["tier"]?.AsInt(0) ?? 0;

        float acc = Math.Max(0.001f, (1 - byEntity.Attributes.GetFloat("aimingAccuracy", 0)));

        double rndpitch = byEntity.WatchedAttributes.GetDouble("aimingRandPitch", 1) * acc * (0.75 * accuracyBonus);
        double rndyaw = byEntity.WatchedAttributes.GetDouble("aimingRandYaw", 1) * acc * (0.75 * accuracyBonus);

        Vec3d pos = byEntity.ServerPos.XYZ.Add(0, byEntity.LocalEyePos.Y, 0);
        Vec3d aheadPos = pos.AheadCopy(1, byEntity.SidedPos.Pitch + rndpitch, byEntity.SidedPos.Yaw + rndyaw);
        Vec3d velocity = (aheadPos - pos) * byEntity.Stats.GetBlended("bowDrawingStrength");


        entityarrow.ServerPos.SetPos(byEntity.SidedPos.BehindCopy(0.21).XYZ.Add(0, byEntity.LocalEyePos.Y, 0));
        entityarrow.ServerPos.Motion.Set(velocity);
        entityarrow.Pos.SetFrom(entityarrow.ServerPos);
        entityarrow.World = byEntity.World;
        entityarrow.SetRotation();

        byEntity.World.SpawnEntity(entityarrow);

        slot.Itemstack.Collectible.DamageItem(byEntity.World, byEntity, slot);

        byEntity.AnimManager.StartAnimation("bowhit");
    }
}