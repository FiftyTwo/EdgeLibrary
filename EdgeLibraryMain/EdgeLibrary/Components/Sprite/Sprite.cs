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
using FarseerPhysics.Dynamics;
using FarseerPhysics;

namespace EdgeLibrary
{
    public class Sprite : DrawableGameComponent, ICloneable
    {
        //Used to identify the sprite
        public string ID;

        //Gets the texture size of the sprite
        public virtual float Width { get { return Texture == null ? 0 : Texture.Width; } }
        public virtual float Height { get { return Texture == null ? 0 : Texture.Height; } }

        //If the Sprite is supposed to be removed from the game, this will be set to true
        public bool ShouldBeRemoved { get; protected set; }

        //Used for physics
        public Body Body { get; protected set; }
        public bool PhysicsEnabled;

        //Sets the texture through a string
        public string TextureName { set { Texture = EdgeGame.GetTexture(value); reloadBoundingBox(); } }

        //Sets the scale with a Vector2
        public virtual Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = new Vector2(
                    (value.X < 0 ? 0 : value.X),
                    (value.Y < 0 ? 0 : value.Y));
                reloadBoundingBox();
            }
        }
        private Vector2 _scale;

        //Optional visual effects
        public virtual Color Color { get; set; }
        //Measured in radians
        public virtual float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                if (PhysicsEnabled && Body != null)
                {
                    Body.Rotation = _rotation;
                }
            }
        }
        private float _rotation;

        public virtual SpriteEffects SpriteEffects { get; set; }

        //Used to store data in a sprite
        public Dictionary<string, string> Data;

        //Gets the origin point of the sprite, which is either (0, 0) or half of the texture size
        public virtual Vector2 OriginPoint { get; protected set; }

        //If set to true, origin will be top left; if not, origin will be center
        public virtual bool CenterAsOrigin { get { return _centerAsOrigin; } set { _centerAsOrigin = value; reloadOriginPoint(); } }
        private bool _centerAsOrigin;

        //The texture for drawing
        public virtual Texture2D Texture { get { return _texture; } set { _texture = value; reloadOriginPoint(); } }
        private Texture2D _texture;

        //Used for OnCollideStart
        protected List<string> currentlyCollidingWithIDs;

        //Used for camera tracking
        public bool FollowsCamera;
        public bool ScaleWithCamera;

        //Used to change properties of the sprite - could be uesd for moving, color changing, etc.
        protected Dictionary<string, Action> Actions;
        protected List<string> actionsToRemove;

        //For buttons, etc.
        public Rectangle BoundingBox { get; protected set; }

        //Location of the sprite
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                if (PhysicsEnabled && Body != null)
                {
                    Body.Position = ConvertUnits.ToSimUnits(_position);
                }
                reloadBoundingBox();
            }
        }
        private Vector2 _position;

        //Which blend state to use when drawing the sprite
        public BlendState BlendState = BlendState.AlphaBlend;

        //Which sampler state to use when drawing the sprite
        public SamplerState SamplerState = SamplerState.LinearClamp;

        public delegate void SpriteEvent(Sprite sprite, GameTime gameTime);
        public event SpriteEvent OnAdded;
        public event SpriteEvent OnRemoved;

        public Sprite(string textureName, Vector2 position)
            : base(EdgeGame.Game)
        {
            ID = this.GenerateID();

            Position = position;

            Data = new Dictionary<string, string>();

            PhysicsEnabled = false;

            ShouldBeRemoved = false;

            FollowsCamera = true;
            ScaleWithCamera = true;

            //Sets the default visual effects
            Scale = Vector2.One;
            Color = Color.White;
            Rotation = 0f;
            SpriteEffects = SpriteEffects.None;

            _centerAsOrigin = true;

            Actions = new Dictionary<string, Action>();
            actionsToRemove = new List<string>();

            currentlyCollidingWithIDs = new List<string>();

            if (textureName != null)
            {
                Texture = EdgeGame.GetTexture(textureName);
                reloadBoundingBox();
            }
            reloadOriginPoint();
        }
        public Sprite(string textureName, Vector2 position, Color color, Vector2 scale, float rotation = 0f, SpriteEffects effects = SpriteEffects.None)
            : this(textureName, position)
        {
            Color = color;
            Rotation = rotation;
            Scale = scale;
            SpriteEffects = effects;
        }

        //Adds the sprite to the game
        public virtual void AddToGame()
        {
            EdgeGame.Game.Components.Add(this);
            if (OnAdded != null)
            {
                OnAdded(this, EdgeGame.GameTime);
            }
        }

        //Removes the sprite from the game
        public virtual void RemoveFromGame()
        {
            EdgeGame.Game.Components.Remove(this);
            Disable();
        }

        public virtual void Disable()
        {
            PhysicsEnabled = false;
            if (EdgeGame.World.BodyList.Contains(Body))
            {
                EdgeGame.World.RemoveBody(Body);
            }
            ShouldBeRemoved = true;
            if (OnRemoved != null)
            {
                OnRemoved(this, EdgeGame.GameTime);
            }
        }

        //Reloads origin point based on texture
        protected virtual void reloadOriginPoint()
        {
            if (Texture != null)
            {
                if (CenterAsOrigin)
                {
                    OriginPoint = new Vector2(Width / 2f, Height / 2f);
                }
                else
                {
                    OriginPoint = Vector2.Zero;
                }
            }
        }

        protected virtual void reloadBoundingBox()
        {
            BoundingBox = new Rectangle((int)Position.X - (int)(Width * Scale.X) / 2, (int)Position.Y - (int)(Height * Scale.Y) / 2, (int)(Width * Scale.X), (int)(Height * Scale.Y));
        }

        //Gets an action
        public T Action<T>(string id) where T : Action
        {
            return (T)Actions[id];
        }

        //Adds an action
        public void AddAction(string id, Action action)
        {
            //Makes a Clone so two sprites don't have the same action
            Action Clone = action.Clone();
            Actions.Add(id, Clone);
        }
        public void AddAction(Action action)
        {
            AddAction(action.GenerateID(), action);
        }

        //Removes an action
        public bool RemoveAction(string id)
        {
            return Actions.Remove(id);
        }

        public void EnablePhysics(Body body)
        {
            PhysicsEnabled = true;
            Body = body;
            Body.Position = ConvertUnits.ToSimUnits(Position);
        }

        public void DisablePhysics()
        {
            PhysicsEnabled = false;
        }

        //Updates the sprite - this should be overwritten
        public virtual void UpdateObject(GameTime gameTime)
        {
            if (PhysicsEnabled && Body != null)
            {
                _position = ConvertUnits.ToDisplayUnits(Body.Position);
                _rotation = Body.Rotation;
            }

            //Updates the actions
            actionsToRemove.Clear();
            for (int i = 0; i < Actions.Count; i++ )
            {
                Actions.Values.ToList()[i].Update(gameTime, this);

                if (Actions.Values.ToList()[i].toRemove)
                {
                    actionsToRemove.Add(Actions.Keys.ToList()[i]);
                }
            }
            foreach (string key in actionsToRemove)
            {
                Actions.Remove(key);
            }
        }

        //Override of DrawableGameComponent - should not be overwritten
        public override sealed void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                UpdateObject(gameTime);
            }
        }

        //Override of DrawableGameComponent - should not be overwritten
        public override sealed void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                DrawObject(gameTime);
            }
        }

        //Draws to the spritebatch - this should be overwritten
        public virtual void DrawObject(GameTime gameTime)
        {
            RestartSpriteBatch();

            EdgeGame.Game.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, OriginPoint, !ScaleWithCamera ? Scale / EdgeGame.Camera.Scale : Scale, SpriteEffects, 0);

            RestartSpriteBatch();
        }

        //Restarts the spritebatch if the blend state is not AlphaBlend
        //Should be called before and after drawing
        protected void RestartSpriteBatch()
        {
            if (BlendState != BlendState.AlphaBlend || SamplerState != SamplerState.LinearClamp || !FollowsCamera)
            {
                EdgeGame.Game.SpriteBatch.End();
                if (FollowsCamera)
                {
                    EdgeGame.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, EdgeGame.Camera.GetTransform());
                }
                else
                {
                    EdgeGame.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                }
            }

            if (!EdgeGame.Game.SpriteBatch.IsStarted)
            {
                EdgeGame.Game.SpriteBatch.Begin();
            }
        }

        //Draws the area of the collision body
        public virtual void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            //Not currently used
        }

        //Creates a copy of the sprite
        public virtual object Clone()
        {
            Sprite clone = (Sprite)MemberwiseClone();
            if (OnAdded != null)
            {
                clone.OnAdded = (SpriteEvent)OnAdded.Clone();
            }
            if (OnRemoved != null)
            {
                clone.OnRemoved = (SpriteEvent)OnRemoved.Clone();
            }

            if (PhysicsEnabled)
            {
                clone.Body = Body.DeepClone();
            }

            clone.Actions = new Dictionary<string, Action>();
            foreach (var action in Actions)
            {
                clone.AddAction(action.Key, action.Value);
            }
            return clone;
        }
    }
}
