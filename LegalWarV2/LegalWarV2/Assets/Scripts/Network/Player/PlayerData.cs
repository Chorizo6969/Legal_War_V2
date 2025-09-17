using System;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientID;
    public int meshID;
    public int playerIndex;

    public bool Equals(PlayerData other)
    {
        return clientID == other.clientID &&
               meshID == other.meshID &&
               playerIndex == other.playerIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref meshID);
        serializer.SerializeValue(ref playerIndex);
    }
}
