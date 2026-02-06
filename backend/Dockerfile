FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY MusicCatalog.Api/*.csproj ./MusicCatalog.Api/
RUN dotnet restore MusicCatalog.Api/MusicCatalog.Api.csproj
COPY MusicCatalog.Api/. ./MusicCatalog.Api/
WORKDIR /src/MusicCatalog.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "MusicCatalog.Api.dll"]
