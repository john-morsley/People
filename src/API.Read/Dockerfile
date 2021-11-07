FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Users.Read.API/Users.Read.API.csproj", "Users.Read.API/"]
RUN dotnet restore "src/Users.Read.API/Users.Read.API.csproj"
COPY . .
WORKDIR "/src/Users.Read.API"
RUN dotnet build "Users.Read.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Users.Read.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Read.API.dll"]
