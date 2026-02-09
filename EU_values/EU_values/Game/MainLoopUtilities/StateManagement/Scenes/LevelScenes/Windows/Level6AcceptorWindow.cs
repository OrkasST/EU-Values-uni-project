using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class Level6AcceptorWindow : GameWindow
{
    public Hitbox InteractiveArea;
    public string AcceptedIcon = "";
    public Level6Icon? StoredIcon;

    public bool IsSelected = false;
    public bool IsFullfilled = false;

    public Level6AcceptorWindow(string name, float x, float y) : base(name, x, y, width: 128, height: 40, text: name, textX: 64, textY: 10, textSize: 20, isCameraAffected: true)
    {
        InteractiveArea = new(x, y, width: 128, height: 256, "InteractiveArea");
        Text.ChangeTextAlignment(TextPositioning.Center);
        AcceptedIcon = name;

        AddObject(new Box("frame", x, y, width: 128, height: 128, Color.FromArgb(180, Color.Black), isFilled: false, isCameraAffected: true), 1);
    }

    public void Select()
    {
        Text.ChangeColor(Color.Green);
        IsSelected = true;
    }
    public void Deselect()
    {
        IsSelected = false;
        if (IsFullfilled) return;
        Text.ChangeColor(Color.White);
    }


    public void StoreItem(Level6Icon? item)
    {
        if (item == null) return;

        item.SetPositionByCenter(Position.X+64, Position.Y+64);
        StoredIcon = item;

        if (item.ID == AcceptedIcon)
        {
            Text.ChangeColor(Color.Gold);
            InteractiveArea.IsCollidable = false;
            IsFullfilled = true;
        }
    }
    public Level6Icon? RemoveItem()
    {
        var item = StoredIcon;
        StoredIcon = null;
        return item;
    }
}
