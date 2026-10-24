FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restaurar solo API y Shared (evita los proyectos MAUI)
COPY DulceRelax.API/DulceRelax.API.csproj DulceRelax.API/
COPY DulceRelax.Shared/DulceRelax.Shared.csproj DulceRelax.Shared/
RUN dotnet restore DulceRelax.API/DulceRelax.API.csproj

COPY DulceRelax.API/ DulceRelax.API/
COPY DulceRelax.Shared/ DulceRelax.Shared/
RUN dotnet publish DulceRelax.API/DulceRelax.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "DulceRelax.API.dll"]