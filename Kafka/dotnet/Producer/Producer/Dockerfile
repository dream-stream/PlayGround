FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-buster-slim-arm32v7 AS base
#FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-disco-arm32v7 AS base
# RUN apk add libc6-compat && apk add libunwind-dev
#ADD Producer/librdkafka* ./
Add Producer/*deb ./
# RUN wget -qO - https://packages.confluent.io/deb/5.3/archive.key | apt-key add -
# RUN add-apt-repository "deb [arch=arm64] https://packages.confluent.io/deb/5.3 stable main"
# RUN apt-get install aptitude 
# RUN cat /etc/apt/sources.list
# RUN apt-get install software-properties-common && apt-get update
# RUN add-apt-repository "deb ftp.dk.debian.org/debian buster main"
# RUN apt-get update
# RUN apt-get install confluent-community-2.12
RUN apt-get install ./librdkafka1_0.11.6-1.1_armhf.deb 
RUN apt-get install ./librdkafka++1_0.11.6-1.1_armhf.deb 
RUN apt-get install ./librdkafka-dev_0.11.6-1.1_armhf.deb
# RUN apt-get install librdkafka1

#RUN apt-get install ./librdkafka1_1.2.1-1_armhf1.deb 
#RUN apt-get install ./librdkafka++1_1.2.1-1_armhf1.deb 
#RUN apt-get install ./librdkafka-dev_1_1.2.1-1_armhf.deb

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Producer/Producer.csproj", "Producer/"]
RUN dotnet restore -r linux-arm "Producer/Producer.csproj"
COPY . .
WORKDIR "/src/Producer"
RUN dotnet build "Producer.csproj" -r linux-arm -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Producer.csproj" -r linux-arm -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]
