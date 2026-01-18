using EU_values.Game.UI.Elements;

namespace EU_values.Game.BaseClasses;

public class PlayerInventory : ComplexDrawableObject
{
    public static Image LeftCellImage { get; private set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\PlayerInventory\\InventoryCell_Left.png");
    public static Image RightCellImage { get; private set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\PlayerInventory\\InventoryCell_Right.png");
    public static Image MiddleCellImage { get; private set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\PlayerInventory\\InventoryCell_Middle.png");
    public static Image CellSelectionImage { get; private set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\PlayerInventory\\InventoryCell_Selection.png");

    public Box Selection { get; private set; }
    public GameUIText ItemName { get; private set; }
    public Box ItemNameBG { get; private set; }

    public int InventorySize { get; private set; }
    public static List<InventoryItem> InventoryItems { get; private set; } = new List<InventoryItem>();

    public PlayerInventory(string name, int x, int y, int inventorySize) : base(name, x, y)
    {
        InventorySize = inventorySize;
        InventoryItems = new List<InventoryItem>(inventorySize);
        Selection = new($"InventoryCellCell", x, y, 128, 128, CellSelectionImage);
        ItemName = new("ItemName", x + 30, y + 128, "Empty", "Times New Roman", 20, Color.White);
        ItemNameBG = new("ItemNameBG", x + 10, y + 128, 108, 25, Color.FromArgb(128, Color.Black));

        for (int i = 0; i < InventorySize; i++)
        {
            if (i == 0) RenderList.AddObject(0, new Box($"InventoryCell{i}", x + i * 128, y, 128, 128, LeftCellImage));
            else if (i == InventorySize - 1) RenderList.AddObject(0, new Box($"InventoryCell{i}", x + i * 128, y, 128, 128, RightCellImage));
            else RenderList.AddObject(0, new Box($"InventoryCell{i}", x + i * 128, y, 128, 128, MiddleCellImage));
        }
        RenderList.AddObject(-1, Selection);
        RenderList.AddObject(-1, ItemNameBG);
        RenderList.AddObject(-1, ItemName);

    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }

    public void MoveSelection(bool isMovingRight)
    {
        var c = isMovingRight ? 1 : -1;

        var selectionX = Selection.Position.X + c * 128;
        var nameBgX = ItemNameBG.Position.X + c * 128;
        var nameX = ItemName.Position.X + c * 128;

        if (selectionX >= Position.X + 128 * InventorySize)
        {
            selectionX = Position.X;
            nameBgX = ItemNameBG.Position.X - 128 * (InventorySize-1);
            nameX = ItemName.Position.X - 128 * (InventorySize-1);
        }
        else if (selectionX < Position.X)
        {
            selectionX = Position.X + 128 * (InventorySize - 1);
            nameBgX = ItemNameBG.Position.X + 128 * (InventorySize - 1);
            nameX = ItemName.Position.X + 128 * (InventorySize - 1);
        }

        Selection.Position = new PointF(selectionX, Selection.Position.Y);
        ItemNameBG.Position = new PointF(nameBgX, ItemNameBG.Position.Y);
        ItemName.Position = new PointF(nameX, ItemName.Position.Y);
    }
}
