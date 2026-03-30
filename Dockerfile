# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
WORKDIR /src

COPY ["PCmonitoring/PCmonitoring.csproj", "PCmonitoring/"]
RUN dotnet restore "PCmonitoring/PCmonitoring.csproj"

COPY PCmonitoring/ PCmonitoring/
WORKDIR /src/PCmonitoring
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PCmonitoring.dll"]
