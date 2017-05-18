﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using Spelkonstruktionsprojekt.ZEngine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZEngine.Wrappers;
using Game.Services;

namespace Game.Menu.States
{
    class GameOver : IMenu
    {
        private GameManager gameManager;
        private GraphicsDevice gd = GameDependencies.Instance.GraphicsDeviceManager.GraphicsDevice;
        private readonly ControlsConfig controls;

        public GameOver(GameManager gameManager)
        {
            controls = new ControlsConfig(gameManager);
            this.gameManager = gameManager;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gd.Clear(Color.Black);

            //spriteBatch.Begin();
            //spriteBatch.Draw(gameManager.MenuContent.GameOver, new Rectangle(0, 0, 1800, 1500), Color.White);
            //spriteBatch.End();

            //var GameScoreList = ComponentManager.Instance.GetEntitiesWithComponent(typeof(GameScoreComponent));
            //if (GameScoreList.Count <= 0) return;
            //var GameScore = (GameScoreComponent)GameScoreList.First().Value;

            //string yourScore = "Total score: " + GameScore.TotalGameScore;
            //string exit = "(Press ESCAPE to exit)";

            //spriteBatch.Begin();
            //spriteBatch.DrawString(gameManager.MenuContent.MenuFont, yourScore, new Vector2(50, 100), Color.Red);
            //spriteBatch.DrawString(gameManager.MenuContent.MenuFont, exit, new Vector2(50, 200), Color.Red);
            //spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(gameManager.MenuContent.GameOver, new Rectangle(0, 0, 1900, 1100), Color.White);
            spriteBatch.End();

            string totalScoreText = "Total score: ";
            string exitText = "(ESC -> main menu)";

            spriteBatch.Begin();
            spriteBatch.DrawString(gameManager.MenuContent.MenuFont, totalScoreText, new Vector2(50, 40), Color.Red);
            spriteBatch.DrawString(gameManager.MenuContent.MenuFont, exitText, new Vector2(30, 90), Color.Red);
            spriteBatch.End();

            var GameScoreList = ComponentManager.Instance.GetEntitiesWithComponent(typeof(GameScoreComponent));
            if (GameScoreList.Count <= 0) return;
            var GameScore = (GameScoreComponent)GameScoreList.First().Value;

            string totalScore = GameScore.TotalGameScore.ToString();

            spriteBatch.Begin();
            spriteBatch.DrawString(gameManager.MenuContent.MenuFont, totalScore, new Vector2(380, 40), Color.Red);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                gameManager.CurrentGameState = GameManager.GameState.MainMenu;

        }
    }
}