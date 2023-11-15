using System.IO;
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
}
