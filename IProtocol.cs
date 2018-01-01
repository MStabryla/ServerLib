namespace ServerLib
{
    public interface IProtocol<T>
    {
        T DecodeProtocol(byte[] buffer);
        byte[] CodeProtocol(T buffer);
    }
}
