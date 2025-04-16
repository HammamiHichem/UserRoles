# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le .csproj (il est à la racine du dépôt)
COPY *.csproj ./

# Restauration des dépendances
RUN dotnet restore

# Copier tout le code et compiler
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Récupérer les binaires publiés
COPY --from=build /app/publish .

# Lancer l’application
ENTRYPOINT ["dotnet", "UserRoles.dll"]
