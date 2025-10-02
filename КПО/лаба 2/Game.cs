using System;
using System.IO;

namespace КПО_лаба_2
{
    public class Game
    {
        public int size;
        public Player cat;
        public Player mouse;
        public GameState state = GameState.Start;

        public Game(int size)
        {
            this.size = size;
            cat = new Player("Cat");
            mouse = new Player("Mouse");
        }

        public void DoingCommand(char command, int move)
        {
            Player player;
            if (command == 'C')
                player = cat;
            else if (command == 'M')
                player = mouse;
            else
                return;

            player.Move(move, size);
        }

        public bool CheckCatch()
        {
            if (cat.state == State.Playing && mouse.state == State.Playing && cat.location == mouse.location)
            {
                cat.state = State.Winner;
                mouse.state = State.Loser;
                state = GameState.End;
                return true;
            }
            return false;
        }

        public void Print(StreamWriter writer)
        {
            string CatPosition = (cat.state == State.Playing || cat.state == State.Winner 
                                  || cat.state == State.Loser) ? cat.location.ToString() : "??";

            string MousePosition = (mouse.state == State.Playing || mouse.state == State.Winner 
                                  || mouse.state == State.Loser) ? mouse.location.ToString() : "??";

            string distance = "";

            if (cat.state == State.Playing && mouse.state == State.Playing)
            {
                distance = GetDistance().ToString();
            }
            writer.WriteLine($"{CatPosition,-5} {MousePosition,-6} {distance}");
        }

        private int GetDistance()
        {
            return Math.Abs(cat.location - mouse.location);
        }
    }
}
