FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5022
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проекта
COPY ["TasksService.Presentation/TasksService.Presentation.csproj", "TasksService.Presentation/"]
COPY ["TasksService.Infrastructure/TasksService.Infrastructure.csproj", "TasksService.Infrastructure/"]
COPY ["TasksService.Application/TasksService.Application.csproj", "TasksService.Application/"]
COPY ["TasksService.Domain/TasksService.Domain.csproj", "TasksService.Domain/"]

# Восстанавливаем зависимости
RUN dotnet restore "TasksService.Presentation/TasksService.Presentation.csproj"

# Копируем остальные файлы
COPY . .

# Сборка проекта
WORKDIR "/src/TasksService.Presentation"
RUN dotnet build "TasksService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "TasksService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TasksService.Presentation.dll"]
