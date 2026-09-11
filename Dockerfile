FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/src/CirMin.API/CirMin.API.csproj backend/src/CirMin.API/
COPY backend/src/CirMin.BusinessLogic/CirMin.BusinessLogic.csproj backend/src/CirMin.BusinessLogic/
COPY backend/src/CirMin.Contracts/CirMin.Contracts.csproj backend/src/CirMin.Contracts/
COPY backend/src/CirMin.DataAccess/CirMin.DataAccess.csproj backend/src/CirMin.DataAccess/

RUN dotnet restore backend/src/CirMin.API/CirMin.API.csproj

COPY backend/ backend/

RUN dotnet publish backend/src/CirMin.API/CirMin.API.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 10000

CMD ["sh", "-c", "dotnet CirMin.API.dll --urls http://0.0.0.0:${PORT:-10000}"]