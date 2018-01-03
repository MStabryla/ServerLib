using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib
{
    /// <summary>
    /// This interface cointains function to create new client instantions ( used in Server Mode ).
    /// </summary>
    /// <typeparam name="T">Type of client class which inherits from Client.</typeparam>
    /// <typeparam name="IProtoco">The class type inherited from IProtocol class with specified data class type.</typeparam>
    /// <typeparam name="Data">The data class type which your server ( and other classes too ) will use to store data.</typeparam>
    public interface IClientManager<T,IProtoco,Data> where T : Client<IProtoco,Data> where IProtoco : IProtocol<Data>
    {
        /// <summary>
        ///     Method to create new T object instantion with active Socket connection.
        /// </summary>
        /// <param name="clientS">Instantion of ClientSocket class which contains active Socket connection.</param>
        /// <returns>Instance of client which contains active connection in ClientSocket object</returns>
        T Convert(ClientSocket<IProtoco,Data> clientS);
    }
}
