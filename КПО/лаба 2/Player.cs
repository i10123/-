using System;

namespace КПО_лаба_2
{
    public class Player
    {
        public string name;
        public State state = State.NotInGame;
        public int location = -1;
        public int traveledDistance = 0;

        public Player(string name)
        {
            this.name = name;
        }

        public void Move(int move, int fieldSize)
        {
            if (state == State.NotInGame)
            {
                location = move;
                state = State.Playing;
            }
            else
            {
                int locationIndex = location - 1;
                locationIndex = (locationIndex + move) % fieldSize;

                if (locationIndex < 0) 
                    locationIndex += fieldSize;

                location = locationIndex + 1;
                traveledDistance += Math.Abs(move);
            }
        }
    }
}
