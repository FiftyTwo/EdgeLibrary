﻿using EdgeLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowerDefenseGame
{
    public class ExplosionProjectile : Projectile
    {
        float ExplosionRadius;
        Texture2D ExplosionTexture;

        public ExplosionProjectile(ProjectileData data, string explosionTexture, Enemy target, float accuracy, Vector2 position, float explosionRadius)
            : base(data, target, accuracy, position)
        {
            ExplosionRadius = explosionRadius;
            ExplosionTexture = EdgeGame.GetTexture(explosionTexture);
        }

        public void Explode(List<Enemy> enemies, Tower tower)
        {
            Texture = ExplosionTexture;

            foreach (Enemy enemy in enemies)
            {
                if (CollisionDetection.CircleCircle(Position, ExplosionRadius, enemy.Position, enemy.EnemyData.CollisionRadius))
                {
                    enemy.Hit(ProjectileData.Damage, ProjectileData.ArmorPierce);

                    //Causes a crash: stack overflow, infinite loop
                    //if (ProjectileData.SpecialActionsOnHit != null)
                    //{
                    //    ProjectileData.SpecialActionsOnHit(this, enemies, enemy, tower);
                    //}
                }
            }
        }
    }
}