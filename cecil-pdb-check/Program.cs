using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

var p = new ReaderParameters () {
	ReadSymbols = true,
	SymbolReaderProvider = new DefaultSymbolReaderProvider (),
	ThrowIfSymbolsAreNotMatching = true,
};
var writer = Console.Out;
foreach (var arg in args) {
	var ad = AssemblyDefinition.ReadAssembly (arg, p);
	Console.WriteLine ($"# {ad.Name.Name}");
	foreach (var module in ad.Modules) {
		foreach (var type in ad.MainModule.Types)
			Process (type);
	}
}

static void Process (TypeDefinition type)
{
	if (type.HasNestedTypes) {
		foreach (var nested in type.NestedTypes)
			Process (nested);
	}
	if (type.HasMethods) {
		foreach (var method in type.Methods) {
			try {
				_ = method.Body;
			}
			catch (Exception e) {
				Console.WriteLine ($"## {method.DeclaringType} / {method}");
				Console.WriteLine ($"```\n{e}\n```\n");
			}
		}
	}
}
