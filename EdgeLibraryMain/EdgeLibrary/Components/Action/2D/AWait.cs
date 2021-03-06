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
    public class AWait : Action
    {
        public float WaitTime;
        private float elapsedTime;

        public AWait(float waitTime) : base()
        {
            WaitTime = waitTime;
            elapsedTime = 0;
        }

        protected override void UpdateAction(GameTime gameTime, Sprite sprite)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds * EdgeGame.GameSpeed;

            if (elapsedTime >= WaitTime)
            {
                Stop(gameTime, sprite);
            }
        }

        public override void Reset()
        {
            elapsedTime = 0;
            base.Reset();
        }

        public override Action SubClone()
        {
            return new AWait(WaitTime);
        }
    }
}
