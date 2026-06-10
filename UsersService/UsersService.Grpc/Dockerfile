FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов
COPY UsersService.Grpc/ UsersService.Grpc/
COPY UsersService.Application/ UsersService.Application/
COPY UsersService.Infrastructure/ UsersService.Infrastructure/
COPY UsersService.Domain/ UsersService.Domain/

# Восстанавливаем зависимости
RUN dotnet restore "UsersService.Grpc/UsersService.Grpc.csproj"

# Копируем весь код
COPY . .

# Сборка
WORKDIR "/src/UsersService.Grpc"
RUN dotnet build "UsersService.Grpc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UsersService.Grpc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.Grpc.dll"]
