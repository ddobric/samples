

### Hadoop Master UI

http://dado-sr1:8088

### Hadoop UI
http://dado-sr1:50070

# copy input files to root.
docker cp TextFile1.txt hadoop-dotnet-master:/TextFile1.txt
docker cp TextFile2.txt hadoop-dotnet-master:/TextFile2.txt

# starts interactive sessions on running master
docker exec -it hadoop-dotnet-master bash

## Copy files to the input location.
docker cp TextFile3.txt hadoop-dotnet-master:/

## Move file to HDFS folder
hadoop fs -put /TextFile1.txt /input/  


## Copy Mapper and Reducer to master

### Copy master from buster publish folder.
docker cp . hadoop-dotnet-master:/apps

### Copy reducer from buster publish folder.
docker cp . hadoop-dotnet-master:/apps

## Run jobs
Remote in the master running container and execute following command to submit map/reduce jobs.
With this command you will remote into container:
docker exec -it hadoop-dotnet-master bash

With this command you will submitt jobs:
hadoop jar $HADOOP_HOME/share/hadoop/tools/lib/hadoop-streaming-2.7.2.jar -files "/apps" -mapper "dotnet apps/HadoopMapper.dll" -reducer  "dotnet apps/Reducer.dll" -input /input/* -output /output

docker cp publish hadoop-dotnet-master:/apps

#References:
https://blog.sixeyed.com/hadoop-and-net-core-a-match-made-in-docker/