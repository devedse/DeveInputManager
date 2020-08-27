# Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
WORKDIR /source

# caches restore result by copying csproj file separately
#COPY /NuGet.config /source/
COPY /DeveInputManager/*.csproj /source/DeveInputManager/
COPY /DeveInputManager.ConsoleApp/*.csproj /source/DeveInputManager.ConsoleApp/
COPY /DeveInputManager.Tests/*.csproj /source/DeveInputManager.Tests/
COPY /DeveInputManager.sln /source/
RUN ls
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet build --configuration Release
RUN dotnet test --configuration Release ./DeveInputManager.Tests/DeveInputManager.Tests.csproj
RUN dotnet publish ./DeveInputManager.ConsoleApp/DeveInputManager.ConsoleApp.csproj --output /app/ --configuration Release

# Stage 2
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "DeveInputManager.ConsoleApp.dll"]