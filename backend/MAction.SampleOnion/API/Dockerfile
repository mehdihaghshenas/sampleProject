#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MAction.SampleOnion/API/MAction.SampleOnion.API.csproj", "MAction.SampleOnion/API/"]
COPY ["BaseProjects/AspNetIdentity/MAction.AspNetIdentityInfrastructure/MAction.AspNetIdentity.Infrastructure.csproj", "BaseProjects/AspNetIdentity/MAction.AspNetIdentityInfrastructure/"]
COPY ["BaseProjects/AspNetIdentity/MAction.AspNetIdentityBase/MAction.AspNetIdentity.Base.csproj", "BaseProjects/AspNetIdentity/MAction.AspNetIdentityBase/"]
COPY ["BaseProjects/AspNetIdentity/MAction.MongoAspNetIdentity/MAction.AspNetIdentity.Mongo.csproj", "BaseProjects/AspNetIdentity/MAction.MongoAspNetIdentity/"]
COPY ["BaseProjects/MAction.BaseMongoRepository/MAction.BaseMongoRepository.csproj", "BaseProjects/MAction.BaseMongoRepository/"]
COPY ["BaseProjects/MAction.BaseClasses/MAction.BaseClasses.csproj", "BaseProjects/MAction.BaseClasses/"]
COPY ["BaseProjects/MAction.WebHelpers/MAction.WebHelpers.csproj", "BaseProjects/MAction.WebHelpers/"]
COPY ["MAction.SampleOnion/Infrastructure/MAction.SampleOnion.Infrastructure.csproj", "MAction.SampleOnion/Infrastructure/"]
COPY ["MAction.SampleOnion/Service/MAction.SampleOnion.Service.csproj", "MAction.SampleOnion/Service/"]
COPY ["MAction.SampleOnion/Repository/MAction.SampleOnion.Repository.csproj", "MAction.SampleOnion/Repository/"]
COPY ["MAction.SampleOnion/Domain/MAction.SampleOnion.Domain.csproj", "MAction.SampleOnion/Domain/"]
COPY ["BaseProjects/MAction.BaseEFRepository/MAction.BaseEFRepository.csproj", "BaseProjects/MAction.BaseEFRepository/"]
COPY ["BaseProjects/MAction.BaseServices/MAction.BaseServices.csproj", "BaseProjects/MAction.BaseServices/"]
RUN dotnet restore "MAction.SampleOnion/API/MAction.SampleOnion.API.csproj"
COPY . .
WORKDIR "/src/MAction.SampleOnion/API"
RUN dotnet build "MAction.SampleOnion.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MAction.SampleOnion.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MAction.SampleOnion.API.dll"]