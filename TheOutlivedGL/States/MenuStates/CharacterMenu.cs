﻿using Game.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using System.Linq;
using static Game.Menu.States.CharacterMenu.CharacterType;
using static Game.Services.VirtualGamePad.MenuKeys;
using static Game.Services.VirtualGamePad.MenuKeyStates;

namespace Game.Menu.States
{
    /// <summary>
    /// The character menu state is used for drawing the options to
    /// choose from different characters that are to be used in the 
    /// game by the player or players.
    /// </summary>
    public class CharacterMenu : IMenu, ILifecycle
    {
        public enum CharacterType
        {
            Bob,
            Edgar,
            Ward,
            Jimmy
        }

        private GameManager gameManager { get; }

        private int CurrentPlayerIndex;
        public GameConfig GameConfig { get; }
        private Viewport viewport;
        //private VirtualGamePad Player;  

        private CharacterType CurrentSelectedCharacter = Bob;
        private int CurrentSelectedCharacterIndex = 0;
        private CharacterType[] Characters;

        public CharacterMenu(GameManager gameManager)
        {
            this.gameManager = gameManager;
            GameConfig = gameManager.gameConfig;
            viewport = this.gameManager.viewport;
            //Player = gameManager.playerControllers.Controllers[0];
            Characters = new[] { Bob, Edgar, Ward, Jimmy };
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Begin();
            gameManager.effects.DrawExpandingEffect(sb, AssetManager.Instance.Get<Texture2D>("Images/Menu/background3"));


            DrawCharacterNames(sb, viewport);
            DrawSelectedOptionText(sb, viewport);
            sb.End();
        }

        private void DrawCharacterNames(SpriteBatch sb, Viewport viewport)
        {
            switch (CurrentSelectedCharacter)
            {
                case Bob:
                    sb.Draw(AssetManager.Instance.Get<Texture2D>("Images/Characters/character_hb"), viewport.Bounds, Color.White);
                    break;
                case Edgar:
                    sb.Draw(AssetManager.Instance.Get<Texture2D>("Images/Characters/character_he"), viewport.Bounds, Color.White);
                    break;
                case Ward:
                    sb.Draw(AssetManager.Instance.Get<Texture2D>("Images/Characters/character_hw"), viewport.Bounds, Color.White);
                    break;
                case Jimmy:
                    sb.Draw(AssetManager.Instance.Get<Texture2D>("Images/Characters/character_hj"), viewport.Bounds, Color.White);
                    break;
            }
        }

        private void DrawSelectedOptionText(SpriteBatch spriteBatch, Viewport viewport)
        {
            var message = $"Player {CurrentPlayerIndex + 1} choose your character!";

            spriteBatch.DrawString(AssetManager.Instance.Get<SpriteFont>("Fonts/ZMenufont"), message,
                new Vector2(viewport.Width * 0.1f, viewport.Height * 0.1f), Color.Black);
        }

        private void NextPlayerOrStartGame()
        {
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= GameConfig.Players.Count)
            {
                StartGame();
            }
            else
            {
               // Player = gameManager.playerControllers.Controllers[CurrentPlayerIndex];
            }
            ResetCharacterSelection();
        }

        private void PreviousPlayerOrGoBack()
        {
            CurrentPlayerIndex--;
            if (CurrentPlayerIndex < 0)
            {
                gameManager.MenuNavigator.GoBack();
            }
            else
            {
               // Player = gameManager.playerControllers.Controllers[CurrentPlayerIndex];
            }
            ResetCharacterSelection();
        }

        private void SelectNextCharacter()
        {
            CurrentSelectedCharacterIndex++;
            if (CurrentSelectedCharacterIndex >= Characters.Length)
            {
                CurrentSelectedCharacterIndex = 0;
            }
            CurrentSelectedCharacter = Characters[CurrentSelectedCharacterIndex];
        }

        private void SelectPreviousCharacter()
        {
            CurrentSelectedCharacterIndex--;
            if (CurrentSelectedCharacterIndex < 0)
            {
                CurrentSelectedCharacterIndex = Characters.Length - 1;
            }
            CurrentSelectedCharacter = Characters[CurrentSelectedCharacterIndex];
        }

        private void ResetCharacterSelection()
        {

            CurrentSelectedCharacterIndex = 0;
            CurrentSelectedCharacter = Characters[CurrentSelectedCharacterIndex];
        }

        private Player CurrentPlayer()
        {
            return GameConfig.Players[CurrentPlayerIndex];
        }

        private void StartGame()
        {
            if (MediaPlayer.State != MediaState.Stopped)
                MediaPlayer.Stop();

            gameManager.MenuNavigator.GoTo(GameConfig.GameMode);
        }

        public void Update(GameTime gameTime)
        {
            if (GameConfig.Players.Count == 0)
            {
                gameManager.MenuNavigator.GoBack();
            }
            else if (gameManager.playerControllers.Controllers.Any(c => c.Is(Cancel, Pressed)))
            {
                PreviousPlayerOrGoBack();
            }
            else if (gameManager.playerControllers.Controllers.Any(c => c.Is(Accept, Pressed)))
            {
                AssetManager.Instance.Get<SoundEffect>("sound/click2").Play();
                CurrentPlayer().CharacterType = CurrentSelectedCharacter;
                CurrentPlayer().SpriteName = GetCharacterSpriteName(CurrentSelectedCharacter);
                NextPlayerOrStartGame();
            }
            else if (gameManager.playerControllers.Controllers.Any(c => c.Is(Right, Pressed)))
            {
                SelectNextCharacter();
            }
            else if (gameManager.playerControllers.Controllers.Any(c => c.Is(Left, Pressed)))
            {
                SelectPreviousCharacter();
            }
        }

        private string GetCharacterSpriteName(CharacterType choice)
        {
            switch (choice)
            {
                case Bob:
                    return "Bob";
                case Edgar:
                    return "Edgar";
                case Ward:
                    return "Ward";
                case Jimmy:
                    return "Jimmy";
                default:
                    return "Bob";
            }
        }

        public void Reset()
        {
        }

        public void BeforeShow()
        {
            ResetCharacterSelection();
            CurrentPlayerIndex = 0;
        }

        public void BeforeHide()
        {
        }
    }
}