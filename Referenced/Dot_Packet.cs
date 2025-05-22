using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot320A
{
    public class Dot_Packet
    {
        public bool SYNC_1 = false;
        public bool SYNC_2 = false;
        public ushort Length = 0;
        public byte DEST_ID = 0;
        public byte CMD_H = 0;
        public byte CMD_L = 0;
        public byte SEQ_NUM = 0;
        public byte[] DATA = new byte[1024];
        public byte CS_DATA = 0;
        public ushort IDX_PARSE = 0;
    }
}
