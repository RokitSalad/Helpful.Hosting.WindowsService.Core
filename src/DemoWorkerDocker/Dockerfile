#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DemoWorkerDocker/DemoWorkerDocker.csproj", "DemoWorkerDocker/"]
COPY ["Helpful.Hosting.WorkerService.Linux/Helpful.Hosting.WorkerService.Linux.csproj", "Helpful.Hosting.WorkerService.Linux/"]
COPY ["Helpful.Hosting.WorkerService/Helpful.Hosting.WorkerService.csproj", "Helpful.Hosting.WorkerService/"]
COPY ["Helpful.Hosting.Dto/Helpful.Hosting.Dto.csproj", "Helpful.Hosting.Dto/"]
RUN dotnet restore "DemoWorkerDocker/DemoWorkerDocker.csproj"
COPY . .
WORKDIR "/src/DemoWorkerDocker"
RUN dotnet build "DemoWorkerDocker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoWorkerDocker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /etc/systemd/system
COPY --from=publish /app/publish/src/DemoWorkerDocker/demoworkerdocker.service .
RUN chmod +x /app/initialise.sh
RUN echo 'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX'
ENTRYPOINT ["/bin/bash", "/app/initialise.sh"]