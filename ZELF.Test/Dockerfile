FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ZELF.Test/ZELF.Test.csproj", "ZELF.Test/"]
RUN dotnet restore "ZELF.Test/ZELF.Test.csproj"
COPY . .
WORKDIR "/src/ZELF.Test"
RUN dotnet build "ZELF.Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZELF.Test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZELF.Test.dll"]
