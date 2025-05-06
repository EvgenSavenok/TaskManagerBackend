FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5255
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем .csproj файлы с правильными путями
COPY ["NotificationsService.Presentation/NotificationsService.Presentation.csproj", "NotificationsService.Presentation/"]
COPY ["NotificationsService.Application/NotificationsService.Application.csproj", "NotificationsService.Application/"]
COPY ["NotificationsService.Domain/NotificationsService.Domain.csproj", "NotificationsService.Domain/"]
COPY ["NotificationsService.Infrastructure/NotificationsService.Infrastructure.csproj", "NotificationsService.Infrastructure/"]

# Восстанавливаем зависимости
RUN dotnet restore "NotificationsService.Presentation/NotificationsService.Presentation.csproj"

# Копируем оставшиеся файлы проекта
COPY . .

# Переходим в папку Presentation
WORKDIR "/src/NotificationsService.Presentation"

# Сборка проекта
RUN dotnet build "NotificationsService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationsService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationsService.Presentation.dll"]
