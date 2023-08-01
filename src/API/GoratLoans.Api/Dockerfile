FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GoratLoans.Api/GoratLoans.Api.csproj", "GoratLoans.Api/"]
RUN dotnet restore "GoratLoans.Api/GoratLoans.Api.csproj"
COPY . .
WORKDIR "/src/GoratLoans.Api"
RUN dotnet build "GoratLoans.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoratLoans.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoratLoans.Api.dll"]
