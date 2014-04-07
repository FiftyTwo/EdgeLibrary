﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EdgeLibrary
{
    //Rotates a sprite towards another one
    public class ARotate : Action
    {
        public Sprite Target;
        //The higher this is, the faster the sprite will rotate
        public float Speed;

        //The number of degrees to add to the sprite's rotation after rotating towards the target sprite
        public float AdditionalAngle;

        public ARotate(Sprite target) : this(target, 0) { }
        public ARotate(Sprite target, float additionalAngle) : this(target, 360, additionalAngle) {}
        public ARotate(Sprite target, float speed, float additionalAngle)
        {
            Target = target;
            Speed = speed;
            AdditionalAngle = additionalAngle;
        }

        //Calculates rotation
        protected override void UpdateAction(GameTime gameTime, Sprite sprite)
        {
            float targetRotation = MathHelper.ToDegrees((float)Math.Atan2(Target.Position.Y - sprite.Position.Y, Target.Position.X - sprite.Position.X)) + AdditionalAngle;

            //Adds the speed to the sprite's rotation if it won't be more/less than the target rotation
            if (sprite.Rotation < targetRotation)
            {
                if (sprite.Rotation + Math.Abs(Speed) > targetRotation)
                {
                    sprite.Rotation = targetRotation;
                }
                else
                {
                    sprite.Rotation += Math.Abs(Speed);
                }
            }
            else if (sprite.Rotation > targetRotation)
            {
                if (sprite.Rotation - Math.Abs(Speed) < targetRotation)
                {
                    sprite.Rotation = targetRotation;
                }
                else
                {
                    sprite.Rotation -= Math.Abs(Speed);
                }
            }
        }

        public override Action Clone()
        {
            return new ARotate(Target, Speed, AdditionalAngle);
        }
    }
}