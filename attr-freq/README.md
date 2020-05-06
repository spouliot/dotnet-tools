# Attributes Frequency

Check which and how often custom attributes are used in one or more assemblies.

The tool outputs markdown so it's easier to read from a gist.

## Usage

```shell
dotnet run <assembly or path-to-assemblies>
```

## Example

### Used on itself

```shell
dotnet run bin/Debug/net5.0/attr-freq.dll > out
gist out -t md -o
```

[Output](https://gist.github.com/spouliot/1efbdf07feaaf5462f1a33a06794890a)

### Used on a Xamarin.iOS application

```shell
dotnet run MyApp.app > out
gist out -t md -o
```

### Used on Xamarin.iOS SDK assemblies

```shell
dotnet run /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS > out
gist out -t md -o
```
