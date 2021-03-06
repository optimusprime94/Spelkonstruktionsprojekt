﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using ZEngine.Components;

namespace ZEngine.Helpers
{
    public class QuadTree
    {
        public ComponentManager ComponentManager { get; }

        private QuadNode _root;

        public QuadTree(ComponentManager componentManager)
        {
            ComponentManager = componentManager;
        }

        private const int MAX_ELEMENTS = 4;

        public QuadTree CreateTree(IEnumerable<uint> entities, Rectangle bounds)
        {
            if (_root != null) Clear();

            _root = CreateNode(null, bounds);
            foreach (var entity in entities)
            {
                Insert(ref _root, entity);
            }
            return this;
        }

        public QuadNode GetRoot()
        {
            return _root;
        }

        public IEnumerable<KeyValuePair<uint, IEnumerable<uint>>> Entities()
        {
            var stack = new Stack<QuadNode>();
            stack.Push(_root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                for (var i = 0; i < current.PermanentMovingEntities.Count; i++)
                {
                    var permMovingEntityId = current.PermanentMovingEntities[i].Item1;
                    yield return new KeyValuePair<uint, IEnumerable<uint>>(permMovingEntityId,
                        StillEntities(permMovingEntityId, current));
                }
                for (var i = 0; i < current.TempMovingEntitiesCount; i++)
                {
                    var tempMovingEntityId = current.TempMovingEntities[i].Item1;
                    yield return new KeyValuePair<uint, IEnumerable<uint>>(tempMovingEntityId,
                        StillEntities(tempMovingEntityId, current));
                }
                if (current.ChildNodes != null)
                {
                    for (var i = 0; i < current.ChildNodes.Length; i++)
                    {
                        stack.Push(current.ChildNodes[i]);
                    }
                }
            }
        }

        private IEnumerable<uint> StillEntities(uint movingEntityId, QuadNode root)
        {
            var stack = new Stack<QuadNode>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                for (var i = 0; i < current.PermanentStillEntities.Count; i++)
                {
                    yield return current.PermanentStillEntities[i].Item1;
                }
                for (var i = 0; i < current.PermanentMovingEntities.Count; i++)
                {
                    if (movingEntityId == current.PermanentMovingEntities[i].Item1) continue;
                    yield return current.PermanentMovingEntities[i].Item1;
                }
                for (var i = 0; i < current.TempMovingEntitiesCount; i++)
                {
                    if (movingEntityId == current.TempMovingEntities[i].Item1) continue;
                    yield return current.TempMovingEntities[i].Item1;
                }

                for (var i = 0; i < current.TempStillEntitiesCount; i++)
                {
                    yield return current.TempStillEntities[i].Item1;
                }
                if (current.ChildNodes != null)
                {
                    for (var i = 0; i < current.ChildNodes.Length; i++)
                    {
                        stack.Push(current.ChildNodes[i]);
                    }
                }
            }
            var parent = root.Parent;
            while (parent != null)
            {
                for (var i = 0; i < parent.PermanentStillEntities.Count; i++)
                {
                    yield return parent.PermanentStillEntities[i].Item1;
                }
                for (var i = 0; i < parent.PermanentMovingEntities.Count; i++)
                {
                    yield return parent.PermanentMovingEntities[i].Item1;
                }
                parent = parent.Parent;
            }
        }

        public void Clear()
        {
            var stack = new Stack<QuadNode>();
            stack.Push(_root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                current.PermanentMovingEntities.Clear();
                current.PermanentStillEntities.Clear();
                current.TempMovingEntitiesCount = 0;
                current.TempStillEntitiesCount = 0;
                if (current.ChildNodes != null)
                {
                    for (var i = 0; i < current.ChildNodes.Length; i++)
                    {
                        stack.Push(current.ChildNodes[i]);
                    }
                }
                ;
            }
        }

        private QuadNode CreateNode(QuadNode parent, Rectangle bounds)
        {
            var node = new QuadNode
            {
                Parent = parent,
                Bounds = bounds
            };
            return node;
        }

