using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NewMod.Content.Items.Weapons
{
	public class ImperialSword : ModItem
	{
		private const int Damage = 50;
		private const int UseTime = 20;
		private const int OnFireDuration = 60;

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = UseTime;
			Item.useAnimation = UseTime;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = Damage;
			Item.knockBack = 6f;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) {
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GoldFlame);
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.OnFire, OnFireDuration);
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
