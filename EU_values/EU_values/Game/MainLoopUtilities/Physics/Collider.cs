using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EU_values.Game.MainLoopUtilities.Physics;

public static class Collider
{
    public static string LastFullEnterCollisionId { get; private set; } = "";
    public static string LastPartialEnterCollisionId { get; private set; } = "";
    public static string LastGroundEnterCollisionId { get; private set; } = "";

    public static Hitbox lastPartialEnteredHitbox { get; private set; } = Hitbox.Empty;
    public static Hitbox lastAccuratePartialEnteredHitbox { get; private set; } = Hitbox.Empty;

    public static PointF lastPartialEnterTouchPosition { get; private set; }
    public static PointF lastFullEnterPosition { get; private set; }

    private static bool _isAternativeVectorCheck = false;

    public static Hitbox CreateHitbox(TiledObject? obj)
    {
        if (obj == null) return Hitbox.Empty;
        return new Hitbox((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height, obj.Name);
    }

    public static bool DetectFullEnterCollision(Hitbox movable, List<Hitbox> hitboxes)
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
    public static bool DetectPartialEnterCollision(Hitbox movable, List<Hitbox> hitboxes)
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

    public static bool DetectAccuratelyPartialEnterCollision(Hitbox movable, List<Hitbox> hitboxes)
    {
        foreach (var hitbox in hitboxes)
        {
            if (
                (movable.X > hitbox.X + hitbox.Width && movable.PreviousX >= hitbox.X + hitbox.Width)
                || (movable.X + movable.Width < hitbox.X && movable.PreviousX + movable.Width <= hitbox.X)
                || (movable.Y > hitbox.Y + hitbox.Height && movable.PreviousY >= hitbox.Y + hitbox.Height)
                || (movable.Y + movable.Height < hitbox.Y && movable.PreviousY + movable.Height <= hitbox.Y)
            ) continue;

            var position = CheckSkippedDistance(movable, hitbox);

            if (position.Length == 0) continue;

            float higherObstackleCheckX1 = movable.X + movable.Width / 2 - movable.Width / 3;
            float higherObstackleCheckX2 = movable.X + movable.Width / 2 + movable.Width / 3;

            if (higherObstackleCheckX1 < hitbox.X + hitbox.Width && higherObstackleCheckX2 > hitbox.X &&
                (movable.Y <= hitbox.Y + hitbox.Height || movable.Y + movable.Height <= hitbox.Y)
                )
            {
                lastPartialEnterTouchPosition = CalculateObjectXYPosition(position, movable);
                lastAccuratePartialEnteredHitbox = hitbox;
            }

            else
            {
                lastPartialEnterTouchPosition = CalculateObjectXPosition(position, movable);
                lastAccuratePartialEnteredHitbox = hitbox;
            }

            return true;
        }
        return false;
    }

    private static PointF CalculateObjectXYPosition(float[] position, Hitbox movable)
    {
        return _isAternativeVectorCheck ? new PointF(position[0] - movable.ModifierX, position[1] - movable.Height - movable.ModifierY)
            : new PointF(position[0] - movable.Width - movable.ModifierX, position[1] - movable.Height - movable.ModifierY);
    }
    private static PointF CalculateObjectXPosition(float[] position, Hitbox movable)
    {
        return _isAternativeVectorCheck ? new PointF(position[0] - movable.ModifierX, movable.Y - movable.ModifierY)
            : new PointF(position[0] - movable.Width - movable.ModifierX, movable.Y - movable.ModifierY);
    }

    public static bool DetectGroundEnterCollision(Hitbox movable, Hitbox hitbox)
    {
        if (movable.X + movable.Width >= hitbox.X && movable.X <= hitbox.X + hitbox.Width
            && movable.Y + movable.Height <= hitbox.Y + hitbox.Height)
        {
            LastGroundEnterCollisionId = hitbox.Id;
            return true;
        }
        return false;
    }

    private static float[] CheckSkippedDistance(Hitbox box, Hitbox checkBox)
    {
        float[] upperVectorPoints = [];
        float[] lowerVectorPoints = [];

        if ((box.Y < box.PreviousY && box.X < box.PreviousX) || (box.Y > box.PreviousY && box.X > box.PreviousX))
        {
            lowerVectorPoints = CheckVector(box.X, box.Y + box.Height, box.PreviousX, box.PreviousY + box.Height, checkBox.Y, checkBox.Y + checkBox.Height);
            upperVectorPoints = CheckVector(box.X + box.Width, box.Y, box.PreviousX + box.Width, box.PreviousY, checkBox.Y, checkBox.Y + checkBox.Height);
            _isAternativeVectorCheck = true;
        }
        else
        {
            upperVectorPoints = CheckVector(box.X, box.Y, box.PreviousX, box.PreviousY, checkBox.Y, checkBox.Y + checkBox.Height);
            lowerVectorPoints = CheckVector(box.X + box.Width, box.Y + box.Height, box.PreviousX + box.Width, box.PreviousY + box.Height, checkBox.Y, checkBox.Y + checkBox.Height);
            _isAternativeVectorCheck = false;
        }

        if ((upperVectorPoints[0] <= checkBox.X + checkBox.Width || upperVectorPoints[1] <= checkBox.X + checkBox.Width)
            && (lowerVectorPoints[0] >= checkBox.X || lowerVectorPoints[1] >= checkBox.X))
        {
            return new float[] { lowerVectorPoints[0], checkBox.Y };
        }

        return [];
    }

    private static float[] CheckVector(float currentX, float currentY, float previousX, float previousY, float checkY1, float checkY2)
    {
        float dx = currentX - previousX;
        float dy = currentY - previousY;

        float hyp = (float)Math.Pow((dx * dx + dy * dy), 0.5f);

        float sin = dx / hyp;
        float cos = dy / hyp;

        return new float[] {
            currentX - GetXDistance(currentY, checkY1, cos, sin),
            currentX - GetXDistance(currentY, checkY2, cos, sin)
        };
    }

    private static float GetXDistance(float y, float checkY, float cos, float sin)
    {
        float dy = y - checkY;
        float hyp = dy * cos;
        float dx = hyp * sin;

        return dx;
    }
}
