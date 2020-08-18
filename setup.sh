image_version='date +%Y%m%d%H%M';
echo $image_version;
# cd Jenkins2Docker
git status  
git add *  
git commit -m 'Test Git'
# git commit -m 'add some results from Server'
git pull --rebase origin master   #domnload data
git push origin master            #upload data
git stash pop
sudo docker stop jenkins2docker;
sudo docker rm jenkins2docker;
sudo docker build -t jenkins2docker:$image_version .;
sudo docker images;
sudo docker run -p 10001:80 -d --name jenkins2docker jenkins2docker:$image_version;
# -v ~/docker-data/house-web/appsettings.json:/app/appsettings.json -v ~/docker-data/house-web/NLogFile/:/app/NLogFile   --restart=always
sudo docker logs jenkins2docker;
#删除build过程中产生的镜像    #docker image prune -a -f
sudo docker rmi $(docker images -f "dangling=true" -q)