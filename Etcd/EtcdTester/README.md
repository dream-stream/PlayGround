# Etcd C#
Etcd contains the implementation of a watch and a leader election example.

## How to execute
The watch example is for a single instance.
While the leader election works best if multiple instances are running to showcase the leader election.

### Prerequirements
Before executing the scripts the following tools are required:
- Visual Studio 16.3.0
- .NET Core 3.0
- Helm 3.0 beta


### Step by step
Setup the etcd cluster by running:
`helm repo add stable https://kubernetes-charts.storage.googleapis.com`<br>
`helm install --name-template my-etcd --set customResources.createEtcdClusterCRD=true stable/etcd-operator`<br>
`kubectl port-forward svc/etcd-cluster 2379:2379`<br>
The etcd cluster will now be accessable on localhost:2379
Open the solution in Visual Studio and run the WatchExample and see the value changing and the the watch getting the changes.
Commented out the watch example and comment in the LeaderElectionExample open two more instances of Visual Studio and run them all.
The three console windows will then output the role of each client.
Alternativly run `dotnet run` in the EtcdTester project folder for running an instance.