﻿using EdgeLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckersGame
{
    //A sprite which runs an 'AColorChange' action on demand
    public class ColorTextSprite : TextSprite
    {
        public ColorChangeIndex Colors;

        public ColorTextSprite(string fontName, Vector2 position, ColorChangeIndex colors) : base(fontName, "", position)
        {
            Colors = colors;
        }

        public override void UpdateObject(GameTime gameTime)
        {
            base.UpdateObject(gameTime);
        }

        public void Display(string text, ColorChangeIndex index = null)
        {
            Text = text;
            Colors.ResetTime();
            AddAction(new AColorChange(index == null ? Colors : index));
        }
    }
}
