using System.Xml.Serialization;

namespace LABS_3_4
{
    public class HistoryManager
    {
        private readonly Stack<string> _history = new();
        private readonly XmlSerializer _serializer = new(typeof(Text));

        // Сохранить текущее состояние перед изменением
        public void SaveState(Text text)
        {
            using var writer = new StringWriter();
            _serializer.Serialize(writer, text);
            _history.Push(writer.ToString());
        }

        // Вернуться назад
        public Text? Undo()
        {
            if (_history.Count == 0) 
                return null;

            string previousStateXml = _history.Pop();
            using var reader = new StringReader(previousStateXml);
            return (Text?)_serializer.Deserialize(reader);
        }

        public bool CanUndo => _history.Count > 0;
    }
}