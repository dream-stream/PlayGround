using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Producer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Kafka Producer");
            var list = new List<string>();
            for (var i = 0; i < 3; i++) list.Add($"kf-kafka-{i}.kf-hs-kafka.default.svc.cluster.local:9093");
            var bootstrapServers = string.Join(',', list);
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using var p = new ProducerBuilder<Null, string>(config).Build();
            for (var i = 0; i < 200; i++)
            {
                try
                {
                    var dr = await p.ProduceAsync("test-topic3", new Message<Null, string> { Value = $"Message {i}" });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
                await Task.Delay(5000);
            }

            while (true)
            {
                await Task.Delay(1000);
            }
        }
    }
}
