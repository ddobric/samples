
Right mouse click on AkkaCluster project and add container support (i.e.:linux)

~~~

### Create Container Instances:
az container create -g  RG-AKKA-SUMCLUSTER --name akka-sum-host1 --image damir.azurecr.io/akka-sum-cluster:v1 --ports 8089 --ip-address Public --cpu 2 --memory 1 --dns-name-label akka-sum-host1 --environment-variables AKKAPORT=8089 AKKASEEDHOSTS="akka-sum-host1.westeurope.azurecontainer.io:8089,akka-sum-host2.westeurope.azurecontainer.io:8089" AKKAPUBLICHOSTNAME=akka-sum-host1.westeurope.azurecontainer.io --registry-username damir --registry-password kkjOvM=J/sEyYTBW6TFltwuV5qXkVH70

az container create -g  RG-AKKA-SUMCLUSTER --name akka-sum-host2 --image damir.azurecr.io/akka-sum-cluster:v1 --ports 8089 --ip-address Public --cpu 2 --memory 1 --dns-name-label akka-sum-host2 --environment-variables AKKAPORT=8089 AKKASEEDHOSTS="akka-sum-host1.westeurope.azurecontainer.io:8089,akka-sum-host2.westeurope.azurecontainer.io:8089" AKKAPUBLICHOSTNAME=akka-sum-host2.westeurope.azurecontainer.io --registry-username damir --registry-password kkjOvM=J/sEyYTBW6TFltwuV5qXkVH70

### Build Docker Image
cd C:\dev\git\me\samples\Akka\AkkaCluster>

docker build --rm -f "AkkaCluster\Dockerfile" -t akkacluster:v1 .

### Run docker image
docker run -it --rm -p 8090:8090 akkacluster:v1 --AKKAPORT=8090 --AKKASEEDHOSTS=localhost:8090 --AKKAPUBLICHOST=localhost

docker run -it --rm -p 8090:8090 akkacluster:v1 --AKKAPORT=8090 --AKKASEEDHOSTS=DADO-SR1:8090 --AKKAPUBLICHOST=DADI-SR1

### Tagging and publishing image
docker tag akkacluster:v1 damir.azurecr.io/akka-sum-cluster:v1

docker push damir.azurecr.io/akka-sum-cluster:v1

~~~