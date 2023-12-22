using CLI.Models;

namespace CLI.Templates
{
    public class DockerfileTemplate() : HandlebarTemplate(TEMPLATE_STR)
    {
        private const string TEMPLATE_STR =
@"FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Copy the application source
COPY . ./

# Restore, build, and publish the application
RUN dotnet restore ""{{ServicePath}}""
RUN dotnet build ""{{ServicePath}}"" -c Release -o /app/build
RUN dotnet publish ""{{ServicePath}}"" -c Release -o /app/publish

# Use the ASP.NET runtime image for the final application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the built application from the build-env to the final image
COPY --from=build-env /app/publish .

# Set the entry point for the container
ENTRYPOINT [""dotnet"", ""{{ServiceName}}.dll""]";

        protected override string TemplateString => TEMPLATE_STR;

        public string Render(MyPath servicePath, Name serviceName)
        {
            var data = new
            {
                ServicePath = servicePath.ToString(),
                ServiceName = serviceName.ToString(),
            };
            return _template(data);
        }
    }
}