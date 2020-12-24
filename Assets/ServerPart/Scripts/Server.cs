using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Economy.Data;
using GameControl;
using LiteNetLib;
using LiteNetLib.Utils;
using ServerPart.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class Server : MonoBehaviour, INetEventListener, INetLogger
{
    private NetManager _netServer;

    private NetDataWriter _dataWriter;
    //private NetPeer _ourPeer;

    private Dictionary<NetPeer, ClientData> clients = new Dictionary<NetPeer, ClientData>();
    [SerializeField] private int maxClients = 32;

    //[SerializeField] private GameObject _serverBall;
    [SerializeField] private int port = 5000;
    [SerializeField] private string gameKey = "MyFirstGameV0";

    [SerializeField] private MapController mapController;
    [SerializeField] private LevelGenerator levelGenerator;

    void Start()
    {
        NetDebug.Logger = this;
        _dataWriter = new NetDataWriter();
        _netServer = new NetManager(this);
        _netServer.Start(port);
        _netServer.BroadcastReceiveEnabled = true;
        _netServer.UpdateTime = 15;
    }

    void Update()
    {
        _netServer.PollEvents();
    }

    void FixedUpdate()
    {
        /*if (_ourPeer != null)
        {
            _serverBall.transform.Translate(1f * Time.fixedDeltaTime, 0f, 0f);
            _dataWriter.Reset();
            _dataWriter.Put(_serverBall.transform.position.x);
            _ourPeer.Send(_dataWriter, DeliveryMethod.Sequenced);
        }*/
    }

    void OnDestroy()
    {
        NetDebug.Logger = null;
        if (_netServer != null)
            _netServer.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        clients.Add(peer, new ClientData());
        //создаем робота
        var x = Random.Range(460, 60 + 460);
        var z = Random.Range(460, 60 + 460);
        var pos = new Vector3(x, levelGenerator.MaxTileYPosition + 10, z);
        var code = @"var a = _bot.Vizor();
            _bot.GoToPosition(a[0].x, a[0].y);
            _bot.Attack();";
        var name = peer.EndPoint.ToString().Replace(":", "-");
        
        var bot = mapController.InitializeBot(pos, code, name, new PlayerDataFieldsInfo());
        clients[peer].bot = bot;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        //юзается при коннекте по бродкасту
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
            //ответ на запрос о подключении
            NetDataWriter resp = new NetDataWriter();
            resp.Put(1);
            _netServer.SendUnconnectedMessage(resp, remoteEndPoint);
        }
    }

    void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        OnNetworkReceiveUnconnected(remoteEndPoint, reader, messageType);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.Log("[SERVER] Received connection request.");
        if (clients.Count < maxClients)
        {
            request.AcceptIfKey(gameKey);
        }
        else
        {
            request.Reject();
        }
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        Destroy(clients[peer].bot);
        clients.Remove(peer);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }
}