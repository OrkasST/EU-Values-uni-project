using System.Text.Json.Serialization;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;

public class TiledMap
{
    [JsonPropertyName("compressionlevel")]
    public int CompressionLevel { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("infinite")]
    public bool Infinite { get; set; }

    [JsonPropertyName("layers")]
    public List<TiledLayer> Layers { get; set; } = [];

    [JsonPropertyName("nextlayerid")]
    public int NextLayerId { get; set; }

    [JsonPropertyName("nextobjectid")]
    public int NextObjectId { get; set; }

    [JsonPropertyName("orientation")]
    public string Orientation { get; set; } = "";

    [JsonPropertyName("renderorder")]
    public string RenderOrder { get; set; } = "";

    [JsonPropertyName("tiledversion")]
    public string TiledVersion { get; set; } = "";

    [JsonPropertyName("tileheight")]
    public int TileHeight { get; set; }

    [JsonPropertyName("tilesets")]
    public List<TiledTileset> Tilesets { get; set; } = [];

    [JsonPropertyName("tilewidth")]
    public int TileWidth { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("width")]
    public int Width { get; set; }
}

public class TiledLayer
{
    // Common fields
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("opacity")]
    public float Opacity { get; set; }

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    // Tile layer fields
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("data")]
    public List<int>? Data { get; set; }

    // Object layer fields
    [JsonPropertyName("draworder")]
    public string? DrawOrder { get; set; }

    [JsonPropertyName("objects")]
    public List<TiledObject>? Objects { get; set; }

    [JsonPropertyName("offsetx")]
    public float? OffsetX { get; set; }

    [JsonPropertyName("offsety")]
    public float? OffsetY { get; set; }
}

public class TiledObject
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("rotation")]
    public float Rotation { get; set; }
}

public class TiledTileset
{
    [JsonPropertyName("firstgid")]
    public int FirstGid { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";
}