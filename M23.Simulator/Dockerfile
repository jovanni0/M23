FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiem soluția și proiectele pentru restore
COPY M23.sln ./
COPY M23.Simulator/M23.Simulator.csproj M23.Simulator/
COPY M23.Shared/M23.Shared.csproj M23.Shared/
RUN dotnet restore M23.Simulator/M23.Simulator.csproj

# Copiem restul codului și compilăm
COPY M23.Simulator/ M23.Simulator/
COPY M23.Shared/ M23.Shared/
WORKDIR /src/M23.Simulator
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "M23.Simulator.dll"]
