using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;

namespace EU_values.Game.MainLoopUtilities.Physics;

public static class Collider
{
    public static string LastFullEnterCollisionId { get; private set; } = "";
    public static string LastPartialEnterCollisionId { get; private set; } = "";
    public static string LastGroundEnterCollisionId { get; private set; } = "";

    public static Hitbox lastPartialEnteredHitbox { get; private set; } = Hitbox.Empty;

    public static Hitbox CreateHitbox(TiledObject? obj)
    {
        if (obj == null) return Hitbox.Empty;
        return new Hitbox((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height, obj.Name);
    }

    public static bool DetectFullEnterCollision(RectangleF movable, List<Hitbox> hitboxes)
    {
        foreach (var hitbox in hitboxes)
            if (movable.X + movable.Width <= hitbox.X + hitbox.Width && movable.X >= hitbox.X
                && movable.Y + movable.Height <= hitbox.Y + hitbox.Height && movable.Y >= hitbox.Y)
            {
                LastFullEnterCollisionId = hitbox.Id;
                return true;
            }
        return false;
    }
    public static bool DetectPartialEnterCollision(RectangleF movable, List<Hitbox> hitboxes)
    {
        foreach (var hitbox in hitboxes)
            if (movable.X + movable.Width >= hitbox.X && movable.X <= hitbox.X + hitbox.Width
                && movable.Y + movable.Height >= hitbox.Y && movable.Y <= hitbox.Y + hitbox.Height)
            {
                LastPartialEnterCollisionId = hitbox.Id;
                lastPartialEnteredHitbox = hitbox;
                return true;
            }
        return false;
    }

    public static bool DetectGroundEnterCollision(RectangleF movable, Hitbox hitbox)
    {
        if (movable.X + movable.Width >= hitbox.X && movable.X <= hitbox.X + hitbox.Width
            && movable.Y + movable.Height <= hitbox.Y + hitbox.Height)
        {
            LastGroundEnterCollisionId = hitbox.Id;
            return true;
        }
        return false;
    }
}
