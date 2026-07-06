FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/SwiftParcel.Api/SwiftParcel.Api.csproj", "SwiftParcel.Api/"]
COPY ["src/SwiftParcel.Application/SwiftParcel.Application.csproj", "SwiftParcel.Application/"]
COPY ["src/SwiftParcel.Domain/SwiftParcel.Domain.csproj", "SwiftParcel.Domain/"]
COPY ["src/SwiftParcel.Infrastructure/SwiftParcel.Infrastructure.csproj", "SwiftParcel.Infrastructure/"]
RUN dotnet restore "SwiftParcel.Api/SwiftParcel.Api.csproj"
COPY src/ .
RUN dotnet publish "SwiftParcel.Api/SwiftParcel.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SwiftParcel.Api.dll"]
