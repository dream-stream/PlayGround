# NATS Go Scripts
NATS Go scripts contain an implementation of a Producer and Consumer.

The producer will publish one message to the NATS server and the consumer will receive the message and write it in the console.

## How to execute
This will be a step by step explanation on how to execute the scripts and see the described functionality.

### Prerequirements
Before executing the scripts the following tools are required:
- Go
- Docker

### Step by step
Install the NATS docker image by running the following command: `docker run -d -p 4222:4222 nats`

Run the Consumer.go script by `go run .\Consumer.go`
The Consumer will write to the console when it is ready to receive messages.\
To stop the application again press 'enter'.

Run the Publisher.go script by `go run .\Publisher.go`