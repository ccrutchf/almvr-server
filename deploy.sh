mkdir ~/.ssh
echo $SSH_PRIVATE_KEY > ~/.ssh/id_rsa
echo $SSH_PUBLIC_KEY > ~/.ssh/id_rsa.pub

ssh root@$SERVER_IP 'bash -c' < ./remote_deploy.sh