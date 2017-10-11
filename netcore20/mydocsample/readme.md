
### Get linux version:
cat /etc/*-release

### Run image with ssh
docker run -ti --entrypoint /bin/sh YOUR_IMAGE


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

## Build Sample


dotnet publish -r debian-x64 -f netcoreapp2.0

docker build --build-arg EXE_DIR=./bin/Debug/netcoreapp2.0/debian-x64/publish -t sample:1.1 .

*run in background*

docker run -d ..

*connect to log*
docker logs -f 36684a
