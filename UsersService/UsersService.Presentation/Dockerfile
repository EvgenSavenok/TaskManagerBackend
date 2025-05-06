FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5151
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проекта
COPY ["UsersService.Presentation/UsersService.Presentation.csproj", "UsersService.Presentation/"]
COPY ["UsersService.Application/UsersService.Application.csproj", "UsersService.Application/"]
COPY ["UsersService.Domain/UsersService.Domain.csproj", "UsersService.Domain/"]
COPY ["UsersService.Infrastructure/UsersService.Infrastructure.csproj", "UsersService.Infrastructure/"]

# Восстанавливаем зависимости
RUN dotnet restore "UsersService.Presentation/UsersService.Presentation.csproj"

# Копируем остальные файлы
COPY . .

# Сборка проекта
WORKDIR "/src/UsersService.Presentation"
RUN dotnet build "UsersService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "UsersService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.Presentation.dll"]
