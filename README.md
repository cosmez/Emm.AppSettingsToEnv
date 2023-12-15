# AppSettingsToEnv
Add appsettings.json to user (or machine) environment variables.

## Installation

Install `appsettingstoenv` as a .NET global tool:

```bash
dotnet tool install --global Emm.AppSettingsToEnv
```

## Usage

```bash
# create environment variables for the user
appsettingstoenv --input appsettings.json

# create environment variables for the machine, requires elevation (sudo)
appsettingstoenv --input appsettings.json --level machine
```

