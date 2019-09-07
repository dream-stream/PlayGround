package main

import (
	"fmt"

	"gopkg.in/confluentinc/confluent-kafka-go.v1/kafka"
)

func main() {
	fmt.Printf("Trying to startup 2\n")
	server := "my-kafka.kafka"
	c, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": server,
		"group.id":          "myGroup",
		"auto.offset.reset": "earliest",
	})

	if err != nil {
		panic(err)
	}

	c.SubscribeTopics([]string{"myTopic", "^aRegex.*[Tt]opic"}, nil)

	for {
		msg, err := c.ReadMessage(-1)
		if err == nil {
			fmt.Printf("Message on %s: %s\n", msg.TopicPartition, string(msg.Value))
		} else {
			// The client will automatically try to recover from all errors.
			fmt.Printf("Consumer error: %v (%v)\n", err, msg)

		}
		// fmt.Println("pooling...")
		// time.Sleep(50 * time.Millisecond)

	}

	c.Close()
}
