# Étape de build avec le SDK .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier .csproj et restaurer les dépendances
COPY ["UserRoles/UserRoles.csproj", "UserRoles/"]
RUN dotnet restore "UserRoles/UserRoles.csproj"

# Copier tous les fichiers et publier
COPY . .
WORKDIR "/src/UserRoles"
RUN dotnet publish "UserRoles.csproj" -c Release -o /app/publish

# Étape finale avec l'image runtime .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserRoles.dll"]
