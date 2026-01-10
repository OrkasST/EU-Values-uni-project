using EU_values.Game.MainLoopUtilities.StateManagement;

namespace EU_values.Game.BaseClasses;

public abstract class ComplexDrawableObject : DrawableObject
{
    public RenderLayerList RenderList { get; private set; } = new RenderLayerList();

    public ComplexDrawableObject(string name, int x, int y) : base(name, x, y, 0, 0) { }
    public ComplexDrawableObject(string name, int x, int y, int width, int height) : base(name, x, y, width, height) { }
    public ComplexDrawableObject(string name, int x, int y, Size size) : base(name, x, y, size) { }
}
