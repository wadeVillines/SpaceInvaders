namespace Intel8080
{
    public class Memory<T>
        where T : struct
    {
        private T[] _data;

        public T this[int address]
        {
            get => _data[address];
            set => _data[address] = value;
        }

        public int Length => _data.Length;

        public Memory()
        {
            _data = [];
        }

        public Memory(int capacity)
        {
            _data = new T[capacity];
        }

        public void Set(T[] data, int address)
        {
            data.CopyTo(_data, address);
        }

        public void Resize(int size)
        {
            Array.Resize(ref _data, size);
        }
    }
}