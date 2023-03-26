FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /stag
COPY stag/stag.csproj /stag/stag/
RUN dotnet restore /stag/stag/stag.csproj

COPY stag /stag/stag/
COPY Model /stag/Model/
RUN dotnet build "stag/stag.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "stag/stag.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "stag.dll", "-c", "/stag/config/config.json"]
