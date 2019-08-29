package main

import (
	"fmt"
	"time"

	"github.com/eiannone/keyboard"
	"github.com/nats-io/nats.go"
)

func main() {
	nc, _ := nats.Connect("localhost:4222")

	err := keyboard.Open()
	if err != nil {
		panic(err)
	}
	defer keyboard.Close()
	count := 0
	// Simple Async Subscriber
	sub, err := nc.Subscribe("foo", func(m *nats.Msg) {
		count++
		fmt.Printf("Received a message: %s %d\n", string(m.Data), count)
	})
	if err != nil {
		panic(err)
	}

	for {
		time.Sleep(50 * time.Millisecond)

		_, key, err := keyboard.GetKey()
		if err != nil {
			panic(err)
		} else if key == keyboard.KeyEnter {
			break
		}
	}

	// Drain - unsubscribes from a topic when there are no more messages
	sub.Drain()

	nc.Close()
}
