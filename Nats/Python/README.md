﻿# NATS Python scripts
NATS Python scripts contain an implementation of a Producer and Consumer.

The producer will publish three messages to the NATS server and the consumer will receive these three messages and write them in the console.

## How to execute
This will be a step by step explanation on how to execute the scripts to see the described functionality.

### Prerequirements
Before executing the scripts the following tools are required:
- Python - 3.7.4
- pip - 19.2.2
- Docker - version 19.03.1, build 74b1e89

The version is the version installed on the computer where the scripts has been tested.

### Step by step
Install the NATS docker image by running the following command:
`docker run -d -p 4222:4222 nats`

Install the library for python:
`pip install asyncio-nats-client`

Run the Consumer.py by the following command:
`python Consumer.py`\
The Consumer will write to the console when it is ready to receive messages.\
To stop the application again press 'enter'.

Run the Producer in another terminal:
`python Producer.py`

