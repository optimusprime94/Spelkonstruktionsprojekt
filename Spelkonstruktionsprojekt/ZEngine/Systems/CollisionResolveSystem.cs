﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZEngine.Components.CollisionComponent;
using ZEngine.Components.MoveComponent;
using ZEngine.Managers;
using static ZEngine.Systems.CollisionEvents;

namespace ZEngine.Systems
{
    class CollisionResolveSystem : ISystem
    {
        private EventBus.EventBus EventBus = ZEngine.EventBus.EventBus.Instance;
        private ComponentManager ComponentManager = ComponentManager.Instance;

        public void ResolveCollisions(Dictionary<CollisionRequirement, CollisionEvent> collisionEvents)
        {
            var collidableEntities = ComponentManager.GetEntitiesWithComponent<CollisionComponent>();

            //For each collidable entity
            foreach (var entity in collidableEntities)
            {
                //Check every occured collision
                foreach (var collisionTarget in entity.Value.collisions)
                {
                    //If the collision matches any valid collision event
                    foreach (var collisionEvent in collisionEvents)
                    {
                        //Collision events are made up from requirement of each party
                        //If both entities (parties) fulfil the component requirements
                        //Then there is a match for a collision event
                        int movingEntityId = entity.Key;
                        var collisionRequirements = collisionEvent.Key;
                        var collisionEventType = collisionEvent.Value;

                        if (MatchesCollisionEvent(collisionRequirements, movingEntityId, collisionTarget))
                        {
                            //When there is a match for a collision-event, an event is published
                            // for any system to pickup and resolve
                            var collisionEventTypeName = FromCollisionEventType(collisionEventType);
                            var collisionEventWrapper = new CollisionEventWrapper(movingEntityId, collisionEventType);
                            EventBus.Publish(collisionEventTypeName, collisionEventWrapper);
                        }
                    }
                }
            }
        }

        private bool MatchesCollisionEvent(CollisionRequirement collisionRequirements, int movingEntityId, int targetId)
        {
            return collisionRequirements.MovingEntityRequirements
                       .Count(componentType => ComponentManager.EntityHasComponent(componentType, movingEntityId))
                            == collisionRequirements.MovingEntityRequirements.Count
                   && collisionRequirements.TargetEntityRequirements
                       .Count(componentType => ComponentManager.EntityHasComponent(componentType, targetId))
                            == collisionRequirements.TargetEntityRequirements.Count;
        }
    }

    public static class CollisionEvents
    {
        public enum CollisionEvent
        {
            Wall = 0,
            Bullet,
            Enemy,
            Neutral
        }

        public static Dictionary<string, CollisionEvent> EventTypes = new Dictionary<string, CollisionEvent>()
            {
                { "Wall", CollisionEvent.Wall },
                { "Bullet", CollisionEvent.Bullet },
                { "Enemy", CollisionEvent.Enemy },
                { "Neutral", CollisionEvent.Neutral },
            };

        public static CollisionEvent FromCollisionEventName(string collisionEventName)
        {
            return EventTypes[collisionEventName];
        }

        public static string FromCollisionEventType(CollisionEvent collisionEvent)
        {
            return EventTypes.First(e => e.Value == collisionEvent).Key;
        }
    }

    //Used for passing event data to system responsible for resolving collision
    public class CollisionEventWrapper
    {
        public int Target;
        public CollisionEvent Event;

        public CollisionEventWrapper(int target, CollisionEvent collisionEvent)
        {
            Target = target;
            Event = collisionEvent;
        }
    }

    //Used for mapping componenet requirements to CollisionEvents
    //This is a preset. The user may setup its own component requirements.
    public class ZEngineCollisionEventPresets
    {
        public static Dictionary<CollisionRequirement, CollisionEvent> StandardCollisionEvents { get; } = new Dictionary<CollisionRequirement, CollisionEvent>()
            {
                {
                    new CollisionRequirement()
                    {
                        MovingEntityRequirements = new List<Type>()
                        {
                            typeof(MoveComponent)
                        },
                        TargetEntityRequirements = new List<Type>()
                        {
                            typeof(CollisionComponent)
                        }
                    },
                    CollisionEvent.Wall
                },
                {
                    new CollisionRequirement()
                    {
                        MovingEntityRequirements = new List<Type>()
                        {
                            typeof(MoveComponent)
                        },
                        TargetEntityRequirements = new List<Type>()
                        {
                            typeof(CollisionComponent),
                            typeof(MoveComponent)
                        }
                    },
                    CollisionEvent.Neutral
                },
            };
    }

    public class CollisionRequirement
    {
        public List<Type> MovingEntityRequirements;
        public List<Type> TargetEntityRequirements;
    }
}