FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY AdPlatforms/AdPlatforms.csproj .
RUN dotnet restore AdPlatforms.csproj
COPY AdPlatforms/ .
RUN dotnet publish AdPlatforms.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:6543
EXPOSE 6543

ENTRYPOINT ["dotnet", "TicTacToe.dll"]
