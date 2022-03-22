FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Morsley.UK.People.Read.API/Morsley.UK.People.Read.API.csproj", "Morsley.UK.People.Read.API/"]
RUN dotnet restore "src/Morsley.UK.People.Read.API/Morsley.UK.People.Read.API.csproj"
COPY . .
WORKDIR "/src/Morsley.UK.People.Read.API"
RUN dotnet build "Morsley.UK.People.Read.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Morsley.UK.People.Read.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Morsley.UK.People.Read.API.dll"]
