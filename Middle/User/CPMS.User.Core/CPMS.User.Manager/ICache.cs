namespace CPMS.User.Manager
{
    public interface ICache<T>
    {
        void Set(string key, T obj);
        T Get(string key);
    }
}
