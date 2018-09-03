FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 20009

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY mesh_web_sample/mesh_web_sample.csproj mesh_web_sample/
RUN dotnet restore mesh_web_sample/mesh_web_sample.csproj
COPY . .
WORKDIR /src/mesh_web_sample
RUN dotnet build mesh_web_sample.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish mesh_web_sample.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "mesh_web_sample.dll"]