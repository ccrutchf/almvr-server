echo $SSH_PRIVATE_KEY > ~/.ssh/id_rsa
echo $SSH_PUBLIC_KEY > ~/.ssh/id_rsa.pub

ssh -o StrictHostKeyChecking=no root@$SERVER_IP 'bash -c' < ./remote_deploy.sh