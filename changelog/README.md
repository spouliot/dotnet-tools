
## Usage

### Using a Pull-Request

```
dotnet run https://github.com/xamarin/xamarin-macios/pull/11175
```

[Markdown Output](https://gist.github.com/spouliot/6a56d6527b750635283d1e8e7c78eaf9)

### Filtering repositories

Too noisy ? If you're only looking for changes in some repos then mention them!

```
dotnet run https://github.com/xamarin/xamarin-macios/pull/11175 dotnet/installer dotnet/runtime dotnet/sdk
```

[Markdown Output](https://gist.github.com/spouliot/64252176f21ba53939df31555368b4a1)

### Without a Pull-Request

No Pull-Request ? Create a local diff and use it!

```
$ git diff eng/Version.Details.xml > ~/local.diff
$ cat ~/local.diff
diff --git a/eng/Version.Details.xml b/eng/Version.Details.xml
index 46cbef586..2303a84b3 100644
--- a/eng/Version.Details.xml
+++ b/eng/Version.Details.xml
@@ -1,8 +1,8 @@
 <Dependencies>
   <ProductDependencies>
-    <Dependency Name="Microsoft.Dotnet.Sdk.Internal" Version="6.0.100-preview.4.21215.12">
+    <Dependency Name="Microsoft.Dotnet.Sdk.Internal" Version="6.0.100-preview.4.21217.1">
       <Uri>https://github.com/dotnet/installer</Uri>
-      <Sha>c1eb61ac8152d6777b0d685e37b32885fd37775d</Sha>
+      <Sha>75a31f792b64b4c55699f11664ee31adcb0b7793</Sha>
     </Dependency>
     <Dependency Name="Microsoft.NET.ILLink" Version="6.0.100-preview.2.21212.1">
       <Uri>https://github.com/mono/linker</Uri>
$ dotnet run ~/local.diff
```

[Markdown Output](https://gist.github.com/spouliot/e0ca48aeaa3d5a69328d9ed2ca8a3d1f)
