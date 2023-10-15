namespace flexington.WFC
{
    public class Socket
    {
        public Direction Direction { get; private set; }
        public int Hash { get; private set; }

        public Socket(Direction direction, int hash)
        {
            Direction = direction;
            Hash = hash;
        }
    }
}