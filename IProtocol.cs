namespace ServerLib
{
    /// <summary>
    ///    Interface responsible for encoding nad decoding messages.
    /// </summary>
    /// <typeparam name="T">The data class type which your server( and other classes too ) will use to store data.</typeparam>
    public interface IProtocol<T>
    {
        T DecodeProtocol(byte[] buffer);
        byte[] EncodeProtocol(T buffer);
    }
}
