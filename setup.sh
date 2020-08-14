image_version=`date +%Y%m%d%H%M`;
echo $image_version;
# cd Jenkins2Docker
git pull --rebase origin master;
docker stop jenkins2docker;
docker rm jenkins2docker;
docker build -t jenkins2docker:$image_version .;
docker images;
docker run -p 10001:80 -d --name jenkins2docker jenkins2docker:$image_version;
# -v ~/docker-data/house-web/appsettings.json:/app/appsettings.json -v ~/docker-data/house-web/NLogFile/:/app/NLogFile   --restart=always
docker logs jenkins2docker;
#删除build过程中产生的镜像    #docker image prune -a -f
docker rmi $(docker images -f "dangling=true" -q)