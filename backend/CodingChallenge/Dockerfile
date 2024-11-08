# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY backend/CodingChallenge/CodingChallenge.csproj CodingChallenge/
RUN dotnet restore "CodingChallenge/CodingChallenge.csproj"

# Copy the rest of the application code
COPY backend/CodingChallenge/ CodingChallenge/

# Build the application
WORKDIR /src/CodingChallenge
RUN dotnet build "CodingChallenge.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "CodingChallenge.csproj" -c Release -o /app/publish --no-restore

# Use the ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CodingChallenge.dll"]
