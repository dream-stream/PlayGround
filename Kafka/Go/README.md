# Kafka Go
Kafka Go contain an implementation of a Producer and Consumer.

The producer will publish messages to the Kafka setup and the consumer will receive the messages and write it in the console.

## How to execute
This will be a step by step explanation on how to execute the programs and see the described functionality.

### Prerequirements
Before executing the scripts the following tools are required:
- Go
- Docker (Kubernetes in docker)
- Helm (version 3)

### Step by step
It is imporant that the docker instance running kubernetes a lot of resources since kafka, takes a fair bit of resources.\
Install kafka running the following commands:\
`helm repo add incubator http://storage.googleapis.com/kubernetes-charts-incubator`\
`kubectl create ns kafka`\
`helm install --name my-kafka --namespace kafka incubator/kafka`

Apply the deployment of the consumer and publisher by running the create.yaml files in each folder.\
`kubectl apply -f create.yaml`

Push new changes by running the following commands:
`docker build -t banders2/kafka-publisher-golang .`\
`docker push banders2/kafka-publisher-golang`\
`docker build -t banders2/kafka-consumer-golang .`\
`push banders2/kafka-consumer-golang`\
To apply the newly pushed images delete the current running pod, and the new image will be pulled.

To apply kafka commands, go to a kafka pod in Kubernetes and go to exec.
Describe a consumer group:\
`kafka-consumer-groups --bootstrap-server localhost:9092 --group myGroup --describe`\
Reset a specific consumer group for a topic:\
`kafka-consumer-groups --bootstrap-server localhost:9092 --group myGroup --topic myTopic --reset-offsets --to-earliest`

The current setup does not work corretly from localhost, because of redirections from kafka.