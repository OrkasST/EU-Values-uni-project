using EU_values.Game.GameObjects.Actors;

namespace EU_values.Game.MainLoopUtilities.Physics;

public static class Collider
{
    public static string LastFullEnterCollisionId { get; private set; } = "";
    public static string LastPartialEnterCollisionId { get; private set; } = "";

    public static bool DetectFullEnterCollision(Rectangle movable, List<Hitbox> hitboxes)
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
    public static bool DetectPartialEnterCollision(Rectangle movable, List<Hitbox> hitboxes)
    {
        foreach (var hitbox in hitboxes)
            if (movable.X + movable.Width >= hitbox.X && movable.X <= hitbox.X + hitbox.Width
                && movable.Y + movable.Height >= hitbox.Y && movable.Y <= hitbox.Y + hitbox.Height)
            {
                LastPartialEnterCollisionId = hitbox.Id;
                return true;
            }
        return false;
    }
}
