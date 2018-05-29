FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
EXPOSE 80

VOLUME [ "/data" ]

# Copy all of the build
COPY ./artifacts /app

ENTRYPOINT [ "dotnet", "AlmVR.Server.dll" ]