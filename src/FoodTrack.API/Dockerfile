FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FoodTrack.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["global.json", "./"]
COPY ["src/FoodTrack.Domain/FoodTrack.Domain.csproj", "src/FoodTrack.Domain/"]
COPY ["src/FoodTrack.Application/FoodTrack.Application.csproj", "src/FoodTrack.Application/"]
COPY ["src/FoodTrack.Infrastructure/FoodTrack.Infrastructure.csproj", "src/FoodTrack.Infrastructure/"]
COPY ["src/FoodTrack.API/FoodTrack.API.csproj", "src/FoodTrack.API/"]

RUN dotnet restore "src/FoodTrack.API/FoodTrack.API.csproj"

COPY . .
RUN dotnet publish "src/FoodTrack.API/FoodTrack.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__FoodTrack="Data Source=/data/foodtrack.db"

COPY --from=build /app/publish .

VOLUME ["/data"]
EXPOSE 8080

ENTRYPOINT ["dotnet", "FoodTrack.API.dll"]
