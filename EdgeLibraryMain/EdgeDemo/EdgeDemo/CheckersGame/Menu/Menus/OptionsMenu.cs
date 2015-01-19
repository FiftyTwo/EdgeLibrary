﻿using EdgeLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeDemo.CheckersService;

namespace EdgeDemo.CheckersGame
{
    public class OptionsMenu : MenuBase
    {
        public OptionsMenu()
            : base("OptionsMenu")
        {
                CheckersServiceClient ServiceClient = new CheckersServiceClient();

            TextSprite title = new TextSprite(Config.MenuTitleFont, "Options Menu", new Vector2(EdgeGame.WindowSize.X / 2, EdgeGame.WindowSize.Y * 0.05f)) { Color = Config.MenuTextColor };
            Components.Add(title);

            TextSprite subTitle = new TextSprite(Config.MenuSubtitleFont, "Click!", new Vector2(EdgeGame.WindowSize.X / 2, EdgeGame.WindowSize.Y * 0.1f)) { Color = Config.MenuTextColor };
            Components.Add(subTitle);

            Button quitButton = new Button("grey_button00", new Microsoft.Xna.Framework.Vector2(EdgeGame.WindowSize.X / 2, EdgeGame.WindowSize.Y * 0.7f)) { Color = Config.MenuButtonColor, Scale = new Vector2(1) };
            quitButton.Style.NormalTexture = EdgeGame.GetTexture("grey_button00");
            quitButton.Style.MouseOverTexture = EdgeGame.GetTexture("grey_button02");
            quitButton.Style.ClickTexture = EdgeGame.GetTexture("grey_button01");
            quitButton.Style.AllColors = Config.MenuButtonColor;
            quitButton.OnRelease += (x, y) => {
                if (Config.ThisGameType == Config.GameType.Online)
                {
                    ServiceClient.Disconnect(Config.ThisGameID, Config.IsHost);
                }
                if (System.Windows.Forms.MessageBox.Show("Are you sure you want to leave this game?", "Leave?", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK) 
                { 
                    Config.ThisGameType = Config.GameType.Singleplayer;
                    MenuManager.SwitchMenu("MainMenu");
                }
            };
            Components.Add(quitButton);

            TextSprite quitButtonText = new TextSprite(Config.MenuButtonTextFont, "Main Menu", quitButton.Position);
            Components.Add(quitButtonText);

            ButtonToggle musicButton = new ButtonToggle("grey_boxCheckmark", new Microsoft.Xna.Framework.Vector2(EdgeGame.WindowSize.X / 2, EdgeGame.WindowSize.Y * 0.5f)) {  Color = Config.MenuButtonColor, Scale = new Vector2(1) };
            musicButton.OnStyle.AllTextures = EdgeGame.GetTexture("grey_boxCheckmark");
            musicButton.OnStyle.AllColors = Config.MenuButtonColor;
            musicButton.OffStyle.AllTextures = EdgeGame.GetTexture("grey_box");
            musicButton.OffStyle.AllColors = Config.MenuButtonColor;
            musicButton.On = true;
            Components.Add(musicButton);

            Input.OnKeyRelease += Input_OnKeyRelease;
        }

        void Input_OnKeyRelease(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (MenuManager.SelectedMenu == this && key == Config.BackKey)
            {
                MenuManager.SwitchMenu(MenuManager.PreviousMenu.Name);
            }
        }
    }
}
