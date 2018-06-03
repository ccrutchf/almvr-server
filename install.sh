export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

wget -q packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get install apt-transport-https sshpass -y
sudo apt-get update
sudo apt-get install dotnet-sdk-2.1 -y