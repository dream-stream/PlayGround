FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-buster-slim-arm32v7 AS base
Add Producer/*deb ./
RUN apt-get install ./librdkafka1_0.11.6-1.1_armhf.deb 
RUN apt-get install ./librdkafka++1_0.11.6-1.1_armhf.deb 
RUN apt-get install ./librdkafka-dev_0.11.6-1.1_armhf.deb

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
