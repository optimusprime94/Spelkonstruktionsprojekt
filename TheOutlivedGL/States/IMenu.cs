﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Menu
{
    public interface IMenu
    {
        void Draw(GameTime gameTime, SpriteBatch sb);

        void Update(GameTime gameTime);

        void Reset();

    }
}
