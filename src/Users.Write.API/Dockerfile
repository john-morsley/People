FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Users.Write.API/Users.Write.API.csproj", "Users.Write.API/"]
RUN dotnet restore "src/Users.Write.API/Users.Write.API.csproj"
COPY . .
WORKDIR "/src/Users.Write.API"
RUN dotnet build "Users.Write.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Users.Write.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Write.API.dll"]
