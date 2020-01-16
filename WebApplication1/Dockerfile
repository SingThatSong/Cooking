#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-nanoserver-1809 AS build
WORKDIR /src
COPY ["Cooking.Web/Cooking.Web.csproj", "Cooking.Web/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "Cooking.Web/Cooking.Web.csproj"
COPY . .
WORKDIR "/src/Cooking.Web"
RUN dotnet build "Cooking.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Cooking.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cooking.Web.dll"]
