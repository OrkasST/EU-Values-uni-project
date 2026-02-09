using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;

namespace EU_values.Game.GameObjects;

public class Door
{

    private static string _imagesLocation = "..\\..\\..\\Resources\\Images\\Doors\\Deltarune_ahh_passage";
    private static Dictionary<GameLevels, Image> _images = new() {
        [GameLevels.Level_1] = Image.FromFile($"{_imagesLocation}_level_1.png"),
        [GameLevels.Level_2] = Image.FromFile($"{_imagesLocation}_level_2.png"),
        [GameLevels.Level_3] = Image.FromFile($"{_imagesLocation}_level_3.png"),
        [GameLevels.Level_4] = Image.FromFile($"{_imagesLocation}_level_4.png"),
        [GameLevels.Level_5] = Image.FromFile($"{_imagesLocation}_level_5.png"),
        [GameLevels.Level_6] = Image.FromFile($"{_imagesLocation}_level_6.png"),
        [GameLevels.Level_7] = Image.FromFile($"{_imagesLocation}.png")
    };
    private static Image unknownDoor = Image.FromFile($"{_imagesLocation}_unknown.png");
    public GameLevels Level;

    public Hitbox Hitbox;
    public Box Body;

    public Door(GameLevels level, float x, float y, Hitbox hitbox)
    {
        Level = level;
        this.Hitbox = hitbox;
        Body = new(name: $"{level}_Door", x, y, width: 128, height: 170, image: unknownDoor, isCameraAffected: true);
    }

    public void Open()
    {
        Body.ChangeBackground(_images[Level]);
    }
}
