# Stop old instances
/snap/bin/docker container stop "almvr-server"
/snap/bin/docker container rm "almvr-server"

# Clean up unused images
/snap/bin/docker images -q | xargs /snap/bin/docker rmi

# Start a new instance
/snap/bin/docker run -dit --restart unless-stopped -v almvr-data:/data -p 9090:80 --name="almvr-server" ccrutchf/almvr:$DOCKER_VERSION