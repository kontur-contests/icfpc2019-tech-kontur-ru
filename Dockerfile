FROM mcr.microsoft.com/dotnet/core/sdk:2.2.300-bionic

RUN apt-get update && apt-get -y install firefox