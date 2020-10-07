using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLib;

namespace ServerCommandSystem
{
    public class ServerData
    {
        public int ActionCode { get; set; }
        public string Text { get; set; }
        public ServerData(int _code,string mess)
        {
            ActionCode = _code;
            Text = mess;
        }
    }
    public class ServerProtocol : IProtocol<ServerData>
    {
        public byte[] EncodeProtocol(ServerData buffer)
        {
            byte[] text = Encoding.UTF8.GetBytes(buffer.Text);
            byte[] returner = new byte[1 + text.Length];
            returner[0] = Convert.ToByte(buffer.ActionCode);
            for(int i=1;i<returner.Length;i++)
            {
                returner[i] = text[i - 1];
            }
            return returner;
        }

        public ServerData DecodeProtocol(byte[] buffer)
        {
            if(buffer.Length < 2)
            {
                throw new Exception("Error: błądny buffer");
            }
            int actionCode = buffer[0];
            byte[] stringBuffer = buffer.FragmentOfByteTable(1, buffer.Length - 1);
            string str = Encoding.UTF8.GetString(stringBuffer);
            return new ServerData(actionCode,str);
        }
    }
    public static class Math
    {
        public static byte[] FragmentOfByteTable(this byte[] table,int from,int to)
        {
            if(from > to)
            {
                throw new Exception("Error: from większe od to");
            }
            if(table.Length > to+1)
            {
                throw new Exception("Error: to przekracza rozmiar tablicy");
            }
            byte[] lul = new byte[to + 1 - from];
            for(int i=0;i<lul.Length;i++)
            {
                lul[i] = table[i + from];
            }
            return lul;
        }
    }
}
