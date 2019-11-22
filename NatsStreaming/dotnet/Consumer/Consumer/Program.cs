using System;
using System.Diagnostics;
using System.Threading;
using STAN.Client;

namespace Consumer
{
    internal class StanSubscriber
    {
        StanSubscriptionOptions _sOpts = StanSubscriptionOptions.GetDefaultOptions();

        private const int Count = 10000;
        private string _clientId = "cs-subscriber";
        private const string ClusterId = "nats-streaming";
        private const string Subject = "foo";
        private const string Url = "nats://nats-cluster:4222";
        private const bool Verbose = false;
        private int _received = 0;

        public void Run(string[] args)
        {
            _clientId += Guid.NewGuid();
            Banner();

            var opts = StanOptions.GetDefaultOptions();
            opts.NatsURL = Url;

            using var c = new StanConnectionFactory().CreateConnection(ClusterId, _clientId, opts);
            while (true)
            {
                _received = 0;
                var elapsed = ReceiveAsyncSubscriber(c);
                Console.Write("Received {0} msgs in {1} seconds ", _received, elapsed.TotalSeconds);
                Console.WriteLine("({0} msgs/second).", (int)(_received / elapsed.TotalSeconds));
            }
        }

        private TimeSpan ReceiveAsyncSubscriber(IStanConnection c)
        {
            var sw = new Stopwatch();
            var ev = new AutoResetEvent(false);

            EventHandler<StanMsgHandlerArgs> msgHandler = (sender, args) =>
            {
                if (_received == 0)
                    sw.Start();

                _received++;

                if (Verbose)
                {
                    Console.WriteLine("Received seq # {0}: {1}",
                        args.Message.Sequence,
                        System.Text.Encoding.UTF8.GetString(args.Message.Data));
                }

                if (_received >= Count)
                {
                    sw.Stop();
                    ev.Set();
                }
            };

            using (var s = c.Subscribe(Subject, _sOpts, msgHandler))
            {
                ev.WaitOne();
            }

            return sw.Elapsed;
        }

        private void Banner()
        {
            Console.WriteLine("Connecting to cluster '{0}' as client '{1}'.", ClusterId, _clientId);
            Console.WriteLine("Consuming {0} messages on subject {1}", Count, Subject);
            Console.WriteLine("  Url: {0}", Url);
        }

        public static void Main(string[] args)
        {
            try
            {
                new StanSubscriber().Run(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.Error.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
        }
    }
}