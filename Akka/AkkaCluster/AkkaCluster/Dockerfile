FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY AkkaCluster/AkkaCluster.csproj AkkaCluster/
COPY AkkaShared/AkkaShared.csproj AkkaShared/
RUN dotnet restore AkkaCluster/AkkaCluster.csproj
COPY . .
WORKDIR /src/AkkaCluster
RUN dotnet build AkkaCluster.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AkkaCluster.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AkkaCluster.dll"]
