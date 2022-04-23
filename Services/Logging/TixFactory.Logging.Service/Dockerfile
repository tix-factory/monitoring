FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Copies the contents of the directory the Dockerfile is in into the image.
COPY ./Application /Application

# Set working directory for the ENTRYPOINT
WORKDIR Application

# Run the application
ENTRYPOINT ["dotnet", "TixFactory.Logging.Service.dll"]
