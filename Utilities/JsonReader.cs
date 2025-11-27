using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace qa_dotnet_cucumber.Utilities
{
    public class JsonReader
    {
        public static List<T> ReadData<T>(string filePath, string rootElement)
        {
            string json = File.ReadAllText(filePath);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement.GetProperty(rootElement);

            var data = JsonSerializer.Deserialize<List<T>>(root.GetRawText());
            return data ?? new List<T>();
        }
    }
}




