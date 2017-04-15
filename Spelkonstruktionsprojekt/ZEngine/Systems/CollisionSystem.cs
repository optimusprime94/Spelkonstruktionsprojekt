﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZEngine.Managers;
using ZEngine.Components;
using ZEngine.Components.CollisionComponent;
using Spelkonstruktionsprojekt.ZEngine.Components;
using ZEngine.Wrappers;
using Spelkonstruktionsprojekt.ZEngine.Wrappers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;
using ZEngine.Components.MoveComponent;

namespace Spelkonstruktionsprojekt.ZEngine.Systems
{
    class CollisionSystem : ISystem
    {
        private readonly ComponentManager ComponentManager = ComponentManager.Instance;

        public void Collisions()
        {
            var collisionEntities = ComponentManager.GetEntitiesWithComponent<CollisionComponent>();
            foreach (var movingEntity in collisionEntities)
            {
                foreach (var collidableObject in collisionEntities)
                {
                    if (movingEntity.Key == collidableObject.Key) continue;
                    if (
                        ComponentManager.EntityHasComponent<RenderComponent>(collidableObject.Key) &&
                        ComponentManager.EntityHasComponent<RenderComponent>(movingEntity.Key) &&
                        ComponentManager.EntityHasComponent<MoveComponent>(movingEntity.Key))
                    {
                        if (WillCollide(movingEntity.Key, collidableObject.Key))
                        {
                            System.Diagnostics.Debug.WriteLine("COLLIDED");
                            var collisionComponent = movingEntity.Value;
                            var secondCollisionComponent = collidableObject.Value;
                            if (!collisionComponent.collisions.Contains(collidableObject.Key))
                            {
                                collisionComponent.collisions.Add(collidableObject.Key);
                            }
                            if (!secondCollisionComponent.collisions.Contains(movingEntity.Key))
                            {
                                secondCollisionComponent.collisions.Add(movingEntity.Key);
                            }
                        }
                    }

                }
            }
        }

        public bool WillCollide(int movingEntity, int objectEntity)
        {
            var movingRenderable = ComponentManager.GetEntityComponent<RenderComponent>(movingEntity);
            var movingMovable = ComponentManager.GetEntityComponent<MoveComponent>(movingEntity);
            var movingPosition = (Vector2)movingRenderable.PositionComponent.Position;
            var movingVelocity = (Vector2)movingMovable.Velocity;
            float movingDirection = (float) movingMovable.Direction;
            var newPosition = movingPosition + movingVelocity;

            var objectRenderable = ComponentManager.GetEntityComponent<RenderComponent>(objectEntity);
            var objectPosition = objectRenderable.PositionComponent.Position;
            var objectWidth = objectRenderable.DimensionsComponent.Width;
            var objectHeight = objectRenderable.DimensionsComponent.Height;

            var direction = Vector2.Normalize(objectPosition - movingPosition);
            //Creates a ray heading out from the movingObjects facing direction
            var ray = new Ray(new Vector3(movingPosition, 0), new Vector3(direction, 0));
            Debug.WriteLine("\n Ray position" + ray.Position + ", direction" + ray.Direction);

            //Creating boundingbox for which to see if the ray has intersected
            var objectBox = new BoundingBox(
                new Vector3((float) objectPosition.X, (float) objectPosition.Y, 0),
                new Vector3((float) (objectPosition.X + objectWidth), (float) (objectPosition.Y + objectHeight), 0));

            var objectBox2 = new BoundingSphere(
                new Vector3((float)(objectPosition.X + objectWidth / 2), (float)(objectPosition.Y + objectHeight / 2), 0),
                objectWidth);
            
            float? distance = ray.Intersects(objectBox);

            System.Diagnostics.Debug.WriteLine("Distance " + distance + ", actual distance " + Vector2.Distance(newPosition, objectPosition));
            System.Diagnostics.Debug.WriteLine("ObjectPosition " + objectPosition + ", MovingPosition " + newPosition);
            //Distance will be a number if the ray hits the Objects BoundingBox
            if (distance != null)
            {
                //The distance of the new position from the objectPosition will be greater than the ray distance to the object
                // only if there will be a collision on the next frame
                if (distance < (Vector2.Distance(newPosition, objectPosition) + movingRenderable.DimensionsComponent.Width))
                {
                    System.Diagnostics.Debug.WriteLine("COLLISION TRUE");

                    return true;
                }
            }
            return false;
        }
        
