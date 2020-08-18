image_version='date +%Y%m%d%H%M';
echo $image_version;
# cd docker
git pull --rebase origin master;
sudo docker stop docker;
sudo docker rm docker;
sudo docker build -t docker:$image_version .;
sudo docker images;
sudo docker run -p 10001:80 -d --name docker docker:$image_version;
# -v ~/docker-data/house-web/appsettings.json:/app/appsettings.json -v ~/docker-data/house-web/NLogFile/:/app/NLogFile   --restart=always
sudo docker logs docker;
#删除build过程中产生的镜像    #docker image prune -a -f
sudo docker rmi $(docker images -f "dangling=true" -q)