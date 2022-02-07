# Application Comparer

> A newer version of this tool is available from it's [own repository](https://github.com/spouliot/appcompare). This version remains for historical purposes but is no longer maintained.

Compare the file sizes between two versions of the same app bundle.

The tool outputs markdown so it's easier to read from a gist.

## Usage

```shell
dotnet run <app1> <app2>
```

## Example

```shell
dotnet run MyOldApp.app MyNewApp.app > out
gist out -t md -o
```
