FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TradePerso.sln ./
COPY src/TradePerso.Domain/TradePerso.Domain.csproj src/TradePerso.Domain/
COPY src/TradePerso.Application/TradePerso.Application.csproj src/TradePerso.Application/
COPY src/TradePerso.Infrastructure/TradePerso.Infrastructure.csproj src/TradePerso.Infrastructure/
COPY src/TradePerso.Web/TradePerso.Web.csproj src/TradePerso.Web/

RUN dotnet restore TradePerso.sln

COPY . .
RUN dotnet publish src/TradePerso.Web/TradePerso.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__TradePerso="Data Source=/data/tradeperso.db"

RUN mkdir -p /data

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TradePerso.Web.dll"]
