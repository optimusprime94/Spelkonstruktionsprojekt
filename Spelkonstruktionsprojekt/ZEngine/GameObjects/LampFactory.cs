using Microsoft.Xna.Framework;
using Penumbra;
using Spelkonstruktionsprojekt.ZEngine.Helpers;

namespace Spelkonstruktionsprojekt.ZEngine.GameObjects
{
    public class LampFactory
    {
        public uint FlickeringLamp(int scale, float intensity, float radius)
        {
            
            var light = new PointLight()
            {
                Scale = new Vector2(scale),
                Radius = radius,
                Intensity = intensity,
                ShadowType = ShadowType.Solid // Will not lit hulls themselves
            };
            var lamp = new EntityBuilder()
                .SetLight(light)
                .SetRendering(50, 50)
                .SetSprite("RedDot")
                .SetPosition(new Vector2(600, 600), 800)
                .BuildAndReturnId();
            return 0;
        }

        public uint HullTester()
        {
            return new EntityBuilder()
//                .SetRendering(50, 50)
//                .SetSprite("RedDot")
//                .SetPosition(new Vector2(600, 600), 100)
//                .SetHull()
                .BuildAndReturnId();
        }
    }
}