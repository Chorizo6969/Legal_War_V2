using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerLaw : IEquatable<PlayerLaw>, INetworkSerializable //Meme principe que playerData
{
    public ulong clientID;
    public FixedString128Bytes law;

    public PlayerLaw(ulong clientID, string law)
    {
        this.clientID = clientID;
        this.law = new FixedString128Bytes(law);
    }

    public bool Equals(PlayerLaw other)
    {
        return clientID == other.clientID && law.Equals(other.law);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref law);
    }
}
