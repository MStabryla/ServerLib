using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib
{
    public interface IClientManager<T,IProtoco,Data> where T : Client<IProtoco,Data> where IProtoco : IProtocol<Data>
    {
        T Convert(ClientSocket<IProtoco,Data> clientS);
    }
}
