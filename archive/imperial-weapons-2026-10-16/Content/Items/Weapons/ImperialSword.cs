using NewMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NewMod.Content.Items.Weapons
{
	public class ImperialSword : ModItem
	{
		private const int Damage = 50;
		private const int UseTime = 20;

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;

			Item.useStyle = ItemUseStyleID.Rapier;
			Item.useTime = UseTime;
			Item.useAnimation = UseTime;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<ImperialSwordThrust>();
			Item.shootSpeed = 2.1f;
			Item.damage = Damage;
			Item.knockBack = 6f;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override bool MeleePrefix() => true;

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock, 1)
				.Register();
		}
	}
}
