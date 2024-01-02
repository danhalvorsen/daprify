# Daprify

## Description

This CLI (Command Line Interface) tool facilitates the creation of components, configuration, certificates, and Docker files necessary for integrating Dapr into your project. It automates the creation of essential files required for Dapr deployment.

Understanding the capabilities of Dapr and the components needed for your project is crucial before using this tool. This knowledge helps streamline your project's integration with Dapr. However, this CLI can also assist with generating additional files as new requirements emerge.

Read more at **[Dapr Resources](#additional-resources)**

## Usage

Run the available commands in your terminal. Outputs are saved to **git_root/Dapr/** (if a git root exists) or **CLI/Dapr/** otherwise. Copy the created directory into your project's root directory and do run **docker-compose up -d** inside Dapr/Docker.

Define your project requirements in the `config.json` file located in `CLI/Commands` and use the command:

### General Usage

```bash
dotnet run -- [command] [options]
```

To get the help in terminal use:

```bash
dotnet run -- --help

Description:
  CLI for creating configuration files for Dapr services.

Usage:
  Commands [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  gen_all         Generates everything needed to use Dapr with docker-compose (components, config, certificates and docker-compose file(s))
                  If all your services has a PackageReference to Dapr use --solution_paths instead of --services.
                  Use dotnet run gen_all -- -help to see all available options.

                  Examples:
                    dotnet run gen_all --components rabbitmq redis --services ServiceA ServiceB --settings mtls tracing --solution_paths ../../Backend
  gen_certs       Generates certificates needed for the Dapr sidecars.

                  Examples:
                    dotnet run gen_certs
  gen_config      Generates configuration file used by every Dapr service

                  Examples:
                    dotnet run gen_config
                    dotnet run gen_config --settings mtls tracing
  gen_components  Generates the desired Dapr components YAML files.

                  Examples:
                    dotnet run gen_components --components statestore
                    dotnet run gen_components --components statestore pubsub
  gen_dockerfiles  Generates dockerfiles for all your projects in the specified solutions.

                   Examples:
                     dotnet run gen_dockerfiles --solution_paths ../../Service1 ../../Service2
  gen_compose     Generates docker-compose.yml from your Dapr directory or for the specified service(s).

                  Examples:
                    dotnet run gen_compose --components rabbitmq sentry
                    dotnet run gen_compose --settings mtls https --services ServiceA
                    dotnet run gen_compose --solution_paths ../../Backend
                    dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend --solution_paths ../../Backend
  gen_puml        Generates a plantuml diagram of the supplied micro service spec.

                  Example:
                    dotnet run gen_puml -i test.mss
                    dotnet run gen_puml -i test.mss -o out.plantuml
  gen_mss         Generates a solution from the supplied micro service spec.

                  Example:
                    dotnet run gen_mss -i test.mss
                    dotnet run gen_mss -i test.mss -o ./out
```

### Generate All

Generates everything needed to use Dapr with docker-compose including components, config, certificates, and docker-compose files. Use --solution_paths instead of --services if all your services have a PackageReference to Dapr.

```bash
dotnet run gen_all -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```bash
Description:
  Generates everything needed to use Dapr with docker-compose (components, config, certificates and docker-compose file(s))
  Easiest to specify all your project needs in at CLI/Commands/config.json and execute with --config config.json
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
  --se, --services <services>                                                         The specific service(s) to generate docker-compose content to.
  --s, --settings <https|logging|metric|middleware|mtls|tracing>                      Additional settings for your services.
  --sp, --solution_paths <solution_path>                                               Path to your .Net sln file (from executing path). Adds all projects dependent on Dapr to docker-compose.
  -?, -h, --help                                                                      Show help and usage information
```

### Generate certificates

To secure your Dapr sidecars with mTLS, generate ca.crt, issuer.crt and issuer.key used by all the services, ensuring they can communicate securely. The sidecars are also dependent to have communication with the dapr sentry service.

```bash
dotnet run gen_certs -- [options]
```

```bash
dotnet run gen_certs -- --help

Description:
  Generates certificates needed for the Dapr sidecars.

  Examples:
    dotnet run gen_certs

Usage:
  Commands gen_certs [options]

Options:
  -?, -h, --help  Show help and usage information
```

To generate certificates for all services use:

```bash
dotnet run gen_certs
```

### Generate component files

Generate necessary component files for your Dapr project. These files, once created, must be mounted to the respective sidecars and the component needs to have a server running.

```bash
dotnet run gen_components -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```bash
dotnet run gen_components -- --help

Description:
  Generates the desired Dapr components YAML files.

  Examples:
    dotnet run gen_components --components statestore
    dotnet run gen_components --components statestore pubsub

Usage:
  Commands gen_components [options]

Options:
  --c, --components <bindings|configstore|crypto|lock|pubsub|secretstore|statestore>  The specific components(s) to generate config files for. [default: pubsub|statestore]
  -?, -h, --help                                                                      Show help and usage information
```

To generate specific component(s) file use:

```bash
dotnet run gen_certs -- components statestore pubsub
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

```bash
dotnet run gen_config -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```bash
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
  -?, -h, --help                                            Show help and usage information
```

To generate specific component(s) file use:

```bash
dotnet run gen_config --settings mtls tracing
```

If there is need for other settings in your configuration file or editing the generated one, see more at: **[Dapr Config Reference](https://docs.dapr.io/operations/configuration/configuration-overview/)**

## Generate dockerfiles

This command generates dockerfiles for your .Net project. The dockerfiles are needed in the docker-compose.yml. In case of special needs in the dockerfiles, add these after they are generated. e.g if you have a option.json file needed inside the docker container.

```bash
dotnet run gen_dockerfiles -- [options]
```

```bash
Description:
  Generates dockerfiles for all your projects in the specified solutions.

  Examples:
    dotnet run gen_dockerfiles --solution_paths ../../Service1 ../../Service2

Usage:
  Commands gen_dockerfiles [options]

Options:
  --pp, --project_path <project_path>    The path to the root of your project (from executing path). Not needed if it's a git project. Used to find correct paths.
  --solution_paths, --sp <solution_path>  The path to your .Net solution file (from executing path). Used to generate dockerfile for all projects in the sln file.
  -?, -h, --help                         Show help and usage information
```

## Generate docker-compose file

If you are going to host your project with the sidecars in a docker container you can generate the docker-compose-yml file for the specified services and components you have in your project. If a docker-compose file already exists at the location it is written to, it will append the services and components to the file instead of overwriting it.

```bash
dotnet run gen_compose -- [options]
```

Find available arguments for the options **[Here](#available-options-and-arguments)**

```bash
dotnet run gen_compose -- --help

Description:
  Generates docker-compose.yml from your Dapr directory or for the specified service(s).

  Examples:
    dotnet run gen_compose --components rabbitmq sentry
    dotnet run gen_compose --settings mtls https --services ServiceA
    dotnet run gen_compose --solution_paths ../../Backend
    dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend --solution_paths ../../Backend

Usage:
  Commands gen_compose [options]

Options:
  --c, --components <dashboard|placement|rabbitmq|redis|sentry|zipkin>  The specific component(s) to generate docker-compose content to. [default: dashboard|placement|zipkin]
  --pp, --project_path <project_path>                                   The path to your project's root from the current execution location. Unnecessary for Git projects; used for path resolution.
  --s, --settings <settings>                                            Additional settings for your services.
  --se, --services <services>                                           The specific service(s) to generate docker-compose content to.
  --sp, --solution_paths <solution_path>                                 The path to your .Net solution file (from executing path). Used to generate certificates for all services dependent on Dapr.
  -?, -h, --help                                                        Show help and usage information
```

To generate a docker-compose-yml with specific components and services:

```bash
dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend
```

## Available options and arguments

| Command          | Option       | Available Arguments                                                  |
| ---------------- | ------------ | -------------------------------------------------------------------- |
| `gen_all`        | `components` | dashboard, placement, rabbitmq, redis, sentry, zipkin                |
|                  | `settings`   | logging, metric, middleware, mtls, tracing, https                    |
| `gen_certs`      |              |                                                                      |
| `gen_config`     | `settings`   | logging, metric, middleware, mtls, tracing, https                    |
| `gen_components` | `components` | bindings, configstore, crypto, lock, pubsub, secretstore, statestore |
| `gen_compose`    | `components` | dashboard, placement, rabbitmq, redis, sentry, zipkin                |
|                  | `settings`   | https, logging, metric, middleware, mtls, tracing, https             |

## Additional Resources

For more detailed guidance and advanced usage scenarios, please refer to the following resources:

- **[Dapr Documentation](https://docs.dapr.io/)**
- **[Dapr Building Blocks](https://docs.dapr.io/concepts/building-blocks-concept/)**
- **[Dapr Components](https://docs.dapr.io/concepts/components-concept/)**
- **[Dapr Configuration](https://docs.dapr.io/concepts/configuration-concept/)**
- **[Dapr Security](https://docs.dapr.io/concepts/security-concept/)**
- **[Dapr GitHub Repository](https://github.com/dapr/dapr)**
- **[Dapr Community and Support](https://github.com/dapr/community)**
