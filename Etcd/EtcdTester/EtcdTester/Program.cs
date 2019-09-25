using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Mvccpb;

namespace EtcdTester
{
    internal class Program
    {
        private const int LeaseTtl = 1;
        private const string LeaderKey = "leader/topic1";

        public static async Task Main(string[] args)
        {
            var me = Guid.NewGuid().ToString();
            Console.WriteLine("Hello World!");

            var client = new EtcdClient("http://localhost");
            //await WatchExample(client);
            await LeaderElectionExample(client, me);

            //Console.WriteLine(client.GetVal(LeaderKey));
            //Console.WriteLine(client.Delete(LeaderKey));

            client.Dispose();
        }

        private static async Task LeaderElectionExample(EtcdClient client, string me)
        {
            client.Watch(LeaderKey, SetNewElection);
            while (true)
            {
                NewElection = false;
                var (leader, lease) = await LeaderElection(client, me);
                if (leader)
                {
                    Console.WriteLine("I'm the leader!!!");
                    var count = 0;
                    while (count < 20)
                    {
                        count++;
                        client.LeaseKeepAlive(new LeaseKeepAliveRequest { ID = lease.ID }, Print, CancellationToken.None);
                        Thread.Sleep(500);
                    }

                    Console.WriteLine("I'm no longer the leader");
                    Thread.Sleep(10000);
                }
                else
                {
                    Console.WriteLine("I'm a follower!!!");
                    while (!NewElection)
                    {
                        Thread.Sleep(500);
                        //Console.WriteLine("Still a follower");
                    }
                }
            }
        }

        private static void SetNewElection(WatchResponse watchResponse)
        {
            if(watchResponse.Events.Any(eventS => eventS.Type == Event.Types.EventType.Delete)) NewElection = true;
        }

        public static bool NewElection { get; set; }

        // Inspiration for the leader election have been found here: https://www.sandtable.com/etcd3-leader-election-using-python/
        private static async Task<(bool, LeaseGrantResponse lease)> LeaderElection(EtcdClient client, string me)
        {
            bool result;
            var lease = client.LeaseGrant(new LeaseGrantRequest{TTL = LeaseTtl});

            try
            {
                result = await AddLeader(client, LeaderKey, me, lease);
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
            var protoKey = Google.Protobuf.ByteString.CopyFromUtf8(key);
            var transactionAsync = await client.TransactionAsync(new TxnRequest
            {
                Compare =
                {
                    new Compare{Key = protoKey, Version = 0}
                },
                Success =
                {
                    new RequestOp
                    {
                        RequestPut = new PutRequest
                        {
                            Key = protoKey,
                            Value = Google.Protobuf.ByteString.CopyFromUtf8(value),
                            Lease = lease.ID
                        }
                    }
                },
                Failure = { }
            });
            return transactionAsync.Succeeded;
        }








        private static async Task WatchExample(EtcdClient client)
        {
            var count = 0;
            client.WatchRange("topic1", Print);
            while (count < 50)
            {
                count++;

                if (count % 10 == 0) await client.PutAsync("topic1/partition1", $"pod{count}");
                if (count % 3 == 0) await client.PutAsync("topic1/partition2", $"pod{count}");

                //var valAsync = await client.GetValAsync("foo/bar");
                //Console.WriteLine(valAsync);

                // Print function that prints key and value from the watch response

                Thread.Sleep(500);
            }
        }

        private static void Print(WatchResponse response)
        {
            if (response.Events.Count == 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                foreach (var responseEvent in response.Events)
                {
                    Console.WriteLine($"{responseEvent.Kv.Key.ToStringUtf8()}:{responseEvent.Kv.Value.ToStringUtf8()}");
                }
            }
        }

        private static void Print(LeaseKeepAliveResponse response)
        {
            //Console.WriteLine(response);
        }
    }
}
