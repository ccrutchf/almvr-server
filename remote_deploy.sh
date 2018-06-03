# Stop old instances
docker container stop "almvr-server"
docker container rm "almvr-server"

# Start a new instance
docker run -dit --restart unless-stopped -v almvr-data:/data -p 9090:80 --name="almvr-server" ccrutchf/almvr:$DOCKER_VERSION

# Clean up unused images
docker images -q | xargs docker rmi