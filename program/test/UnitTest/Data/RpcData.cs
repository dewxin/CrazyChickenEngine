using Block.RPC.Attr;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Data
{

    [MessagePackObject]
    public class ServerConnectRet
    {
        [Key(0)]
        public ushort ServerId { get; set; }
        [Key(1)]
        public string PeerType { get; set; }
    }

    [MessagePackObject]
    [RpcData(RpcDataOption.UseCompress)]
    public class RpcDataAgres
    {
        [Key(0)]
        public int Val1 { get; set; }

    }


    [MessagePackObject]
    public class RpcDataNotAgres
    {
        [Key(0)]
        public int Val1 { get; set; }

    }


    public class RpcBase
    {
        [IgnoreMember]
        public int Val1;
    };

    [MessagePackObject]
    public class MyClass :RpcBase
    {
        // Key attributes take a serialization index (or string name)
        // The values must be unique and versioning has to be considered as well.
        // Keys are described in later sections in more detail.
        [Key(0)]
        public int Age { get; set; }

        [Key(1)]
        public string FirstName { get; set; }

        [Key(2)]
        public string LastName { get; set; }

        // All fields or properties that should not be serialized must be annotated with [IgnoreMember].
        [IgnoreMember]
        public string FullName { get { return FirstName + LastName; } }
    }

    [MessagePackObject]
    public class MyClassPlain
    {
        // Key attributes take a serialization index (or string name)
        // The values must be unique and versioning has to be considered as well.
        // Keys are described in later sections in more detail.
        [Key(0)]
        public int Age { get; set; }

        [Key(1)]
        public string FirstName { get; set; }

        [Key(2)]
        public string LastName { get; set; }

        // All fields or properties that should not be serialized must be annotated with [IgnoreMember].
        [IgnoreMember]
        public string FullName { get { return FirstName + LastName; } }
    }



}
