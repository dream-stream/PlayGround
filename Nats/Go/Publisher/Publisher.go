package main

import (
	"fmt"

	nats "github.com/nats-io/nats.go"
)

func main() {
	// Connect to a server
	nc, _ := nats.Connect("localhost:4222")

	// Simple Publisher
	fmt.Println("Publishing message: Hello World")
	// Uncommented for message spam
	// for {
	nc.Publish("foo", []byte("Hello World"))
	// }

	nc.Close()
}
