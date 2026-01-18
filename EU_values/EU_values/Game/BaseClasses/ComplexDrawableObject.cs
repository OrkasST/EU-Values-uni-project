using EU_values.Game.MainLoopUtilities.StateManagement;

namespace EU_values.Game.BaseClasses;

public abstract class ComplexDrawableObject : DrawableObject
{
    public RenderLayerList RenderList { get; private set; } = new RenderLayerList();

    public ComplexDrawableObject(string name, int x, int y, bool? isCameraAffected = false) : base(name, x, y, 0, 0, isCameraAffected) { }
    public ComplexDrawableObject(string name, int x, int y, int width, int height, bool? isCameraAffected = false) : base(name, x, y, width, height, isCameraAffected) { }
    public ComplexDrawableObject(string name, int x, int y, Size size, bool? isCameraAffected = false) : base(name, x, y, size, isCameraAffected) { }
    public ComplexDrawableObject(string name, int x, int y, int width, int height, Image image, bool? isCameraAffected = false) : base(name, x, y, width, height, image, isCameraAffected) { }
    public ComplexDrawableObject(string name, float x, float y, float width, float height, Image image, bool? isCameraAffected = false) : base(name, x, y, width, height, image, isCameraAffected) { }
}
