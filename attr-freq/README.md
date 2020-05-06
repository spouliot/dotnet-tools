# Attributes Frequency

Check which and how often custom attributes are used in one or more assemblies.

The tool outputs markdown so it's easier to read from a gist.

## Usage

```shell
dotnet run <assembly or path-to-assemblies>
```

## Example

```shell
dotnet run MyApp.app > out
gist out -t md -o
```
