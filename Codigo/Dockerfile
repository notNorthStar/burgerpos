# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para aprovechar cache de capas
COPY BurgerPOS.slnx .
COPY BurgerPOS.Domain/BurgerPOS.Domain.csproj BurgerPOS.Domain/
COPY BurgerPOS.Application/BurgerPOS.Application.csproj BurgerPOS.Application/
COPY BurgerPOS.Infrastructure/BurgerPOS.Infrastructure.csproj BurgerPOS.Infrastructure/
COPY BurgerPOS.Web/BurgerPOS.Web.csproj BurgerPOS.Web/

RUN dotnet restore BurgerPOS.Web/BurgerPOS.Web.csproj

# Copiar el resto del codigo y publicar
COPY . .
RUN dotnet publish BurgerPOS.Web/BurgerPOS.Web.csproj \
    -c Release \
    -o /app/publish

# Stage 2: Runtime (imagen mas ligera, sin SDK)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "BurgerPOS.Web.dll"]
