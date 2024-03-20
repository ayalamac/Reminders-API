FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /bin/Release/net8.0/linux-musl-x64/publish
COPY . .

WORKDIR /app
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]