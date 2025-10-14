using System.IO;
using System.Xml.Serialization;

public static class XmlExporter
{
    public static void Export(Text text, string path)
    {
        var serializer = new XmlSerializer(typeof(Text));
        using var writer = new StreamWriter(path);
        serializer.Serialize(writer, text);
    }
}