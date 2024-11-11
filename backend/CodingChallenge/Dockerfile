
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/CodingChallenge/CodingChallenge.csproj CodingChallenge/
RUN dotnet restore "CodingChallenge/CodingChallenge.csproj"

COPY backend/CodingChallenge/ CodingChallenge/

WORKDIR /src/CodingChallenge
RUN dotnet build "CodingChallenge.csproj" -c Release -o /app/build

RUN dotnet publish "CodingChallenge.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CodingChallenge.dll"]
