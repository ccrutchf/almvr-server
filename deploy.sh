echo Checking docker file...
cat ~/docker-version

sshpass -p "$SSH_PASSWORD" ssh -o StrictHostKeyChecking=no $SSH_USER@$SSH_IP "DOCKER_VERSION=$(cat ~/docker-version) && echo $DOCKER_VERSION > ~/othertest" # && bash -s" < ./remote_deploy.sh