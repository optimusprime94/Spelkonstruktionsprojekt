﻿using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using Spelkonstruktionsprojekt.ZEngine.Components;
using ZEngine.Components;
using ZEngine.Managers;

namespace Spelkonstruktionsprojekt.ZEngine.Systems
{
    public class InertiaDampenerSystem : ISystem
    {
        public void Apply(GameTime gameTime)
        {
            var delta = gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var entity in ComponentManager.Instance.GetEntitiesWithComponent(typeof(MoveComponent)))
            {
                var moveComponent = entity.Value as MoveComponent;
                var dampeningComponent = ComponentManager.Instance
                    .GetEntityComponentOrDefault<InertiaDampeningComponent>(entity.Key);
                if (dampeningComponent == null) return;

                if (moveComponent == null) return;
                var notAccelerating = moveComponent.CurrentAcceleration > -0.01 &&
                                      moveComponent.CurrentAcceleration < 0.01;
                if (notAccelerating && moveComponent.Speed < 0)
                {
                    Debug.WriteLine("damp up");
                    moveComponent.Speed += (float)(dampeningComponent.StabilisingSpeed * delta);
                }
                else if (notAccelerating && moveComponent.Speed > 0)
                {
                    Debug.WriteLine("damp down");
                    moveComponent.Speed -= (float) (dampeningComponent.StabilisingSpeed * delta);
                }
            }
        }
    }
}