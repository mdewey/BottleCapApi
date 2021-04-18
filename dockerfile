FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app
# copy csproj and restore as distinct layers
COPY *.csproj .

RUN dotnet restore


# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./
CMD ASPNETCORE_URLS=http://*:$PORT dotnet BottleCapApi.dll