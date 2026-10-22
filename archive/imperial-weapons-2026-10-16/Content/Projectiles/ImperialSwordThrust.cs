using Microsoft.Xna.Framework;
using NewMod.Content.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NewMod.Content.Projectiles
{
	// ExampleShortswordProjectile's hand anchoring and line collision, with retraction.
	public class ImperialSwordThrust : ModProjectile
	{
		private const int Duration = 16;
		private const int ReleaseTick = 8;
		private const float Reach = 30f;
		private const float BladeTipLength = 18f;
		private const float ShardDamageFraction = 0.4f;
		private const int FireDuration = 60;

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.hide = true;
			Projectile.timeLeft = Duration + 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override bool ShouldUpdatePosition() => false;

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead || player.noItems || player.CCed ||
				player.HeldItem.type != ModContent.ItemType<ImperialSword>()) {
				Projectile.Kill();
				return;
			}
			float timer = ++Projectile.ai[0];
			if (timer >= Duration) {
				Projectile.Kill();
				return;
			}
			Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
			float extension = timer <= ReleaseTick ? timer / ReleaseTick : (Duration - timer) / ReleaseTick;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, false) + direction * Reach * extension;
			player.heldProj = Projectile.whoAmI;
			Projectile.spriteDirection = direction.X >= 0 ? 1 : -1;
			Projectile.rotation = direction.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
			DrawOffsetX = -(40 / 2 - Projectile.width / 2);
			DrawOriginOffsetY = -(40 / 2 - Projectile.height / 2);
			Projectile.Opacity = Utils.GetLerpValue(0, 2, timer, true) * Utils.GetLerpValue(Duration, Duration - 3, timer, true);

			// Synced emission flag plus owner-only spawning prevents duplicate shards.
			if (timer >= ReleaseTick && Projectile.ai[1] == 0 && Projectile.owner == Main.myPlayer) {
				Projectile.ai[1] = 1;
				Projectile.netUpdate = true;
				// Start at the hand rather than beyond a wall at the sword tip.
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.MountedCenter,
					direction * ImperialShard.Speed, ModContent.ProjectileType<ImperialShard>(),
					System.Math.Max(1, (int)(Projectile.damage * ShardDamageFraction)),
					Projectile.knockBack * ShardDamageFraction, Projectile.owner);
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 tip = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * BladeTipLength;
			float point = 0;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
				Projectile.Center, tip, 10f * Projectile.scale, ref point);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.OnFire, FireDuration);
		}
	}
}
