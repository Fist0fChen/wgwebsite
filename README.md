# WG website
A websize to manage housekeeping in a shared flat.  
This is the successor project to https://github.com/JF0C/wgwebsite01  

## Troubleshooting Dev
docker stop $(docker ps -a -q)  
docker rm $(docker ps -a -q)  
docker rm $(docker image ls)  
docker volume rm $(docker volume ls -q)  
