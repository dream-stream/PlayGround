using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Mvccpb;

namespace Dream_Stream.Services
{
    public class LeaderElection
    {
        private const int LeaseTtl = 1;
        private readonly string _leaderKey;
        private readonly string _me;


        public EtcdClient Client { get; set; }

        private const string Prefix = "Leader/";

        // Inspiration for the leader election have been found here: https://www.sandtable.com/etcd3-leader-election-using-python/
        public LeaderElection(EtcdClient client, string topic, string me)
        {
            Client = client;
            _leaderKey = Prefix + topic;
            _me = me;

            Client.Watch(_leaderKey, SetNewElection);
        }

        private async void SetNewElection(WatchResponse watchResponse)
        {
            if (watchResponse.Events.Any(eventS => eventS.Type == Event.Types.EventType.Delete)) await Election();
        }

        public async Task Election()
        {
            var (leader, lease) = await ElectLeader(Client, _me);
            if (leader)
            {
                Console.WriteLine("I'm the leader!!!");

                var count = 0;
                while (count < 100)
                {
                    count++;
                    Thread.Sleep(500);

                    LeaderHandler();

                    Client.LeaseKeepAlive(new LeaseKeepAliveRequest { ID = lease.ID }, Print, CancellationToken.None);
                }

                Console.WriteLine("I'm no longer the leader");
                Thread.Sleep(10000);
            }
        }

        private async void LeaderHandler()
        {
            await Client.PutAsync("topic1/partition1", DateTime.Now.Second.ToString());
            await Client.PutAsync("topic1/partition2", Environment.MachineName);
            await Client.PutAsync("topic1/partition3", _me);
        }

        private void Print(LeaseKeepAliveResponse response)
        {
            //Console.WriteLine(response);
        }

        private async Task<(bool, LeaseGrantResponse lease)> ElectLeader(EtcdClient client, string me)
        {
            bool result;
            var lease = client.LeaseGrant(new LeaseGrantRequest { TTL = LeaseTtl });

            try
            {
                result = await AddLeader(client, _leaderKey, me, lease);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, lease);
            }

            return (result, lease);
        }

        private static async Task<bool> AddLeader(EtcdClient client, string key, string value, LeaseGrantResponse lease)
        {
            var protoKey = ByteString.CopyFromUtf8(key);
            var transactionAsync = await client.TransactionAsync(new TxnRequest
            {
                Compare =
                {
                    new Compare {Key = protoKey, Version = 0}
                },
                Success =
                {
                    new RequestOp
                    {
                        RequestPut = new PutRequest
                        {
                            Key = protoKey,
                            Value = ByteString.CopyFromUtf8(value),
                            Lease = lease.ID
                        }
                    }
                }
            });
            return transactionAsync.Succeeded;
        }
    }
}