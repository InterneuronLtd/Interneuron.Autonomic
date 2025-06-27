FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install curl -y
WORKDIR /app
COPY buildfolder/ ./
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
ENTRYPOINT ["dotnet", "Interneuron.Autonomic.dll"]