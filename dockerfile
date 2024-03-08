FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV SolutionDir /src
WORKDIR /src
COPY . .
# Build Api
WORKDIR "/src/Service.Rsu.Management"
RUN dotnet build Service.Rsu.Management.csproj -c Release -o /app/build


FROM build AS publish
RUN dotnet publish Service.Rsu.Management.csproj --no-restore -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Rsu.Management.dll"]