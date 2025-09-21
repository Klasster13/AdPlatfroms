FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY AdPlatforms/AdPlatforms.csproj ./AdPlatforms/
COPY AdPlatforms.Tests/AdPlatforms.Tests.csproj ./AdPlatforms.Tests/
COPY *.sln ./

RUN dotnet restore

COPY AdPlatforms/ ./AdPlatforms/
COPY AdPlatforms.Tests/ ./AdPlatforms.Tests/

RUN dotnet dotnet build -c Release --no-restore
RUN dotnet test -c Release --no-build

RUN dotnet publish AdPlatforms/AdPlatforms.csproj -c Release -o /app/publish --no-restore



FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /app/publish .

RUN adduser --disabled-password --home /app user && chown -R user:user /app
USER user

ENTRYPOINT ["dotnet", "AdPlatforms.dll"]
