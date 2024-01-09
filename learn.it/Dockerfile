FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["learn.it/learn.it.csproj", "learn.it/"]
RUN dotnet restore "learn.it/learn.it.csproj"
COPY . .
WORKDIR "/src/learn.it"
RUN dotnet build "learn.it.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "learn.it.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "learn.it.dll"]