namespace LABA2
{
    public enum GameState
    {
        Start,
        End
    }

    public class Game(int size)
    {
        private readonly Player cat = new("Cat", size);
        private readonly Player mouse = new("Mouse", size);
        private GameState state = GameState.Start;

        public static string InputFile = string.Empty;
        public static string OutFile = string.Empty;

        public void Run(string[] lines)
        {
            var output = new List<string>
            {
                "Cat and Mouse",
                "",
                "Cat Mouse  Distance",
                new('-', 19)
            };

            for (int i = 1; i < lines.Length && state != GameState.End; i++)
            {
                var parts = RemoveOverEntries(lines[i]);

                if (parts.Length == 0) 
                    continue;

                char process = parts[0][0];

                switch (process)
                {
                    case 'M':
                        ProcessMove(parts, mouse);
                        break;
                    case 'C':
                        ProcessMove(parts, cat);
                        break;
                    case 'P':
                        Print(output);
                        break;
                    default:
                        break;
                }

                if (MouseIsCaught())
                    state = GameState.End;
            }

            AppendSummary(output);
            File.WriteAllLines(OutFile, output);
        }

        private static string[] RemoveOverEntries(string line)
        {
            return string.IsNullOrWhiteSpace(line) ? [] : line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private static void ProcessMove(string[] parts, Player player)
        {
            if (parts.Length < 2) 
                return;

            if (!TryParseInt(parts[1], out int value)) 
                return;

            if (player.State == State.NotInGame)
                player.SetPosition(value);
            else
                player.Move(value);
        }

        private static bool TryParseInt(string word, out int value)
        {
            return int.TryParse(word, out value);
        }

        private void Print(List<string> output)
        {
            string catPosition = cat.State == State.NotInGame ? "??" : cat.Location.ToString();
            string mousePosition = mouse.State == State.NotInGame ? "??" : mouse.Location.ToString();
            string distance = (cat.State == State.NotInGame || mouse.State == State.NotInGame) ? "" : GetDistance().ToString();

            output.Add($"{catPosition,2}{mousePosition,6}{distance,8}");
        }

        private bool MouseIsCaught()
        {
            return cat.State == State.Playing && mouse.State == State.Playing && cat.Location == mouse.Location;
        }

        private int GetDistance()
        {
            if (cat.State == State.NotInGame || mouse.State == State.NotInGame) return 0;

            return Math.Abs(cat.Location - mouse.Location);
        }

        private void AppendSummary(List<string> output)
        {
            output.Add(new('-', 19));
            output.Add("");
            output.Add("Distance traveled:   Mouse    Cat");
            output.Add($"{mouse.DistanceTraveled,26}{cat.DistanceTraveled,7}");
            output.Add("");

            if (cat.Location == mouse.Location && cat.State != State.NotInGame && mouse.State != State.NotInGame)
                output.Add($"Mouse caught at: {cat.Location}");
            else
                output.Add("Mouse evaded Cat");
        }
    }
}