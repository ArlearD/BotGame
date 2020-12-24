using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GameControl;
using LiteNetLib;
using LiteNetLib.Utils;
using ServerPart.Scripts;
using UnityEngine;

public class Client : MonoBehaviour, INetEventListener
{
    private NetManager _netClient;
    private NetPacketProcessor _netPacketProcessor;

    /*[SerializeField] private GameObject _clientBall;
    [SerializeField] private GameObject _clientBallInterpolated;*/

    [SerializeField] private string gameKey = "MyFirstGameV0";
    [SerializeField] private string serverAddress = "127.0.0.1";
    [SerializeField] private int serverPort = 5000;
    
    [SerializeField] private MapController mapController;

    private float _newBallPosX;
    private float _oldBallPosX;
    private float _lerpTime;

    void Start()
    {
        _netClient = new NetManager(this);
        _netClient.UnconnectedMessagesEnabled = true;
        _netClient.UpdateTime = 15;
        _netClient.Start();

        _netPacketProcessor = new NetPacketProcessor();
        _netPacketProcessor.RegisterNestedType(() => new ClientData());

        _netPacketProcessor.SubscribeReusable<ClientDataPacket>(packet =>
        {
            mapController.ApplyBots(packet.ClientData);
        });
    }

    void Update()
    {
        _netClient.PollEvents();

        var peer = _netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            /*//Fixed delta set to 0.05
            var pos = _clientBallInterpolated.transform.position;
            pos.x = Mathf.Lerp(_oldBallPosX, _newBallPosX, _lerpTime);
            _clientBallInterpolated.transform.position = pos;

            //Basic lerp
            _lerpTime += Time.deltaTime / Time.fixedDeltaTime;*/
        }
        else
        {
            _netClient.SendUnconnectedMessage(new byte[] {1},
                new IPEndPoint(IPAddress.Parse(serverAddress), serverPort));
            _netClient.Connect(serverAddress, serverPort, gameKey);
            //_netClient.SendBroadcast(new byte[] {1}, 5000);
        }
    }

    void OnDestroy()
    {
        if (_netClient != null)
            _netClient.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        _netPacketProcessor.ReadPacket(reader);
        /*_newBallPosX = reader.GetFloat();

        var pos = _clientBall.transform.position;

        _oldBallPosX = pos.x;
        pos.x = _newBallPosX;

        _clientBall.transform.position = pos;

        _lerpTime = 0f;*/
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.BasicMessage && _netClient.ConnectedPeersCount == 0 &&
            reader.GetInt() == 1)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            _netClient.Connect(remoteEndPoint, gameKey);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }
}