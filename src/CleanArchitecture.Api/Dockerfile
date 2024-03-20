FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /bin/Release/net8.0/publish
COPY . app/

ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app

ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]