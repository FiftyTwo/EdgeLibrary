﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeLibrary;
using Microsoft.Xna.Framework;

namespace EdgeDemo.CheckersGame
{
    public class Square : Sprite
    {
        public bool FakeSquare;
        public Piece OccupyingPiece;

        public int X;
        public int Y;

        public bool hasPiece
        {
            get
            {
                return OccupyingPiece != null;
            }
        }

        public Square(string texture, Vector2 position, float size, Color color)
            : base(texture, position)
        {
            Scale = new Vector2(size, size);
            Color = color;
        }

        public void SetPiece(Piece piece)
        {
            piece.X = X;
            piece.Y = Y;
        }
    }
}