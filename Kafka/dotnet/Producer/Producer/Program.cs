using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

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
            //var config = new ProducerConfig { BootstrapServers = bootstrapServers };

            //// If serializers are not specified, default serializers from
            //// `Confluent.Kafka.Serializers` will be automatically used where
            //// available. Note: by default strings are encoded as UTF8.
            //using var p = new ProducerBuilder<Null, string>(config).Build();
            //try
            //{
            //    var dr = await p.ProduceAsync("test-topic", new Message<Null, string> { Value = "test" });
            //    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            //}
            //catch (ProduceException<Null, string> e)
            //{
            //    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            //}

            //while (true)
            //{
            //    await Task.Delay(1000);
            //}

            var brokerList = bootstrapServers;
            const string topicName = "test-topic2";

            var config = new Dictionary<string, object> { { "bootstrap.servers", brokerList } };

            using var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
            Console.WriteLine($"{producer.Name} producing on {topicName}. q to exit.");


            for (var i = 0; i < 1000; i++)
            {
                var deliveryReport = producer.ProduceAsync(topicName, null, $"My message {i}");
                await deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                });
            }


            // Tasks are not waited on synchronously (ContinueWith is not synchronous),
            // so it's possible they may still in progress here.
            producer.Flush(TimeSpan.FromSeconds(10));

            while (true)
            {
                await Task.Delay(1000);
            }
        }
    }
}
