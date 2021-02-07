git pull --rebase origin master;
sudo docker stop jenkins2docker;
sudo docker rm jenkins2docker;
sudo docker build -t jenkins2docker .;
sudo docker images;
sudo docker run -p 54907:80 -d --name jenkins2docker jenkins2docker;
# -v ~/docker-data/house-web/appsettings.json:/app/appsettings.json -v ~/docker-data/house-web/NLogFile/:/app/NLogFile   --restart=always
sudo docker logs jenkins2docker;
#删除build过程中产生的镜像    #docker image prune -a -f
sudo docker rmi $(docker images -f "dangling=true" -q);
