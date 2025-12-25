namespace EasyTools.Utils.Pool
{
    public interface IPool<T>
    {
        public T Get();

        public void Return(T obj);
    }
}
