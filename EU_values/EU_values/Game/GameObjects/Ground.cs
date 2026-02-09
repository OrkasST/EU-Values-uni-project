using EU_values.Game.BaseClasses;
using EU_values.Game.UI.Elements;

namespace EU_values.Game.GameObjects;

public class Ground: ComplexDrawableObject
{
    public static Image BG = Image.FromFile("..\\..\\..\\Resources\\Images\\Ground\\Wall_tile_damaged.png");
    public Ground (float x, float y, float width, float height): base("Ground", x, y, width, height, isCameraAffected: true)
    {
        for (int i = 0; i < width; i += 62)
        {
            RenderList.AddObject(0, new Box("ground", x + i, y, width: width - i > 64 ? 64 : width - i, height, BG, isCameraAffected: true));
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }
}