        public void DetectCollisions()
        {
            var collisionEntities = ComponentManager.GetEntitiesWithComponent<CollisionComponent>();
            foreach (var movingEntity in collisionEntities)
            {
                var movingEntityId = movingEntity.Key;
                CollisionComponent movingCollisionComponent = movingEntity.Value;

                foreach (var objectEntity in collisionEntities)
                {
                    var objectEntityId = objectEntity.Key;
                    if (movingEntityId != objectEntityId)
                    {
                        CollisionComponent objectCollisionComponent = objectEntity.Value;

                        //insert stuff
                        var renderComponent = ComponentManager.GetEntityComponent<RenderComponent>(movingEntityId);
                        var secondRenderComponent = ComponentManager.GetEntityComponent<RenderComponent>(objectEntityId);

                        var boundingBox = GetSpriteBoundingRectangle(renderComponent, movingCollisionComponent.spriteBoundingRectangle);
                        var secondBoundingBox = GetSpriteBoundingRectangle(secondRenderComponent, objectCollisionComponent.spriteBoundingRectangle);

                        var boundingSphere = GetSpriteBoundingSphere(renderComponent, movingCollisionComponent.spriteBoundingRectangle);
                        var secondBoundingSphere = GetSpriteBoundingSphere(secondRenderComponent, objectCollisionComponent.spriteBoundingRectangle);
                        if (movingCollisionComponent.CageMode)
                        {
                            if (IsCagedBy(objectEntityId, movingEntityId))
                            {
                                System.Diagnostics.Debug.WriteLine("IS CAGED!!!!!! CLAMPED OR WHATEVER");
                                System.Diagnostics.Debug.WriteLine(boundingBox.X + ", " + boundingBox.Y + ", " + boundingBox.Width + ", " + boundingBox.Height);
                                if (!boundingBox.Contains(secondBoundingBox))
                                {
                                    System.Diagnostics.Debug.WriteLine("COLLISION!!!!!! CLAMPED OR WHATEVER");

                                    if (!movingCollisionComponent.collisions.Contains(objectEntityId))
                                    {
                                        movingCollisionComponent.collisions.Add(objectEntityId);
                                    }
                                    if (!objectCollisionComponent.collisions.Contains(movingEntityId))
                                    {
                                        objectCollisionComponent.collisions.Add(movingEntityId);
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("IS NOT!!!!!! NOT CLAMPED OR WHATEVER");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Checking for collision");
                            System.Diagnostics.Debug.WriteLine("Moving object: " + boundingBox);
                            System.Diagnostics.Debug.WriteLine("Imovable object: " + secondBoundingBox);
                            System.Diagnostics.Debug.WriteLine("Imovable object bounding box offset: " + objectCollisionComponent.spriteBoundingRectangle);

                            if (boundingSphere.Intersects(secondBoundingSphere))
                            {
                                if (!movingCollisionComponent.collisions.Contains(objectEntityId))
                                {
                                    movingCollisionComponent.collisions.Add(objectEntityId);
                                }
                                if (!objectCollisionComponent.collisions.Contains(movingEntityId))
                                {
                                    objectCollisionComponent.collisions.Add(movingEntityId);
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        // stops the sprite from going off the screen
        public void Boundering(Vector2D spritePosition, int width, int height)
        {
            spritePosition.X = MathHelper.Clamp((float) spritePosition.X, (0 + (width/2)), (900 -(width/2)));
            spritePosition.Y = MathHelper.Clamp((float) spritePosition.Y, (0 + (height/2)), (500-(height/2)));
        }

        public Rectangle GetSpriteBoundingRectangle(RenderComponent renderComponent, Rectangle spriteBoundingBox)
        {
            var x = renderComponent.PositionComponent.Position.X + spriteBoundingBox.X;
            var y = renderComponent.PositionComponent.Position.Y + spriteBoundingBox.Y;
            var width = spriteBoundingBox.Width;
            var height = spriteBoundingBox.Height;
            return new Rectangle((int) x, (int) y, (int) width, (int) height);
        }
        public BoundingSphere GetSpriteBoundingSphere(RenderComponent renderComponent, Rectangle spriteBoundingBox)
        {
            var x = renderComponent.PositionComponent.Position.X + spriteBoundingBox.X;
            var y = renderComponent.PositionComponent.Position.Y + spriteBoundingBox.Y;
            var width = spriteBoundingBox.Width;
            var height = spriteBoundingBox.Height;
            var tightBoundFactor = 0.8; //makes for a tighter fit.
            return new BoundingSphere(new Vector3((float) x, (float) y, 0), (float) ((width / 2) * tightBoundFactor));
        }

        public bool IsCagedBy(int entityId, int potentialCageId)
        {
            if (ComponentManager.EntityHasComponent<CageComponent>(entityId))
            {
                var cageComponent = ComponentManager.GetEntityComponent<CageComponent>(entityId);
                return cageComponent.CageId == potentialCageId;
            }
            return false;
        }
    }
}
