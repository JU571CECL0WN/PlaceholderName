using System;
using Unity.Netcode;

[Serializable]
public struct CellDataNet : INetworkSerializable
{
    public byte type;
    public int roomId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref roomId);
    }
}