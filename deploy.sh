echo Checking docker file...
cat ~/docker-version

sshpass -p "$SSH_PASSWORD" scp -o StrictHostKeyChecking=no ./remote_deploy.sh $SSH_USER@$SSH_IP:~/
sshpass -p "$SSH_PASSWORD" ssh -o StrictHostKeyChecking=no $SSH_USER@$SSH_IP "export DOCKER_VERSION='$(cat ~/docker-version)' && chmod u+x ./remote_deploy.sh && ./remote_deploy.sh"