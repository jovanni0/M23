FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Instalam dotnet-ef pentru migrații la pornire
RUN dotnet tool install --global dotnet-ef

COPY M23.sln ./
COPY M23.Controller/M23.Controller.csproj M23.Controller/
COPY M23.Shared/M23.Shared.csproj M23.Shared/
RUN dotnet restore M23.Controller/M23.Controller.csproj

COPY M23.Controller/ M23.Controller/
COPY M23.Shared/ M23.Shared/
WORKDIR /src/M23.Controller
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "M23.Controller.dll"]
