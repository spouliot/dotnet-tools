# Assembly Definitions

This tool recursively shows references between assemblies, starting with a root one.

## Usage

```shell
dotnet run level assembly.dll
```

where `level` is

* `ad` for assembly definitions
* `td` for type definitions
* `md` for member definitions

Output ir sorted so it's easier to `diff` between different versions of assemblies or applications.


## Example

Show assembly references from main `MySingleView.dll`

```shell
$ dotnet run ar MySingleView.app/MySingleView.dll
A: MySingleView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
   AR: System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
   AR: Xamarin.iOS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065
A: System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
   -
A: Xamarin.iOS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065
   AR: System.Collections.NonGeneric, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
   AR: System.Linq, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
   AR: System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
   AR: System.Runtime.InteropServices, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
```
