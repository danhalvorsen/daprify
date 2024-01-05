# Daprify

This CLI (Command Line Interface) tool facilitates the creation of certificates, components, configuration, dockerfiles and docker-compose files necessary for integrating Dapr into your project.

Daprify automates the creation of essential files required for Dapr deployment.

Understanding the capabilities of Dapr and the components needed for your project is preferred before using this tool. This knowledge helps streamline your project's integration with Dapr. However, this CLI can also assist with generating additional files as new requirements emerge.

Read more at **[Dapr Resources](#additional-resources)**

## Usage

1. Define your project requirements in the `config.json` file located in `Daprify/Commands`.
2. Run the available commands in your terminal.
3. If project_path or solution_paths is given the outputs are saved directly into your project's root in **<YOUR_PROJECT>/Dapr/**.
4. If not, they are saved in **Daprify/Dapr/**. Copy the **Dapr** directory into your own project's root directory.
5. Run **docker-compose up -d** inside Dapr/Docker.

### General Usage

```zsh
dotnet run -- [command] [options]
```

To get the help in terminal use:

```zsh
dotnet run -- --help

Description:
  CLI for creating all the necessary files for your Dapr project.
  This includes the certificates, config, components, dockerfiles and docker-compose file(s)
  Prefer to use gen_all command with specified options in Daprify/Commands/config.json

Usage:
  Commands [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  gen_all          Generates everything needed to use Dapr with docker (components, config, certificates, dockerfiles and docker-compose file(s))
                   Easiest to specify all your project needs in at Daprify/Commands/config.json and execute with --config config.json
                   If all your services has a PackageReference to Dapr use --solution_paths instead of --services.
                   Use dotnet run gen_all -- -help to see all available options.

                   Examples:
                     dotnet run gen_all --config config.json
                     dotnet run gen_all --components rabbitmq redis --services ServiceA ServiceB --settings mtls tracing --solution_paths ../../Backend
  gen_certs        Generates certificates needed for the Dapr sidecars and Sentry service.

                   Examples:
                     dotnet run gen_certs
  gen_config       Generates configuration file used by every Dapr service

                   Examples:
                     dotnet run gen_config
                     dotnet run gen_config --settings mtls tracing
  gen_components   Generates the desired Dapr components .yml files.

                   Examples:
                     dotnet run gen_components --components statestore
                     dotnet run gen_components --components statestore pubsub
  gen_dockerfiles  Generates dockerfiles for all your projects in the specified solutions.

                   Examples:
                     dotnet run gen_dockerfiles --solution_paths ../../Service1 ../../Service2
  gen_compose      Generates docker-compose.yml from your .Net solution(s) or for specified service(s).

                   Examples:
                     dotnet run gen_compose --components rabbitmq sentry
                     dotnet run gen_compose --settings mtls https --services ServiceA
                     dotnet run gen_compose --solution_paths ../../Backend
                     dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend --solution_paths ../../Backend
```

### Generate All

Generates everything needed to use Dapr with docker-compose including certificates, components, config, dockerfiles and docker-compose files.

Prefer to specify your project requirements in config.json and use the --config option. Use the solution_paths option instead of services if all your services have a PackageReference to Dapr.

```zsh
dotnet run gen_all -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```zsh
dotnet run gen_all -- --help

Description:
  Generates everything needed to use Dapr with docker (components, config, certificates, dockerfiles and docker-compose file(s))
  Easiest to specify all your project needs in at Daprify/Commands/config.json and execute with --config config.json
  If all your services has a PackageReference to Dapr use --solution_paths instead of --services.
  Use dotnet run gen_all -- -help to see all available options.

  Examples:
    dotnet run gen_all --config config.json
    dotnet run gen_all --components rabbitmq redis --services ServiceA ServiceB --settings mtls tracing --solution_paths ../../Backend

Usage:
  Commands gen_all [options]

Options:
  --config <config>                                                                   Path to your config file (from executing path).
  --c, --components <bindings|configstore|crypto|lock|pubsub|secretstore|statestore>  The specific component(s) to generate docker-compose content to. [default: statestore|secretstore]
  --pp, --project_path <project_path>                                                 The path to the root of your project (from executing path). Not needed if it's a git project. Used to
                                                                                      find correct paths.
  --se, --services <services>                                                         The specific service(s) to generate docker-compose content to.
  --s, --settings <https|logging|metric|middleware|mtls|tracing>                      Additional settings for your services.
  --solution_paths, --sp <solution_paths>                                             Path to your .Net sln file (from executing path). Adds all projects dependent on Dapr to docker-compose.
  --v, --verbose                                                                      Enable verbose logging
  -?, -h, --help                                                                      Show help and usage information
```

### Generate certificates

To secure your Dapr sidecars with mTLS, generate ca.crt, issuer.crt and issuer.key used by all the services, ensuring they can communicate securely. The sidecars are also dependent to have communication with the dapr sentry service.

```zsh
dotnet run gen_certs -- [options]
```

```zsh
dotnet run gen_certs -- --help

Description:
  Generates certificates needed for the Dapr sidecars and Sentry service.

  Examples:
    dotnet run gen_certs

Usage:
  Commands gen_certs [options]

Options:
  --v, --verbose  Enable verbose logging
  -?, -h, --help  Show help and usage information
```

To generate certificates for all services use:

```zsh
dotnet run gen_certs
```

### Generate component files

Generate necessary component files for your Dapr project. These files, once created, must be mounted to the respective sidecars and the component needs to have a server running. This will automatically happen in the generated docker-compose.

```zsh
dotnet run gen_components -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```zsh
dotnet run gen_components -- --help

Description:
  Generates the desired Dapr components .yml files.

  Examples:
    dotnet run gen_components --components statestore
    dotnet run gen_components --components statestore pubsub

Usage:
  Commands gen_components [options]

Options:
  --c, --components <bindings|configstore|crypto|lock|pubsub|secretstore|statestore>  The specific component(s) to generate a file for. [default: pubsub|statestore]
  --v, --verbose                                                                      Enable verbose logging
  -?, -h, --help                                                                      Show help and usage information
```

To generate specific component(s) file use:

```zsh
dotnet run gen_components -- components statestore pubsub
```

Available components are:

- bindings
- configstore
- crypto
- lock
- pubsub
- secretstore
- statestore

If there is need for other component files or editing the generated one, see more at: **[Dapr Components Reference](https://docs.dapr.io/reference/components-reference/)**

## Generate config file

If no configuration file is mounted to the sidecar, the dapr sidecar will use the default config.yml file. However, if you are going to use any features you need to mount a config file to the sidecar with the settings you are going to use.

```zsh
dotnet run gen_config -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```zsh
dotnet run gen_config -- --help

Description:
  Generates configuration file used by every Dapr service

  Examples:
    dotnet run gen_config
    dotnet run gen_config --settings mtls tracing

Usage:
  Commands gen_config [options]

Options:
  --s, --settings <logging|metric|middleware|mtls|tracing>  The specific setting(s) to add to the configuration
  --v, --verbose                                            Enable verbose logging
  -?, -h, --help                                            Show help and usage information
```

To generate specific component(s) file use:

```zsh
dotnet run gen_config --settings mtls tracing
```

If there is need for other settings in your configuration file or editing the generated one, see more at: **[Dapr Config Reference](https://docs.dapr.io/operations/configuration/configuration-overview/)**

## Generate dockerfiles

This command generates dockerfiles for your .Net project. The dockerfiles are needed in the docker-compose.yml. In case of special needs in the dockerfiles, add these after they are generated. e.g if you have a option.json file needed inside the docker container.

```zsh
dotnet run gen_dockerfiles -- [options]
```

```zsh
dotnet run gen_dockerfiles -- --help

Description:
  Generates dockerfiles for all your projects in the specified solutions.

  Examples:
    dotnet run gen_dockerfiles --solution_paths ../../Service1 ../../Service2

Usage:
  Commands gen_dockerfiles [options]

Options:
  --pp, --project_path <project_path>                 The path to the root of your project (from executing path). Not needed if it's a git project. Used to find correct paths.
  --solution_paths, --sp <solution_paths> (REQUIRED)  The path to your .Net solution file (from executing path). Used to generate dockerfile for all projects in the sln file.
  --v, --verbose                                      Enable verbose logging
  -?, -h, --help                                      Show help and usage information
```

## Generate docker-compose file

If you are going to host your project with the sidecars in a docker container you can generate the docker-compose.yml file for the specified services and components you have in your project. If a docker-compose file already exists at the location it is written to, it will append the services and components to the file instead of overwriting it.

```zsh
dotnet run gen_compose -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```zsh
dotnet run gen_compose -- --help

Description:
  Generates docker-compose.yml from your .Net solution(s) or for specified service(s).

  Examples:
    dotnet run gen_compose --components rabbitmq sentry
    dotnet run gen_compose --settings mtls https --services ServiceA
    dotnet run gen_compose --solution_paths ../../Backend
    dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend --solution_paths ../../Backend

Usage:
  Commands gen_compose [options]

Options:
  --c, --components <dashboard|placement|rabbitmq|redis|sentry|zipkin>  The specific component(s) to generate docker-compose content to. [default: dashboard|placement|zipkin]
  --s, --settings <https|mtls>                                          Additional settings for your services.
  --se, --services <services>                                           The specific service(s) to generate docker-compose content to.
  --solution_paths, --sp <solution_paths>                               The path to your .Net solution file (from executing path). Used to generate certificates for all services dependent
                                                                        on Dapr.
  --v, --verbose                                                        Enable verbose logging
  -?, -h, --help                                                        Show help and usage information
```

To generate a docker-compose-yml with specific components and services:

```zsh
dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend
```

## Available options and arguments

| Command           | Option           | Available Arguments                                                  |
| ----------------- | ---------------- | -------------------------------------------------------------------- |
| `gen_all`         | `config`         |                                                                      |
|                   | `components`     | dashboard, placement, rabbitmq, redis, sentry, zipkin                |
|                   | `project_path`   |                                                                      |
|                   | `services`       |                                                                      |
|                   | `settings`       | logging, metric, middleware, mtls, tracing, https                    |
|                   | `solution_paths` |                                                                      |
| `gen_certs`       |                  |                                                                      |
| `gen_config`      | `settings`       | logging, metric, middleware, mtls, tracing                           |
| `gen_components`  | `components`     | bindings, configstore, crypto, lock, pubsub, secretstore, statestore |
| `gen_dockerfiles` | `project_path`   |                                                                      |
|                   | `solution_paths` |                                                                      |
| `gen_compose`     | `components`     | dashboard, placement, rabbitmq, redis, sentry, zipkin                |
|                   | `settings`       | https, mtls,                                                         |
|                   | `services`       |                                                                      |
|                   | `solution_paths` |                                                                      |

## Additional Resources

For more detailed guidance and advanced usage scenarios, please refer to the following resources:

- **[Dapr Documentation](https://docs.dapr.io/)**
- **[Dapr Building Blocks](https://docs.dapr.io/concepts/building-blocks-concept/)**
- **[Dapr Components](https://docs.dapr.io/concepts/components-concept/)**
- **[Dapr Configuration](https://docs.dapr.io/concepts/configuration-concept/)**
- **[Dapr Security](https://docs.dapr.io/concepts/security-concept/)**
- **[Dapr GitHub Repository](https://github.com/dapr/dapr)**
- **[Dapr Community and Support](https://github.com/dapr/community)**
