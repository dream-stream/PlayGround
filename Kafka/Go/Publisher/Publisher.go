package main

import (
	"fmt"
	"time"

	"gopkg.in/confluentinc/confluent-kafka-go.v1/kafka"
)

func main() {
	server := "my-kafka.kafka"
	p, err := kafka.NewProducer(&kafka.ConfigMap{"bootstrap.servers": server})
	if err != nil {
		panic(err)
	}

	fmt.Println("This is my server: %s", server)

	fmt.Println("This is my line YEYEYEYE")

	defer p.Close()

	// Delivery report handler for produced messages
	go func() {
		fmt.Println("This is my line4 YEYEYEYE")
		for e := range p.Events() {
			fmt.Println("This is my line5 YEYEYEYE")
			switch ev := e.(type) {
			case *kafka.Message:
				if ev.TopicPartition.Error != nil {
					fmt.Printf("Delivery failed: %v\n", ev.TopicPartition)
				} else {
					fmt.Printf("Delivered message to %v\n", ev.TopicPartition)
				}
			}
		}
	}()

	fmt.Println("This is my line2 YEYEYEYE")

	// Produce messages to topic (asynchronously)
	for {
		topic := "myTopic"
		for _, word := range []string{"Welcome", "to", "the", "Confluent", "Kafka", "Golang", "client"} {
			p.Produce(&kafka.Message{
				TopicPartition: kafka.TopicPartition{Topic: &topic, Partition: kafka.PartitionAny},
				Value:          []byte(word),
			}, nil)
		}
		fmt.Println("Looping...")
		time.Sleep(5 * time.Second)
	}
	fmt.Println("This is my line3 YEYEYEYE")
	// Wait for message deliveries before shutting down
	p.Flush(15 * 1000)
}
