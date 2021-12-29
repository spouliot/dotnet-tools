# Assembly References

This tool recursively shows references between assemblies, starting with a root one.

## Usage

```shell
dotnet run level assembly.dll
```

where `level` is

* `ar` for assembly references
* `tr` for type references
* `mr` for member references

Output is sorted so it's easier to `diff` between different versions of assemblies or applications.


## Example

Show assembly references from main `MySingleView.dll`

```shell
$ dotnet run ar MySingleView.app/MySingleView.dll
A: MySingleView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
├── AR: System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
└── AR: Xamarin.iOS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065

A: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e

A: System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
└── AR: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e

A: Xamarin.iOS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065
└── AR: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
```
