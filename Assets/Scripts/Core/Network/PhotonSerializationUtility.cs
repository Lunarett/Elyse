using ExitGames.Client.Photon;

public interface IPhotonSerializable
{
    byte[] Serialize();
    void Deserialize(byte[] data);
}

public static class PhotonSerializationUtility
{
    public static void RegisterIPhotonSerializable<T>() where T : IPhotonSerializable, new()
    {
        PhotonPeer.RegisterType(typeof(T), (byte)typeof(T).Name.GetHashCode(), Serialize, Deserialize<T>);
    }

    private static byte[] Serialize(object customObject)
    {
        IPhotonSerializable serializableObject = (IPhotonSerializable)customObject;
        return serializableObject.Serialize();
    }

    private static object Deserialize<T>(byte[] data) where T : IPhotonSerializable, new()
    {
        T result = new T();
        result.Deserialize(data);
        return result;
    }
}