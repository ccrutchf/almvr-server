echo Checking docker file...
cat ~/docker-version

sshpass -p "$SSH_PASSWORD" ssh -o StrictHostKeyChecking=no $SSH_USER@$SSH_IP "env DOCKER_VERSION='$(cat ~/docker-version)' && bash -s" < ./remote_deploy.sh