        private void Split(QuadNode node)
        {
            var subWidth = (int) (node.Bounds.Width * 0.5);
            var subHeight = (int) (node.Bounds.Height * 0.5);
            var topLeft = new Rectangle(node.Bounds.Left, node.Bounds.Top, subWidth, subHeight);
            var topRight = new Rectangle(node.Bounds.Right - subWidth, node.Bounds.Top, subWidth, subHeight);
            var bottomLeft = new Rectangle(node.Bounds.Left, node.Bounds.Bottom - subHeight, subWidth, subHeight);
            var bottomRight = new Rectangle(node.Bounds.Right - subWidth, node.Bounds.Bottom - subHeight, subWidth,
                subHeight);

            //Create four child nodes
            node.ChildNodes = new[]
            {
                CreateNode(node, topLeft),
                CreateNode(node, topRight),
                CreateNode(node, bottomLeft),
                CreateNode(node, bottomRight)
            };

            //Place moving entities into child nodes where they fit
            //Or keep them if they only fit in the current node
            for (var i = 0; i < node.TempMovingEntitiesCount; i++)
            {
                PassDownEntity(ref node, node.TempMovingEntities[i]);
                node.TempMovingEntities[i] = null;
            }
            node.TempMovingEntitiesCount = 0;

            //Place still entities into child nodes where they fit
            //Or keep them if they only fit in the current node
            for (var i = 0; i < node.TempStillEntitiesCount; i++)
            {
                PassDownEntity(ref node, node.TempStillEntities[i]);
                node.TempStillEntities[i] = null;
            }
            node.TempStillEntitiesCount = 0;
        }

        private void Insert(ref QuadNode parent, uint entityId)
        {
            var positionComponent = ComponentManager.GetEntityComponentOrDefault<PositionComponent>(entityId);
            var dimensionsComponent =
                ComponentManager.GetEntityComponentOrDefault<DimensionsComponent>(entityId);
            var hasMoveComponent = ComponentManager.EntityHasComponent<MoveComponent>(entityId);
            if (hasMoveComponent)
            {
            }
            InsertIntoTree(ref parent,
                new Tuple<uint, Rectangle, bool>(
                    entityId,
                    new Rectangle((int) positionComponent.Position.X, (int) positionComponent.Position.Y,
                        dimensionsComponent.Width, dimensionsComponent.Height),
                    hasMoveComponent)
            );
        }

        private void InsertIntoTree(ref QuadNode node, Tuple<uint, Rectangle, bool> entity)
        {
            //Fits no more entites, try to split entities into smaller quad nodes
            var reachedMaxEntitiesCount = node.TempMovingEntitiesCount >= MAX_ELEMENTS ||
                                          node.TempStillEntitiesCount >= MAX_ELEMENTS;
            if (reachedMaxEntitiesCount && node.ChildNodes == null)
            {
                if (node.Bounds.Width > 5)
                {
                    Split(node);
                }
            }

            //Has no child nodes and fits more entities
            if (node.ChildNodes == null)
            {
                if (entity.Item3) //Has move component
                {
                    node.TempMovingEntities[node.TempMovingEntitiesCount++] = entity;
                }
                else
                {
                    node.TempStillEntities[node.TempStillEntitiesCount++] = entity;
                }
            }
            else
            {
                PassDownEntity(ref node, entity);
            }
        }

        private void PassDownEntity(ref QuadNode parent, Tuple<uint, Rectangle, bool> entity)
        {
            //If node has children and if the entity fits in any child, put it in that child node
            for (var childNodeIndex = 0; childNodeIndex < parent.ChildNodes.Length; childNodeIndex++)
            {
                var intersectionCount = 0;
//                if (parent.ChildNodes[childNodeIndex].Bounds.Contains(entity.Item2))
                if (entity.Item2.Intersects(parent.ChildNodes[childNodeIndex].Bounds))
                {
                    InsertIntoTree(ref parent.ChildNodes[childNodeIndex], entity);
                    intersectionCount++;
//                    return;
                }
                if (intersectionCount > 0) return;
            }

            //If entity too large too fit in any child node
            if (entity.Item3) //Has MoveComponent
            {
                parent.PermanentMovingEntities.Add(entity);
            }
            else
            {
                parent.PermanentStillEntities.Add(entity);
            }
        }
    }

    public class QuadNode
    {
        public QuadNode Parent;
        public List<Tuple<uint, Rectangle, bool>> PermanentMovingEntities = new List<Tuple<uint, Rectangle, bool>>();
        public List<Tuple<uint, Rectangle, bool>> PermanentStillEntities = new List<Tuple<uint, Rectangle, bool>>();
        public Tuple<uint, Rectangle, bool>[] TempStillEntities = new Tuple<uint, Rectangle, bool>[4];
        public int TempStillEntitiesCount = 0;
        public Tuple<uint, Rectangle, bool>[] TempMovingEntities = new Tuple<uint, Rectangle, bool>[4];
        public int TempMovingEntitiesCount = 0;
        public Rectangle Bounds;
        public QuadNode[] ChildNodes;
    }
}