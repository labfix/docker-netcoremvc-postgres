FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src

# Copy solution/csproj and restore as distinct layers
COPY /*.sln ./

RUN mkdir -p LabFix.NetcoreMVC

COPY /LabFix.NetcoreMVC/*.csproj LabFix.NetcoreMVC/

RUN dotnet restore

# Copy everything else and build
COPY . ./
WORKDIR /src/LabFix.NetcoreMVC
RUN dotnet publish -c Release -o /app

# Build runtime image
FROM microsoft/aspnetcore:2.0 AS base
RUN ln -fs /usr/share/zoneinfo/Asia/Bangkok /etc/localtime
RUN dpkg-reconfigure --frontend noninteractive tzdata

WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000