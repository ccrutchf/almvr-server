OLD_DOCKER_VERSION=$(docker inspect --format='{{.Config.Image}}' "almvr-server")

# Stop old instances
/snap/bin/docker container stop "almvr-server"
/snap/bin/docker container rm "almvr-server"

# Clean up after ourselves
/snap/bin/docker image rm $OLD_DOCKER_VERSION

# Start a new instance
/snap/bin/docker run -dit --restart unless-stopped -v almvr-data:/data -p 9090:80 --name="almvr-server" ccrutchf/almvr:$DOCKER_VERSION