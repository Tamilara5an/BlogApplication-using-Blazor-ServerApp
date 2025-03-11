FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["BlogApplication.csproj", "./"]
RUN dotnet restore "BlogApplication.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet build "BlogApplication.csproj" -c Release -o /app/build

RUN dotnet publish "BlogApplication.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BlogApplication.dll"]
