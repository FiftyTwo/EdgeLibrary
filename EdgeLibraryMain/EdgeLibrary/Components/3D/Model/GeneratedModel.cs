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
    /// <summary>
    /// Holds a Model and draws it
    /// </summary>
    public class GeneratedModel : SpriteModel
    {
        public Model Model { get { return _model; } set { _model = value; ReloadModel(); } }
        private Model _model;

        private Matrix[] transforms;

        public GeneratedModel(string modelName) : this(EdgeGame.GetModel(modelName)) { }
        public GeneratedModel(Model model)
            : base()
        {
            Model = model;
        }

        private void ReloadModel()
        {
            transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * World;
                    effect.View = EdgeGame.Camera3D.View;
                    effect.Projection = EdgeGame.Camera3D.Projection;
                }

                mesh.Draw();
            }
        }

        public override object Clone()
        {
            SpriteModel model = (SpriteModel)MemberwiseClone();
            //Clone here
            return model;
        }
    }
}
