#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Infrastructure/Morsley.UK.People.Synchronizer/Morsley.UK.People.Synchronizer.csproj", "src/Infrastructure/Morsley.UK.People.Synchronizer/"]
RUN dotnet restore "src/Infrastructure/Morsley.UK.People.Synchronizer/Morsley.UK.People.Synchronizer.csproj"
COPY . .
WORKDIR "/src/src/Infrastructure/Morsley.UK.People.Synchronizer"
RUN dotnet build "Morsley.UK.People.Synchronizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Morsley.UK.People.Synchronizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Morsley.UK.People.Synchronizer.dll"]