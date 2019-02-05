FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Terragistry.Web/Terragistry.Web.csproj", "Terragistry.Web/"]
RUN dotnet restore "Terragistry.Web/Terragistry.Web.csproj"
COPY . .
WORKDIR "/src/Terragistry.Web"
RUN dotnet build "Terragistry.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Terragistry.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Terragistry.Web.dll"]