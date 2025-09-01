FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ENV NODE_MAJOR=20
RUN apt-get update && \
    apt-get install -y --no-install-recommends ca-certificates curl gnupg && \
    mkdir -p /etc/apt/keyrings && \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg && \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list && \
    apt-get update && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

COPY ["Api/Api.csproj", "Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Telegram/Telegram.csproj", "Telegram/"]
COPY ["Api/EasyUi/package.json", "Api/EasyUi/"]
COPY ["Api/EasyUi/package-lock.json", "Api/EasyUi/"]

ARG VITE_API_BASE_URL_BUILD_ARG

ENV VITE_API_BASE_URL=$VITE_API_BASE_URL_BUILD_ARG

COPY . . 

WORKDIR /src/Api
RUN dotnet restore "Api.csproj"

WORKDIR /src/Api/EasyUi
RUN npm ci
RUN npm run build
WORKDIR /src/Api

RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Api 
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

COPY --from=build /src/Api/EasyUi/dist /app/publish/EasyUi/dist

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]