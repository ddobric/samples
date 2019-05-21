FROM mcr.microsoft.com/dotnet/core/runtime:3.0-stretch-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["dotnetindocker/dotnetindocker.csproj", "dotnetindocker/"]
RUN dotnet restore "dotnetindocker/dotnetindocker.csproj"
COPY . .
WORKDIR "/src/dotnetindocker"
RUN dotnet build "dotnetindocker.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "dotnetindocker.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "dotnetindocker.dll"]