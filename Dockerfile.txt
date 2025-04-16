# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copier les fichiers .csproj et restaurer les dépendances
COPY *.csproj ./
RUN dotnet restore

# Copier le reste des fichiers et builder l'application
COPY . ./
RUN dotnet publish -c Release -o out

# Étape 2 : Image de runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Exposer le port (à ajuster selon ton app)
EXPOSE 80

# Lancer l'application
ENTRYPOINT ["dotnet", "UserRoles.dll"]
