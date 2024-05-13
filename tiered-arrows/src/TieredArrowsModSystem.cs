using Vintagestory.API.Common;

namespace TieredArrows
{
    public class TieredArrowsModSystem : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            
            api.RegisterItemClass("ItemTieredArrow", typeof(ItemTieredArrow));
            api.RegisterEntity("EntityTieredProjectile", typeof(EntityTieredProjectile));
            api.RegisterItemClass("ItemTieredBow", typeof(ItemTieredBow));
        }

    }
}