using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EdgeLibrary;
using EdgeLibrary.Platform;

namespace EdgeDemo
{
    public class EdgeDemo : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ParticleEmitter MasterEmitter;

        float speed = 10;

        protected override void LoadContent()
        {
            ResourceManager.LoadTexturesInSpritesheet("SpaceSheet.xml", "SpaceSheet");
            ResourceManager.LoadTexturesInSpritesheet("ParticleSheet.xml", "ParticleSheet");
            ResourceManager.LoadFont("SmallFont");
            ResourceManager.LoadFont("MediumFont");
            ResourceManager.LoadFont("LargeFont");
        }

        public void initEdgeGame()
        {
            EdgeGame.GameDrawState = GameDrawState.Normal;
            IsMouseVisible = true;

            EdgeGame.WindowSize = new Vector2(1300, 700);

            EdgeGame.ClearColor = new Color(20, 20, 20);
            TextSprite Header = new TextSprite("LargeFont", "EdgeDemo", new Vector2(EdgeGame.WindowSize.X/2, 50), Color.White);

            Vector2[] buttonPositions = new Vector2[1] { new Vector2(200, 150) };

            LabelButton PlatformButton = new LabelButton("MediumFont", "Platform Demo", buttonPositions[0], Color.OrangeRed);
            PlatformButton.OffStyle.Color = Color.Orange;
            PlatformButton.MouseOverStyle.Color = Color.OrangeRed;
            PlatformButton.DrawLayer = -1;
            PlatformButton.MouseOver += new Button.ButtonEventHandler(platform_mouseOver);
            PlatformButton.MouseOff += new Button.ButtonEventHandler(buttonMouseOff);
            PlatformButton.Click += new Button.ButtonEventHandler(platform_click);

            emitter_toNormal();
        }

        private void emitter_toNormal()
        {
            MasterEmitter = new ParticleEmitter("fire", EdgeGame.WindowSize/2);
            MasterEmitter.SetSize(new Vector2(15), new Vector2(100));
            MasterEmitter.SetRotationSpeed(-8, 8);
            MasterEmitter.BlendState = BlendState.Additive;
            MasterEmitter.SetLife(3000);
            ColorChangeIndex index = new ColorChangeIndex(700, Color.Blue, Color.Green, Color.Transparent);
            MasterEmitter.SetColor(index);
        }

        private void emitter_toPlatform()
        {
            MasterEmitter = new ParticleEmitter("fire", EdgeGame.WindowSize / 2);
            MasterEmitter.SetSize(new Vector2(15), new Vector2(100));
            MasterEmitter.SetRotationSpeed(-8, 8);
            MasterEmitter.BlendState = BlendState.Additive;
            MasterEmitter.SetLife(3000);
            ColorChangeIndex index = new ColorChangeIndex(700, Color.Crimson, Color.OrangeRed, Color.Transparent);
            MasterEmitter.SetColor(index);
        }

        private void platform_click(ButtonEventArgs e) { EdgeGame.AddScene(new PlatformDemo()); EdgeGame.SwitchScene("PlatformDemo"); }
        private void platform_mouseOver(ButtonEventArgs e) { emitter_toPlatform(); } 
        private void buttonMouseOff(ButtonEventArgs e) { emitter_toNormal(); } 

        #region UNUSED

        public EdgeDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            EdgeGame.Init(Content, GraphicsDevice, graphics, spriteBatch);
            base.Initialize();
            initEdgeGame();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            EdgeGame.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            EdgeGame.Draw(gameTime);
        }
        #endregion
    }
}
