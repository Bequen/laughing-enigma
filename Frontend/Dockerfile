FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /stag
COPY Frontend/Frontend.csproj /stag/Frontend/
RUN dotnet restore /stag/Frontend/Frontend.csproj
COPY Frontend /stag/Frontend/
COPY Model /stag/Model/
RUN dotnet build "Frontend/Frontend.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Frontend/Frontend.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Frontend.dll"]
