using System;

namespace КПО_лаба_2
{
    public class Player
    {
        public string name;
        public State state = State.NotInGame;
        public int location = -1;
        public int distanceTraveled = 0;

        public Player(string name)
        {
            this.name = name;
        }

        public void Move(int steps, int fieldSize)
        {
            if (state == State.NotInGame)
            {
                location = steps;
                state = State.Playing;
            }
            else
            {
                int zeroBased = location - 1;
                zeroBased = (zeroBased + steps) % fieldSize;
                if (zeroBased < 0) zeroBased += fieldSize;
                location = zeroBased + 1;
                distanceTraveled += Math.Abs(steps);
            }
        }
    }
}
