﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spelkonstruktionsprojekt.ZEngine.Constants
{
    public static class EventConstants
    {
        // These constants are used to simplify the usage of the eventbus,
        // e.g. when the user wants to set an action for when the entity
        // walks forward We use these variables so the user doesn't have
        // to know the strings, or misspell them for that matter.
        public const string WalkForward = "entityWalkForwards";
        public const string WalkBackward = "entityWalkBackwards";
        public const string TurnLeft = "entityTurnLeft";
        public const string TurnRight = "entityTurnRight";
        public const string TurnAround = "entityTurnAround";
        public const string FireWeapon = "entityFireWeapon";
        public const string ReloadWeapon = "entityReloadWeapon";
        public const string Running = "entityRun";


        // Collisions
        public const string WallCollision = "WallCollision";
        public const string EnemyCollision = "EnemyCollision";
        public const string BulletCollision = "BulletCollision";
    }
}