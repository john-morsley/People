FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/People.Write.API/People.Write.API.csproj", "People.Write.API/"]
RUN dotnet restore "src/People.Write.API/People.Write.API.csproj"
COPY . .
WORKDIR "/src/People.Write.API"
RUN dotnet build "People.Write.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "People.Write.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "People.Write.API.dll"]
