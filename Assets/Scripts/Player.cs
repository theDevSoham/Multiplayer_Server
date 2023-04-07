using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set;  }
    public string Username { get; private set; }

    public PlayerMovement Movement => movement;

    [SerializeField] private PlayerMovement movement;

    public int health = 100;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {

        foreach (Player otherPlayer in list.Values)
            otherPlayer.SendSpawned(id);

        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(Random.Range(-5, 5), 1f, Random.Range(-5, 5)), Quaternion.identity).GetComponent<Player>();

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = $"{(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.health = 100;
        player.SendSpawned();

        list.Add(id, player);
    }

    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddData(Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.Singleton.Server.Send(AddData(Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.playerSpawned)), toClientId);
    }

    private Message AddData(Message message)
    {
        message.AddUShort(Id);
        message.AddInt(health);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }


    [MessageHandler((ushort)ClientToServerId.name)]

    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.input)]

    private static void Input(ushort fromClientId, Message message)
    {
        if(list.TryGetValue(fromClientId, out Player player))
        {
            player.Movement.SetInput(message.GetBools(8), message.GetVector3());
        }
    }
}
