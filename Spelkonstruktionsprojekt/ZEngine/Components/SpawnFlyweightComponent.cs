﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZEngine.Components;
namespace Spelkonstruktionsprojekt.ZEngine.Components
{
    public class SpawnFlyweightComponent : IComponent
    {
        public IComponent Reset()
        {
            return this;
        }
    }
}
