package main

import (
	"fmt"
	"os"
	"time"

	"github.com/nats-io/nats.go"
)

func main() {
	nc, _ := nats.Connect("localhost:4222")

	// Simple Async Subscriber
	sub, err := nc.Subscribe("foo", func(m *nats.Msg) {
		fmt.Printf("Received a message: %s\n", string(m.Data))
	})

	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	time.Sleep(4 * time.Second)

	// Drain - unsubscribes from a topic when there are no more messages
	sub.Drain()

	nc.Close()
}
