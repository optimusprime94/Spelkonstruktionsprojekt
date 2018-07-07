﻿using System.Collections.Generic;
using Game.Menu.States;
using Game.Menu.States.GameModes;
using Game.Menu.States.GameModes.DeathMatch;
using Game.Menu.States.GameModes.Extinction;
using Game.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Game.Menu.OutlivedStates;

namespace Game.Menu
{
    public class GameManager
    {
        public OutlivedContent OutlivedContent { get; }

        // Here we just say that the first state is the Intro
        protected internal GameState CurrentGameState = GameState.Intro;

        protected internal GameState PreviousGameState;
        //        protected internal KeyboardState OldKeyboardState;
        //        protected internal GamePadState OldGamepadState;

        public VirtualGamePad Controller { get; set; }
        public MenuNavigator MenuNavigator { get; set; }

        //protected internal GameEngine Engine;
        protected internal Viewport viewport;
        protected internal SpriteBatch spriteBatch;
        protected internal BackgroundEffects effects;
        protected internal Microsoft.Xna.Framework.Game game;
        // To keep track of the game configurations made
        protected internal GameConfig gameConfig;

        private IMenu mainMenu;
        private IMenu gameModesMenu;
        private IMenu characterMenu;
        private IMenu credits;
        private IMenu gameIntro;
        private IMenu survivalGame;
        private IMenu pausedMenu;
        private IMenu aboutMenu;
        private IMenu multiplayerMenu;
        private IMenu gameOver;
        private IMenu gameOverCredits;

        public Dictionary<GameState, IMenu> GameStateMenuMap;
        private PlayerVirtualInputCollection virtualInputCollection;

        public void SetCurrentState(GameState state)
        {
            CurrentGameState = state;
        }

        public GameManager()
        {
       //     Engine = OutlivedGame.Instance();
       //     Engine.Logger = Logger;

            spriteBatch = OutlivedGame.Instance().spriteBatch;
            game = OutlivedGame.Instance();
            viewport = OutlivedGame.Instance().graphics.GraphicsDevice.Viewport;

            
            effects = new BackgroundEffects(viewport);
            gameConfig = new GameConfig();

            virtualInputCollection = new PlayerVirtualInputCollection(new[]
            {
                new VirtualGamePad(0, isKeyboardControlled: true),
                new VirtualGamePad(1),
                new VirtualGamePad(2),
                new VirtualGamePad(3)
            });
            
            MenuNavigator = new MenuNavigator(this);

            var gameModeDependencies = new GameModeDependencies()
            {
                GameConfig = gameConfig,
                MenuNavigator = MenuNavigator,
               // GameSystems = Engine,
                Viewport = viewport,
                VirtualInputs = virtualInputCollection
            };
            
            // initializing the states, remember:
            // all the states need to exist in the 
            // manager.
            mainMenu = new MainMenu(this, virtualInputCollection.PlayerOne(), MenuNavigator);
            gameModesMenu = new GameModeMenu(this, MenuNavigator, virtualInputCollection.PlayerOne());
            characterMenu = new CharacterMenu(this, virtualInputCollection);
            credits = new Credits(this, MenuNavigator, virtualInputCollection.PlayerOne());
            gameIntro = new GameIntro(this, MenuNavigator, virtualInputCollection.PlayerOne());
            pausedMenu = new PausedMenu(this, MenuNavigator, virtualInputCollection);
            multiplayerMenu = new MultiplayerMenu(this, MenuNavigator, virtualInputCollection);
            aboutMenu = new AboutMenu(this, MenuNavigator, virtualInputCollection.PlayerOne());
            gameOver = new GameOver(this, MenuNavigator, virtualInputCollection.PlayerOne());
            gameOverCredits = new GameOverCredits(this, MenuNavigator, virtualInputCollection.PlayerOne());

            var gameModeSurvival = new Survival(gameModeDependencies);
            var deathMatch = new DeathMatch(gameModeDependencies);
            var extinction = new Extinction(gameModeDependencies);

            GameStateMenuMap = new Dictionary<GameState, IMenu>
            {
                {GameState.Intro, gameIntro},
                {GameState.MainMenu, mainMenu},
//                {GameState.SurvivalGame, survivalGame},
                {GameState.SurvivalGame, gameModeSurvival},
                {GameState.PlayDeathMatchGame, deathMatch},
                {GameState.PlayExtinctionGame, extinction},
                {GameState.Quit, mainMenu},
                {GameState.GameModesMenu, gameModesMenu},
                {GameState.CharacterMenu, characterMenu},
                {GameState.Credits, credits},
                {GameState.Paused, pausedMenu},
                {GameState.MultiplayerMenu, multiplayerMenu},
                {GameState.About, aboutMenu},
                {GameState.GameOver, gameOver},
                {GameState.GameOverCredits,  gameOverCredits}
            };
            var lifecycleStates = new Dictionary<GameState, ILifecycle>
            {
                {GameState.SurvivalGame, gameModeSurvival},
                {GameState.PlayDeathMatchGame, deathMatch},
                {GameState.PlayExtinctionGame, extinction},
                {GameState.MultiplayerMenu, (ILifecycle) multiplayerMenu},
                {GameState.CharacterMenu, (ILifecycle) characterMenu},
                {GameState.GameOver, (ILifecycle) gameOver}
            };

            MenuNavigator.GameStateMenuMap = GameStateMenuMap;
            MenuNavigator.MenuStates = lifecycleStates;
        }

        // Draw method consists of a switch case with all
        // the different states that we have, depending on which
        // state we are we use that state's draw method.
        public void Draw(GameTime gameTime)
        {
            if (CurrentGameState == GameState.Paused)
            {
                OutlivedGame.Instance().GraphicsDevice.Viewport = viewport;
            }

            if (CurrentGameState == GameState.Quit)
            {
                OutlivedGame.Instance().Exit();
            }
            else if (GameStateMenuMap.ContainsKey(CurrentGameState))
            {
                GameStateMenuMap[CurrentGameState].Draw(gameTime, spriteBatch);
            }
        }

        // Same as the draw method, the update method
        // we execute is the one of the current state.
        public void Update(GameTime gameTime)
        {
            foreach (var virtualGamePad in virtualInputCollection.VirtualGamePads)
            {
                virtualGamePad.UpdateKeyboardState();
            }

            if (CurrentGameState == GameState.Paused)
            {
                OutlivedGame.Instance().GraphicsDevice.Viewport = viewport;
            }

            if (CurrentGameState == GameState.Quit)
            {
                OutlivedGame.Instance().Exit();
            }
            else if (GameStateMenuMap.ContainsKey(CurrentGameState))
            {
                GameStateMenuMap[CurrentGameState].Update(gameTime);
            }

            foreach (var virtualGamePad in virtualInputCollection.VirtualGamePads)
            {
                virtualGamePad.MoveCurrentStatesToOld();
            }
        }
    }
}