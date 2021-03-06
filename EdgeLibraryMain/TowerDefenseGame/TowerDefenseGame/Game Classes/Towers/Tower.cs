﻿using EdgeLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowerDefenseGame
{
    public class Tower : Sprite
    {
        public TowerData TowerData;
        public AttackTarget AttackTarget;
        public Enemy Target;
        public Ticker ShootTicker;
        public Color TowerColor;
        private bool canShoot;
        public bool ShowRadius = false;
        public bool ShowTarget = false;
        public List<Projectile> ProjectilesToAdd;
        public List<Projectile> Projectiles;
        private List<Projectile> projectilesToRemove;
        private Sprite targetIcon = new Sprite("target", Vector2.Zero) { Visible = false };

        private Sprite towerRange;

        public Tower(TowerData data, Vector2 position)
            : base(data.Texture, position)
        {
            TowerData = data;
            ShootTicker = new Ticker(data.AttackSpeed);
            ShootTicker.OnTick += ShootTicker_OnTick;
            ShootTicker.Started = true;
            canShoot = false;
            Projectiles = new List<Projectile>();
            ProjectilesToAdd = new List<Projectile>();
            towerRange = new Sprite("Circle", Position);
            towerRange.Scale = new Vector2(TowerData.Range / 500f);
            Scale = TowerData.Scale;
            TowerColor = RandomTools.RandomColor();
        }

        void ShootTicker_OnTick(GameTime gameTime)
        {
            canShoot = true;
        }

        public void UpdateTower(List<Enemy> Enemies)
        {
            Target = SelectTarget(Enemies);

            if (TowerData.SpecialActionsOnUpdate != null)
            {
                TowerData.SpecialActionsOnUpdate(this, Enemies);
            }

            if (Target != null && TowerData.TracksEnemies)
            {
                Rotation = -1f * (float)Math.Atan2(Position.X - Target.Position.X, Position.Y - Target.Position.Y) + TowerData.BaseRotation;
            }

            if (canShoot)
            {
                if (Target != null && TowerData.EmitsProjectile)
                {
                    Projectile projectile = new Projectile(TowerData.AttackData, TowerData.AttackDamage, Target, TowerData.Accuracy, Position);
                    if (projectile.ProjectileData.SpecialActionsOnCreate != null)
                    {
                        projectile.ProjectileData.SpecialActionsOnCreate(projectile, this);
                    }

                    projectile.Rotation = Rotation + projectile.ProjectileData.BaseRotation;
                    Projectiles.Add(projectile);

                    projectile.TargetPosition.Normalize();
                }

                if (Enemies.Count > 0 && TowerData.SpecialActionsOnShoot != null)
                {
                    TowerData.SpecialActionsOnShoot(this, Enemies, Target);
                }

                canShoot = false;
                ShootTicker.elapsedMilliseconds = 0;
            }

            foreach (Projectile projectile in Projectiles)
            {
                projectile.UpdateProjectile(Enemies, this);
            }
        }

        public override void UpdateObject(GameTime gameTime)
        {
            if (ShowTarget && Target != null)
            {
                Target.BeingTargeted = true;
                Target.TargetTower = this;
                Color = TowerColor;
            }
            else
            {
                Color = Color.White;
            }

            ShootTicker.Update(gameTime);
            projectilesToRemove = new List<Projectile>();
            towerRange.Visible = ShowRadius;

            Projectiles.AddRange(ProjectilesToAdd);
            foreach (Projectile projectile in ProjectilesToAdd)
            {
                if (projectile.ProjectileData.SpecialActionsOnCreate != null)
                {
                    projectile.ProjectileData.SpecialActionsOnCreate(projectile, this);
                }
            }
            ProjectilesToAdd.Clear();

            foreach (Projectile projectile in Projectiles)
            {
                projectile.Update(gameTime);
                if (projectile.ShouldBeRemoved)
                {
                    projectilesToRemove.Add(projectile);
                }
            }
            foreach (Projectile projectile in projectilesToRemove)
            {
                Projectiles.Remove(projectile);
            }

            base.UpdateObject(gameTime);
        }

        public override void DrawObject(GameTime gameTime)
        {
            towerRange.Draw(gameTime);

            foreach (Projectile projectile in Projectiles)
            {
                projectile.Draw(gameTime);
            }
            base.DrawObject(gameTime);
            targetIcon.Draw(gameTime);
        }

        private Enemy SelectTarget(List<Enemy> Enemies)
        {
            List<Enemy> EnemiesInRange = new List<Enemy>();
            foreach (Enemy enemy in Enemies)
            {
                if (Vector2.DistanceSquared(enemy.Position, Position) <= (TowerData.Range * TowerData.Range))
                {
                    EnemiesInRange.Add(enemy);
                }
            }

            if (EnemiesInRange.Count == 0) { return null; }

            switch (AttackTarget)
            {
                case AttackTarget.First:
                    EnemiesInRange = EnemiesInRange.OrderByDescending(x => x.TrackDistance).ToList();
                    if (TowerData.SpecialActionsOnSelectTarget != null)
                    {
                        TowerData.SpecialActionsOnSelectTarget(this, Enemies, EnemiesInRange[0]);
                    }
                    return EnemiesInRange[0];
                    break;
                case AttackTarget.Last:
                    EnemiesInRange = EnemiesInRange.OrderBy(x => x.TrackDistance).ToList();
                    if (TowerData.SpecialActionsOnSelectTarget != null)
                    {
                        TowerData.SpecialActionsOnSelectTarget(this, Enemies, EnemiesInRange[0]);
                    }
                    return EnemiesInRange[0];
                    break;
                case AttackTarget.Strong:
                    EnemiesInRange = EnemiesInRange.OrderByDescending(x => x.Health).ThenByDescending(x => x.EnemyData.Armor).ToList();
                    if (TowerData.SpecialActionsOnSelectTarget != null)
                    {
                        TowerData.SpecialActionsOnSelectTarget(this, Enemies, EnemiesInRange[0]);
                    }
                    return EnemiesInRange[0];
                    break;
                case AttackTarget.Weak:
                    EnemiesInRange = EnemiesInRange.OrderBy(x => x.Health).ThenBy(x => x.EnemyData.Armor).ToList();
                    if (TowerData.SpecialActionsOnSelectTarget != null)
                    {
                        TowerData.SpecialActionsOnSelectTarget(this, Enemies, EnemiesInRange[0]);
                    }
                    return EnemiesInRange[0];
                    break;
            }
            return null;
        }
    }

    public enum AttackTarget
    {
        First,
        Strong,
        Last,
        Weak
    }

    [Flags]
    public enum PlaceableArea
    {
        Land = 1,
        Water = 2
    }

    public struct TowerData
    {
        public float AttackDamage;
        public float AttackSpeed;
        public float Range;

        //0 - Perfect accuracy
        //Any number higher than 0 is the spread (in pixels) at 100 distance from the tower
        public float Accuracy;
        public int Cost;
        public PlaceableArea PlaceableArea;
        public ProjectileData AttackData;
        public string Name;
        public string BaseName;

        public System.Action<Tower, List<Enemy>, Enemy> SpecialActionsOnSelectTarget;
        public System.Action<Tower, List<Enemy>, Enemy> SpecialActionsOnShoot;
        public System.Action<Tower, List<Enemy>> SpecialActionsOnUpdate;
        public System.Action<Tower> SpecialActionsOnCreate;
        public System.Action<Tower> SpecialActionsOnSell;

        public string Texture;
        public Vector2 Scale;
        public float BaseRotation;
        public bool EmitsProjectile;
        public bool TracksEnemies;

        public TowerData(string name, float attackDamage, float attackSpeed, float range, float accuracy, ProjectileData attackData, string texture, float baseRotation, Vector2 scale, int cost, PlaceableArea placeableArea, string baseName, bool tracksEnemies = true, System.Action<Tower, List<Enemy>, Enemy> specialActionsOnSelectTarget = null, System.Action<Tower> specialActionsOnCreate = null, System.Action<Tower, List<Enemy>> specialActionsOnUpdate = null, System.Action<Tower, List<Enemy>, Enemy> specialActionsOnShoot = null, System.Action<Tower> specialActionsOnSell = null, bool emitsProjectile = true)
        {
            AttackDamage = attackDamage;
            AttackSpeed = attackSpeed;
            Range = range;
            AttackData = attackData;
            Texture = texture;
            Scale = scale;
            Accuracy = accuracy;
            Cost = cost;
            PlaceableArea = placeableArea;
            Name = name;
            BaseName = baseName;
            BaseRotation = baseRotation;
            TracksEnemies = tracksEnemies;
            SpecialActionsOnSelectTarget = specialActionsOnSelectTarget;
            SpecialActionsOnCreate = specialActionsOnCreate;
            SpecialActionsOnSell = specialActionsOnSell;
            SpecialActionsOnShoot = specialActionsOnShoot;
            SpecialActionsOnUpdate = specialActionsOnUpdate;
            EmitsProjectile = emitsProjectile;
        }
    }
}
