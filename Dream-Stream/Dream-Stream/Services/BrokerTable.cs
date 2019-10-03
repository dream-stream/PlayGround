using System;
using System.Threading;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;

namespace Dream_Stream.Services
{
    public class BrokerTable
    {
        private const int LeaseTtl = 10;
        private readonly EtcdClient _client;
        private long _leaseId;
        private readonly ByteString _key;
        private readonly ByteString _broker;
        private Timer _timer;
        private const string Prefix = "Broker/";

        public BrokerTable(EtcdClient client)
        {
            _client = client;
            var machineName = Environment.MachineName;
            _broker = ByteString.CopyFromUtf8(machineName);
            _key = ByteString.CopyFromUtf8(Prefix + machineName);
        }

        public async Task ImHere()
        {
            var leaseGrantRequest = new LeaseGrantRequest
            {
                TTL = LeaseTtl
            };

            var leaseGrantResponse = await _client.LeaseGrantAsync(leaseGrantRequest);
            _leaseId = leaseGrantResponse.ID;
            await _client.PutAsync(new PutRequest
            {
                Lease = leaseGrantResponse.ID,
                Key = _key,
                Value = _broker
            });

            _timer = new Timer(LeaseKeepAlive, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private void LeaseKeepAlive(object state)
        {
            _client.LeaseKeepAlive(new LeaseKeepAliveRequest{ID = _leaseId}, KeepAliveResponseHandler, CancellationToken.None);
        }

        private static void KeepAliveResponseHandler(LeaseKeepAliveResponse leaseKeepAliveResponse)
        {
            //Console.WriteLine(leaseKeepAliveResponse.ID);
        }
    }
}