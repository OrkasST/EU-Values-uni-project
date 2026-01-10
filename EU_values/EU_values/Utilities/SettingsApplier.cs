namespace EU_values.Utilities;

public class SettingsApplier
{
    public GameSettings gameSettings = new GameSettings();
    private Dictionary<int, int> _settings = new Dictionary<int, int>();

    private string _filePath = "..\\..\\..\\Docs\\Settings\\GameSettings.txt";

    public void ReadSettings()
    {
        if (!File.Exists(_filePath))
        {
            WriteSettings();
            return;
        }

        var lines = File.ReadAllLines(_filePath).ToList();

        foreach (var line in lines) {
            var splited = line.Split('_');
            string key = splited[0];
            int value = int.Parse(splited[1]);
        }
    }

    public void WriteSettings()
    {
        File.WriteAllLines(_filePath, PrepareData());
    }

    private List<string> PrepareData()
    {
        List<string> data = new List<string>();



            return data;
    }
}
