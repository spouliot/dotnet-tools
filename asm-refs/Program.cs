using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace AssemblyReferences {

	enum ReferenceLevel {
		AssemblyReferences,
		TypeReferences,
		MemberReferences,
	}

	class Program {

		static DefaultAssemblyResolver resolver = new DefaultAssemblyResolver ();
		static List<AssemblyDefinition> assemblies = new List<AssemblyDefinition>();
		static HashSet<string> names = new HashSet<string>();

		static int Main (string[] args)
		{
			ReferenceLevel level = ReferenceLevel.AssemblyReferences;
			switch (args [0].ToLowerInvariant ()) {
			case "a":
			case "ar":
				break;
			case "t":
			case "tr":
				level = ReferenceLevel.TypeReferences;
				break;
			case "m":
			case "mr":
				level = ReferenceLevel.MemberReferences;
				break;
			default:
				Console.WriteLine ("Missing level argument: (a)ssembly, (t)ype, (m)ember references");
				return 1;
			}

			var exe = Path.GetFullPath (args [1]);
			resolver.AddSearchDirectory(Path.GetDirectoryName (exe));
			var ad = AssemblyDefinition.ReadAssembly (exe);
			assemblies.Add (ad);
			for (int i = 0; i < assemblies.Count; i++) {
				var name = ad.MainModule.ToString ();
				if (names.Contains (name))
					continue;
				names.Add (name);
				foreach (var ar in assemblies[i].MainModule.AssemblyReferences) {
					var a = resolver.Resolve (AssemblyNameReference.Parse (ar.Name));
					if (!assemblies.Contains (a))
						assemblies.Add (a);
				}
			}
			// show the entry point (.exe) first
			ShowModule (level, ad.MainModule);
			assemblies.Remove (ad);
			foreach (var assembly in assemblies.OrderBy ((arg) => arg.FullName))
				ShowModule (level, assembly.MainModule);
			return 0;
		}

		static void ShowModule (ReferenceLevel level, ModuleDefinition md)
		{
			Console.WriteLine ($"A: {md.Assembly.FullName}");
			if (md.HasAssemblyReferences) {
				foreach (var ar in md.AssemblyReferences.OrderBy ((arg) => arg.ToString ())) {
					Console.WriteLine ($"   AR: {ar}");
					if (level > ReferenceLevel.AssemblyReferences) {
						var type_refs = md.GetTypeReferences ();
						foreach (var tr in type_refs.Where ((arg) => arg.Scope.ToString () == ar.FullName).OrderBy ((arg) => arg.FullName)) {
							Console.WriteLine ($"      TR: {tr}", tr);
							if (level > ReferenceLevel.TypeReferences) {
								var member_refs = md.GetMemberReferences ();
								foreach (var mr in member_refs.Where ((arg) => arg.DeclaringType.FullName == tr.FullName).OrderBy ((arg) => arg.FullName))
									Console.WriteLine ($"          MR: {mr}", mr);
							}
						}
					}
				}
			} else {
				Console.WriteLine ("   -");
			}

			// Console.WriteLine ("   Type References");
			// foreach (var tr in md.GetTypeReferences ().OrderBy ((arg) => arg.FullName))
			// 	Console.WriteLine ("   > {0}", tr);
			// Console.WriteLine ("   Member References");
			// foreach (var mr in md.GetMemberReferences ().OrderBy ((arg) => arg.DeclaringType.FullName + "::" + arg.Name + "::" + arg.ToString ()))
			// 	Console.WriteLine ("   > {0}", mr);
		}
	}
}
