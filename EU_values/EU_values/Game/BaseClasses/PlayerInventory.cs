using EU_values.Game.GameObjects;
using EU_values.Game.Interfaces;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using System.Diagnostics.Metrics;

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

    public GameWindow ItemDescriptionWindow { get; private set; }

    public int InventorySize { get; private set; }

    private int _selectedCellIndex = 0;
    public List<IStorable?> InventoryItems { get; private set; } = new List<IStorable?>();

    private float _selectionInitialX;
    private float _nameBgInitialX;
    private float _nameInitialX;

    private List<int> _freePositions = [];

    public PlayerInventory(string name, int x, int y, int inventorySize) : base(name, x, y)
    {
        InventorySize = inventorySize;
        InventoryItems = new List<IStorable?>();

        Selection = new($"InventoryCellCell", x, y, 128, 128, CellSelectionImage);
        _selectionInitialX = x;

        ItemName = new("ItemName", x + 64, y + 128, "", "Times New Roman", 20, Color.White);
        ItemName.ChangeTextAlignment(TextPositioning.Center);
        _nameInitialX = ItemName.Position.X;

        ItemNameBG = new("ItemNameBG", x + 10, y + 128, 108, 25, Color.FromArgb(128, Color.Black));
        _nameBgInitialX = ItemNameBG.Position.X;

        ItemDescriptionWindow = new(name: "InventoryItemDescription", x: UIPositioner.CenterX - 400, y: UIPositioner.CenterY - 100, width: 800, height: 180,
            text: "Item Description...",
            textX: 400, textY: 30, textSize: 30, isCameraAffected: false);

        ItemDescriptionWindow.Text.ChangeTextAlignment(TextPositioning.Center);
        ItemDescriptionWindow.ToggleVisibility();

        for (int i = 0; i < InventorySize; i++)
        {
            if (i == 0) RenderList.AddObject(0, new Box($"InventoryCell{i}", x + i * 128, y, 128, 128, LeftCellImage));
            else if (i == InventorySize - 1) RenderList.AddObject(0, new Box($"InventoryCell{i}", x + i * 128, y, 128, 128, RightCellImage));
            else RenderList.AddObject(0, new Box($"InventoryCell{i}", x + i * 128, y, 128, 128, MiddleCellImage));
        }
        RenderList.AddObject(-1, ItemDescriptionWindow);
        RenderList.AddObject(-1, Selection);
        RenderList.AddObject(-1, ItemNameBG);
        RenderList.AddObject(-1, ItemName);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }

    public void MoveSelection(bool isMovingRight)
    {
        var c = isMovingRight ? 1 : -1;

        _selectedCellIndex += c;

        var selectionX = Selection.Position.X + c * 128;
        var nameBgX = ItemNameBG.Position.X + c * 128;
        var nameX = ItemName.Position.X + c * 128;

        if (selectionX >= Position.X + 128 * InventorySize)
        {
            selectionX = Position.X;
            nameBgX = ItemNameBG.Position.X - 128 * (InventorySize - 1);
            nameX = ItemName.Position.X - 128 * (InventorySize - 1);
            _selectedCellIndex = 0;
        }
        else if (selectionX < Position.X)
        {
            selectionX = Position.X + 128 * (InventorySize - 1);
            nameBgX = ItemNameBG.Position.X + 128 * (InventorySize - 1);
            nameX = ItemName.Position.X + 128 * (InventorySize - 1);
            _selectedCellIndex = InventorySize - 1;
        }

        Selection.Position = new PointF(selectionX, Selection.Position.Y);
        ItemNameBG.Position = new PointF(nameBgX, ItemNameBG.Position.Y);
        ItemName.Position = new PointF(nameX, ItemName.Position.Y);

        if (InventoryItems.Count > 0 && _selectedCellIndex < InventoryItems.Count && InventoryItems[_selectedCellIndex] != null)
            ItemName.Text = $"{InventoryItems[_selectedCellIndex]?.Name}";
        else ItemName.Text = "";
    }
    private void MoveSelectionToLastItem()
    {
        if (_selectedCellIndex == InventoryItems.Count - 1) return;

        _selectedCellIndex = InventoryItems.Count - 1;

        var selectionX = _selectionInitialX + _selectedCellIndex * 128;
        var nameBgX = _nameBgInitialX + _selectedCellIndex * 128;
        var nameX = _nameInitialX + _selectedCellIndex * 128;

        Selection.Position = new PointF(selectionX, Selection.Position.Y);
        ItemNameBG.Position = new PointF(nameBgX, ItemNameBG.Position.Y);
        ItemName.Position = new PointF(nameX, ItemName.Position.Y);

        var t = InventoryItems[_selectedCellIndex];
        ItemName.Text = $"{t?.Name}";
    }

    private void MoveSelectionTo(int x)
    {
        _selectedCellIndex = x >= InventorySize ? InventorySize - 1 : x;

        var selectionX = _selectionInitialX + _selectedCellIndex * 128;
        var nameBgX = _nameBgInitialX + _selectedCellIndex * 128;
        var nameX = _nameInitialX + _selectedCellIndex * 128;

        Selection.Position = new PointF(selectionX, Selection.Position.Y);
        ItemNameBG.Position = new PointF(nameBgX, ItemNameBG.Position.Y);
        ItemName.Position = new PointF(nameX, ItemName.Position.Y);

        if (InventoryItems.Count > 0 && _selectedCellIndex < InventoryItems.Count && InventoryItems[_selectedCellIndex] != null)
            ItemName.Text = $"{InventoryItems[_selectedCellIndex]?.Name}";
        else ItemName.Text = "";
    }

    public void StoreItem(IStorable? item)
    {
        if (_freePositions.Count > 0)
        {
            InventoryItems[_freePositions.Last()] = item;
            MoveSelectionTo(_freePositions.Last());
            _freePositions.RemoveAt(_freePositions.Count - 1);
        }
        else if (InventoryItems.Count == InventorySize) return;
        else
        {
            InventoryItems.Add(item);
            MoveSelectionToLastItem();
        }

        if (item is Actor)
        {
            var actor = (Actor)item;
            actor.IsCameraAffected = false;
        }
        if (InventoryItems.Count > 0 && _selectedCellIndex < InventoryItems.Count) ItemName.Text = $"{InventoryItems[_selectedCellIndex]?.Name}";
    }

    public IStorable? RemoveItem()
    {
        if (_selectedCellIndex == InventoryItems.Count || InventoryItems.Count == 0) return null;

        if (_selectedCellIndex != InventoryItems.Count - 1) _freePositions.Add(_selectedCellIndex);

        if (InventoryItems[_selectedCellIndex] is Actor)
        {
            (InventoryItems[_selectedCellIndex] as Actor)?.ChangeCameraRelation(true);
        }
        var item = InventoryItems[_selectedCellIndex];

        if (_selectedCellIndex < InventoryItems.Count - 1) InventoryItems[_selectedCellIndex] = null;
        else InventoryItems.RemoveAt(_selectedCellIndex);
        
        ItemName.Text = "";
        return item;
    }

    public PointF GetCellPosition()
    {
        return new PointF(Position.X + 128 * _selectedCellIndex + 64, Position.Y + 64);
    }

    public string GetItemDescription()
    {
        if (InventoryItems.Count > 0 && _selectedCellIndex < InventoryItems.Count && InventoryItems[_selectedCellIndex] != null)
            return $"{InventoryItems[_selectedCellIndex]?.Description}";
        return "";
    }

    public void ShowItemDescription(string additionalText)
    {
        string text = GetItemDescription();
        if (text != "")
        {
            ItemDescriptionWindow.Text.Text = $"\"{text}\"\n{additionalText}";
            ItemDescriptionWindow.ToggleVisibility(true);
        }
    }
    public void HideItemDescription() => ItemDescriptionWindow.ToggleVisibility(false);

    public IStorable? GetSelectedItem()
    {
        if (InventoryItems.Count == 0) return null;

        if (_selectedCellIndex >= InventoryItems.Count)
        {
            MoveSelectionToLastItem();
        }
        return InventoryItems[_selectedCellIndex];
    }
}
