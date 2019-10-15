
## How to run .NET Core application in docker container?

First, we have to publish the application for OS of the base image. Here is an example for debian auf x64 platform:
~~~~
dotnet publish -r debian-x64 -f netcoreapp2.0
~~~~

Then create the file with the name *dockerfile* and paste following code in the file:
~~~~
FROM microsoft/dotnet
ARG EXE_DIR=. 
WORKDIR /app 
COPY $EXE_DIR/ ./ 
CMD ["./mydocsample"] 
~~~~

This defines the new image on top of the microsoft/dotnet base image. I have used microsoft/dotnet as base, but it is not required to have dotnet installed on the image, because our *publish* has published required framework assemblies for runtime *debian-x64 *.

As next the docker image has to be built:
~~~~
docker build --build-arg EXE_DIR=./bin/Debug/netcoreapp2.0/debian-x64/publish -t ddc:3.0.
~~~~

This will create the new images with name *ddc* and version (tag) *3.0*. If you execute now 
`docker images `
you will see following:

~~~~
REPOSITORY                                                                      TAG                              IMAGE ID            CREATED             SIZE
ddc                                                                             1.2                              8d8e7c5186c6        43 hours ago        1.75GB
ddc <--                                                                         3.0 ,<--                         8d8e7c5186c6        43 hours ago        1.75GB
fra                                                                             1.0                              8d8e7c5186c6        43 hours ago        1.75GB
ddc                                                                             1.0                              6779ba25cd4d        45 hours ago        284MB
microsoft/dotnet                                                                latest                           84b39efffa19        8 days ago          1.68GB
~~~~

Note the second entry in the list. It new is the image, which has been created. 

To run the container with .NET Core application execute following command:
`docker run 8d8e7c5186c6`

To run in in the backgraound:
`docker run 8d8e7c5186c6`

~~~~
C:\mydocsample>docker run 8d8e7c5186c6
Hello World!
Hello World!
Hello World!
Hello World!
~~~~

To see running container:
`docker ps`

you will see the container is running:
~~~~
CONTAINER ID        IMAGE               COMMAND             CREATED             STATUS              PORTS               NAMES
a0f200ad8fb4        8d8e7c5186c6        "./mydocsample"     45 seconds ago      Up 44 seconds                           adoring_kalam
~~~~


To connecto the the log output of the running container:
docker logs -f adoring_kalam


## List of commands related to docker 

### Get linux version:
cat /etc/*-release

### Run image with ssh
docker run -ti --entrypoint /bin/sh YOUR_IMAGE
or
docker run -it --entrypoint=sh YOUR_IMAGE


### Install sudo
apt-get update && apt-get -y install sudo


### Install curl
sudo apt-get install curl

### Register the trusted Microsoft signature key
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg 


sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg 


sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list' 


sudo apt-get update

### Install VS Code
https://code.visualstudio.com/docs/setup/linux

## Install .NET Core on Ubuntu
sudo sh -c 'echo "deb [arch=amd64] http://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'

curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg

sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg

sudo apt-get update

sudo apt-get install dotnet-sdk-2.0.0

## Run image with overwritting of default start command
docker run -it --entrypoint bash imagename:tag
