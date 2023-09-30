using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public delegate void DelAddMsg(string s);

public interface IChatClientService
{
    bool AddMsgtoGUI(string s);
}
public class GUIChatClient : IChatClientService
{
    private readonly GrpcChannel channel;
    private readonly ChatServerService.ChatServerServiceClient client;
    private string nick;
    private Server server;
    private string hostname;
    private AsyncUnaryCall<BcastMsgReply> lastMsgCall;

    public GUIChatClient(bool sec, string serverHostname, int serverPort,
                         string clientHostname)
    {
        this.hostname = clientHostname;
        // setup the client side

        AppContext.SetSwitch(
            "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        channel = GrpcChannel.ForAddress("http://" + serverHostname + ":" + serverPort.ToString());

        client = new ChatServerService.ChatServerServiceClient(channel);
    }

    public bool AddMsgtoGUI(string s)
    {
        lock (this)
        {
            Console.WriteLine(s);
        }
        return true;
    }

    public List<string> Register(string nick, string port)
    {
        this.nick = nick;
        List<string> result = new List<string>();
        // setup the client service
        server = new Server
        {
            Services = { ChatClientService.BindService(new ClientService(this)) },
            Ports = { new ServerPort(hostname, Int32.Parse(port), ServerCredentials.Insecure) }
        };
        server.Start();

        ChatClientRegisterReply reply = client.Register(new ChatClientRegisterRequest
        {
            Nick = nick,
            Url = "http://localhost:" + port
        },
          new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5)));

        foreach (User u in reply.Users)
        {
            result.Add(u.Nick);
        }
        return result;
    }

    public async Task BcastMsg(string m)
    {
        BcastMsgReply reply;
        if (lastMsgCall != null)
        { // await for async end if it's not the first message
            reply = await lastMsgCall.ResponseAsync;
        }
        lastMsgCall = client.BcastMsgAsync(new BcastMsgRequest
        {
            Nick = this.nick,
            Msg = m
        });
    }

    public void ServerShutdown()
    {
        server.ShutdownAsync().Wait();
    }
}


public class ClientService : ChatClientService.ChatClientServiceBase
{
    IChatClientService clientLogic;

    public ClientService(IChatClientService clientLogic)
    {
        this.clientLogic = clientLogic;
    }

    public override Task<RecvMsgReply> RecvMsg(
        RecvMsgRequest request, ServerCallContext context)
    {
        return Task.FromResult(UpdateGUIwithMsg(request));
    }

    public RecvMsgReply UpdateGUIwithMsg(RecvMsgRequest request)
    {
        lock (clientLogic)
        {
            Console.WriteLine(request.Msg);
        }
        return new RecvMsgReply
        {
            Ok = true
        };
    }
}
