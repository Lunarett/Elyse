using System.IO;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public struct DamageCauserInfo : IPhotonSerializable
{
    public Player CauserOwner;
    public bool HeadShot;

    public DamageCauserInfo(Player causerOwner, bool headShot = false)
    {
        CauserOwner = causerOwner;
        HeadShot = headShot;
    }

    public byte[] Serialize()
    {
        using (var memStream = new MemoryStream())
        {
            using (var binaryWriter = new BinaryWriter(memStream))
            {
                binaryWriter.Write(CauserOwner.ActorNumber);
                binaryWriter.Write(HeadShot);
            }
            return memStream.ToArray();
        }
    }

    public void Deserialize(byte[] data)
    {
        using (var memStream = new MemoryStream(data))
        {
            using (var binaryReader = new BinaryReader(memStream))
            {
                int actorNumber = binaryReader.ReadInt32();
                CauserOwner = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
                HeadShot = binaryReader.ReadBoolean();
            }
        }
    }
    
    public static short Serialize(StreamBuffer outStream, object customObject)
    {
        DamageCauserInfo damageCauserInfo = (DamageCauserInfo)customObject;
        byte[] data = damageCauserInfo.Serialize();
        
        outStream.Write(data, 0, data.Length);
        return (short)data.Length;
    }

    public static object Deserialize(StreamBuffer inStream, short length)
    {
        byte[] data = new byte[length];
        inStream.Read(data, 0, length);
        
        DamageCauserInfo damageCauserInfo = new DamageCauserInfo();
        damageCauserInfo.Deserialize(data);
        return damageCauserInfo;
    }
}