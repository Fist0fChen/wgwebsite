# WG website
manage housekeeping in a shared flat  

## Troubleshooting Dev
docker stop $(docker ps -a -q)  
docker rm $(docker ps -a -q)  
docker rm $(docker image ls)  
docker volume rm $(docker volume ls -q)  
