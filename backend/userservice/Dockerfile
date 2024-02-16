FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY src/UserService.csproj ./
RUN dotnet restore

COPY src/ ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UserService.dll"]