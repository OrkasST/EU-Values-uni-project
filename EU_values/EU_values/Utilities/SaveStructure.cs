using System.Text.Json.Serialization;

namespace EU_values.Utilities;

public class SaveStructure
{
    [JsonPropertyName("Fields")]
    public List<SaveField> Fields { get; set; } = [];
}

public class SaveField
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Level")]
    public int Level { get; set; }

}
