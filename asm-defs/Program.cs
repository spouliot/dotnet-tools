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
			case "ad":
				break;
			case "t":
			case "td":
				level = ReferenceLevel.TypeReferences;
				break;
			case "m":
			case "md":
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
			if (level > ReferenceLevel.AssemblyReferences) {
				foreach (var td in md.Types.OrderBy ((arg) => arg.FullName)) {
					Console.WriteLine ($"      TD: {td}", td);
					if (level > ReferenceLevel.TypeReferences) {
						var member_refs = md.GetMemberReferences ();
						foreach (var m in td.Methods.OrderBy ((arg) => arg.FullName))
							Console.WriteLine ($"          MD: {m}", m);
					}
				}
			}
		}
	}
}
