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

        public void DoMoveCommand(char command, int steps)
        {
            Player player;
            if (command == 'C')
            {
                player = cat;
            }
            else if (command == 'M')
            {
                player = mouse;
            }
            else
            {
                return; // Invalid
            }

            player.Move(steps, size);
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

        public void DoPrintCommand(StreamWriter writer)
        {
            string catPos = (cat.state == State.Playing || cat.state == State.Winner || cat.state == State.Loser) ? cat.location.ToString() : "??";
            string mousePos = (mouse.state == State.Playing || mouse.state == State.Winner || mouse.state == State.Loser) ? mouse.location.ToString() : "??";
            string dist = "";
            if (cat.state == State.Playing && mouse.state == State.Playing)
            {
                dist = GetDistance().ToString();
            }
            writer.WriteLine(string.Format("{0,-5} {1,-7} {2}", catPos, mousePos, dist));
        }

        private int GetDistance()
        {
            return Math.Abs(cat.location - mouse.location);
        }
    }
}