using Mono.Cecil;
using Spectre.Console;

namespace AssemblyReferences {

	enum ReferenceLevel {
		AssemblyReferences,
		TypeReferences,
		MemberReferences,
	}

	class Program {

		static readonly DefaultAssemblyResolver resolver = new ();
		static readonly List<AssemblyDefinition> assemblies = new ();
		static readonly HashSet<string> names = new ();

		static int Main (string [] args)
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
			resolver.AddSearchDirectory (Path.GetDirectoryName (exe));
			var ad = AssemblyDefinition.ReadAssembly (exe);
			assemblies.Add (ad);
			for (int i = 0; i < assemblies.Count; i++) {
				var name = ad.MainModule.ToString ();
				if (names.Contains (name))
					continue;
				names.Add (name);
				foreach (var ar in assemblies [i].MainModule.AssemblyReferences) {
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
			Tree atree = new ($"[bold]A:[/] {md.Assembly.FullName}");
			atree.Style = new (Color.Blue);
			if (md.HasAssemblyReferences) {
				foreach (var ar in md.AssemblyReferences.OrderBy ((arg) => arg.ToString ())) {
					var arnode = atree.AddNode ($"[bold]AR:[/] {ar}");
					if (level > ReferenceLevel.AssemblyReferences) {
						var type_refs = md.GetTypeReferences ();
						foreach (var tr in type_refs.Where ((arg) => arg.Scope.ToString () == ar.FullName).OrderBy ((arg) => arg.FullName)) {
							var trnode = arnode.AddNode ($"[bold]TR:[/] {tr}");
							if (level > ReferenceLevel.TypeReferences) {
								var member_refs = md.GetMemberReferences ();
								foreach (var mr in member_refs.Where ((arg) => arg.DeclaringType.FullName == tr.FullName).OrderBy ((arg) => arg.FullName))
									trnode.AddNode ($"[bold]MR:[/] {mr.FullName.EscapeMarkup ()}");
							}
						}
					}
				}
			} else {
				atree.AddNode ("");
			}
			AnsiConsole.Write (atree);
			AnsiConsole.WriteLine ();
		}
	}
}
