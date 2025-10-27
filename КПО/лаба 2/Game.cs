namespace LABA2
{
    public enum GameState
    {
        Start,
        End
    }

    public class Game
    {
        private readonly Player cat;
        private readonly Player mouse;
        private GameState state = GameState.Start;

        public static string InputFile = string.Empty;
        public static string OutFile = string.Empty;

        public int Size { get; }

        public Game(int size)
        {
            Size = size;
            cat = new Player("Cat", this);
            mouse = new Player("Mouse", this);
        }

        public void Run(string[] lines)
        {
            using var writer = new StreamWriter(OutFile);

            writer.WriteLine("Cat and Mouse\n"+
                "\n"+
                "Cat Mouse  Distance\n"+
                new string('-', 19));

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
                        Print(writer);
                        break;
                }

                if (MouseIsCaught())
                    state = GameState.End;
            }

            AppendSummary(writer);
        }

        private static string[] RemoveOverEntries(string line)
        {
            return string.IsNullOrWhiteSpace(line) ? [] : line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private static void ProcessMove(string[] parts, Player player)
        {
            if (parts.Length < 2)
                return;

            if (!int.TryParse(parts[1], out int value))
                return;

            if (player.State == State.NotInGame)
                player.SetPosition(value);
            else
                player.Move(value);
        }

        private void Print(StreamWriter writer)
        {
            string catPosition = cat.State == State.NotInGame ? "??" : cat.Location.ToString();
            string mousePosition = mouse.State == State.NotInGame ? "??" : mouse.Location.ToString();
            string distance = (cat.State == State.NotInGame || mouse.State == State.NotInGame) ? "" : GetDistance().ToString();

            writer.WriteLine($"{catPosition,2}{mousePosition,6}{distance,8}");
        }

        private bool MouseIsCaught()
        {
            return cat.State == State.Playing && mouse.State == State.Playing && cat.Location == mouse.Location;
        }

        private int GetDistance()
        {
            if (cat.State == State.NotInGame || mouse.State == State.NotInGame)
                return 0;

            return Math.Abs(cat.Location - mouse.Location);
        }

        private void AppendSummary(StreamWriter writer)
        {
            writer.WriteLine(new string('-', 19));
            writer.WriteLine();
            writer.WriteLine("Distance traveled:   Mouse    Cat");
            writer.WriteLine($"{mouse.DistanceTraveled,26}{cat.DistanceTraveled,7}");
            writer.WriteLine();

            if (cat.Location == mouse.Location && cat.State != State.NotInGame && mouse.State != State.NotInGame)
                writer.WriteLine($"Mouse caught at: {cat.Location}");
            else
                writer.WriteLine("Mouse evaded Cat");
        }
    }
} 