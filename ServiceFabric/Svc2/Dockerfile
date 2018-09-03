FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 20001

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Svc2/Svc2.csproj Svc2/
RUN dotnet restore Svc2/Svc2.csproj
COPY . .
WORKDIR /src/Svc2
RUN dotnet build Svc2.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Svc2.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Svc2.dll"]