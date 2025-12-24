FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Postech.NETT11.PhaseOne.WebApp/Postech.NETT11.PhaseOne.WebApp.csproj", "src/Postech.NETT11.PhaseOne.WebApp/"]
#COPY ["Postech.NETT11.PhaseOne.Application/Postech.NETT11.PhaseOne.Application.csproj", "src/Postech.NETT11.PhaseOne.Application/"]
COPY ["Postech.NETT11.PhaseOne.Domain/Postech.NETT11.PhaseOne.Domain.csproj", "src/Postech.NETT11.PhaseOne.Domain/"]
COPY ["Postech.NETT11.PhaseOne.Infrastructure/Postech.NETT11.PhaseOne.Infrastructure.csproj", "src/Postech.NETT11.PhaseOne.Infrastructure/"]

RUN dotnet restore "src/Postech.NETT11.PhaseOne.WebApp/Postech.NETT11.PhaseOne.WebApp.csproj"

COPY . .

WORKDIR "/src/Postech.NETT11.PhaseOne.WebApp"
RUN dotnet build "Postech.NETT11.PhaseOne.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Postech.NETT11.PhaseOne.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Postech.NETT11.PhaseOne.WebApp.dll"]
