FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release


WORKDIR /app


COPY ["Backend.csproj", "Backend/"]


RUN dotnet restore "Backend/Backend.csproj"


COPY . ./Backend/


WORKDIR /app/Backend


RUN dotnet build "Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish


ARG BUILD_CONFIGURATION=Release


RUN dotnet publish "Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final


WORKDIR /app


COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "Backend.dll"]
