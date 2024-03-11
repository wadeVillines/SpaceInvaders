namespace Shared
{
    public class Memory<T> : List<T>
        where T : struct
    {
        public Memory(int capacity)
        {
            Capacity = capacity;
            for (int i = 0; i < capacity; i++)
                Add(default);
        }

        public void Load(T[] data, int address)
        {
            for (int i = 0; i < data.Length; i++)
                this[address + i] = data[i];
        }
    }
}
