using Terraria;
using Terraria.ModLoader;

namespace NewMod.Content.Projectiles
{
	public class ImperialShard : ModProjectile
	{
		// Iteration values: at most 6 tiles of travel in 0.4 seconds.
		public const float Speed = 4f;
		private const int Lifetime = 24;

		public override void SetDefaults() {
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = Lifetime;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI() {
			Projectile.ai[0]++;
			Projectile.Opacity = 1f - Projectile.ai[0] / Lifetime;
			Projectile.rotation = Projectile.velocity.ToRotation();
			// Horizontal shard centered on a single 32x32 canvas.
			DrawOffsetX = -(32 / 2 - Projectile.width / 2);
			DrawOriginOffsetY = -(32 / 2 - Projectile.height / 2);
		}
	}
}
