namespace LABA2
{
    public enum State
    {
        Winner,
        Loser,
        Playing,
        NotInGame
    }

    public class Player(string name, int size)
    {
        public string Name { get; } = name;
        public int Location { get; private set; } = -1;
        public State State { get; private set; } = State.NotInGame;
        public int DistanceTraveled { get; private set; } = 0;
        public int Size { get; } = size;

        public bool SetPosition(int position)
        {
            if (Size <= 0)
                return false;

            Location = NormalizeIndex(position);
            State = State.Playing;
            return true;
        }

        public bool Move(int steps)
        {
            if (State == State.NotInGame)
                return false;

            int newLocation = NormalizeIndex(Location + steps);
            DistanceTraveled += Math.Abs(steps);
            Location = newLocation;
            return true;
        }

        private int NormalizeIndex(int position)
        {
            return ((position - 1) % Size + Size) % Size + 1;
        }

        public void MarkWinner()
        {
            State = State.Winner;
        }

        public void MarkLoser()
        {
            State = State.Loser;
        }

        public void Reset()
        {
            Location = -1;
            State = State.NotInGame;
            DistanceTraveled = 0;
        }
    }
}