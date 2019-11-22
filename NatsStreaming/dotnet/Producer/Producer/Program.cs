using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using STAN.Client;

namespace Producer
{
    internal class StanPublisher
    {
        private readonly StanOptions _cOpts = StanOptions.GetDefaultOptions();

        private const int Count = 10000;
        private string _clientId = "cs-publisher";
        private const string ClusterId = "nats-streaming";
        private readonly byte[] _payload = Encoding.UTF8.GetBytes("hello");
        private const string Subject = "foo";
        private const string Url = "nats://nats-cluster:4222";
        private const bool Verbose = false;

        private void Run(string[] args)
        {
            _clientId += Guid.NewGuid();

            Banner();

            _cOpts.NatsURL = Url;
            using var c = new StanConnectionFactory().CreateConnection(ClusterId, _clientId, _cOpts);
            while (true)
            {
                long acksProcessed = 0;
                var sw = Stopwatch.StartNew();
                var ev = new AutoResetEvent(false);

                // async
                for (var i = 0; i < Count; i++)
                {
                    var guid = c.Publish(Subject, _payload, (obj, pubArgs) =>
                    {
                        if (Verbose) Console.WriteLine("Received ack for message {0}", pubArgs.GUID);
                        if (!string.IsNullOrEmpty(pubArgs.Error))
                            Console.WriteLine("Error processing message {0}", pubArgs.GUID);

                        if (Interlocked.Increment(ref acksProcessed) == Count)
                            ev.Set();
                    });

                    if (Verbose)
                        Console.WriteLine("Published message with guid: {0}", guid);
                }

                ev.WaitOne();
                sw.Stop();

                Console.Write("Published {0} msgs with acknowledgements in {1} seconds ", Count, sw.Elapsed.TotalSeconds);
                Console.WriteLine("({0} msgs/second).", (int)(Count / sw.Elapsed.TotalSeconds));
            }
        }

        private void Banner()
        {
            Console.WriteLine("Connecting to cluster '{0}' as client '{1}'.", ClusterId, _clientId);
            Console.WriteLine("Publishing {0} messages on subject {1}", Count, Subject);
            Console.WriteLine("  Url: {0}", Url); 
            Console.WriteLine("  Payload is {0} bytes.", _payload?.Length ?? 0);
            Console.WriteLine("  Publish Mode is Asynchronous" );
        }

        public static void Main(string[] args)
        {
            try
            {
                new StanPublisher().Run(args);
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