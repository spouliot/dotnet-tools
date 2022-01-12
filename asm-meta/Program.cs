using Mono.Cecil;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace AssemblyMetadata {

	enum Level {
		Assembly,
		Type,
		Member,
	}

	enum Output {
		AnsiOnly,
		AnsiAndTextFile,
		ANsiAndHtmlFile,
	}

	static class Program {

		static readonly DefaultAssemblyResolver resolver = new ();
		static readonly List<AssemblyDefinition> assemblies = new ();
		static readonly HashSet<string> names = new ();
		static Output Output { get; set; }

		static int Main (string [] args)
		{
			bool references = true;
			Level level = Level.Assembly;
			switch (args [0].ToLowerInvariant ()) {
			case "ad":
				references = false;
				break;
			case "ar":
				break;
			case "td":
				references = false;
				level = Level.Type;
				break;
			case "tr":
				level = Level.Type;
				break;
			case "md":
				references = false;
				level = Level.Member;
				break;
			case "mr":
				level = Level.Member;
				break;
			default:
				Console.WriteLine ("Missing argument: (a)ssembly | (t)ype | (m)ember references + (d)efinitions | (r)eferences");
				return 1;
			}

			if (args.Length > 3) {
				switch (args [2]) {
				case "--text":
					Output = Output.AnsiAndTextFile;
					break;
				case "--ansi":
					Output = Output.ANsiAndHtmlFile;
					break;
				}
			} else {
				Output = Output.AnsiOnly;
			}

			var exe = Path.GetFullPath (args [1]);
			resolver.AddSearchDirectory (Path.GetDirectoryName (exe));
			var ad = AssemblyDefinition.ReadAssembly (exe);
			assemblies.Add (ad);
			for (int i = 0; i < assemblies.Count; i++) {
				var asm = assemblies [i];
				var name = asm.MainModule.ToString ();
				if (names.Contains (name))
					continue;
				names.Add (name);
				foreach (var ar in asm.MainModule.AssemblyReferences) {
					var a = resolver.Resolve (AssemblyNameReference.Parse (ar.Name));
					if (!assemblies.Contains (a))
						assemblies.Add (a);
				}
			}

			if (Output != Output.AnsiOnly)
				AnsiConsole.Record ();

			// show the entry point (.exe) first
			if (references)
				ShowReferences (level, ad.MainModule);
			else
				ShowDefinitions (level, ad.MainModule);
			assemblies.Remove (ad);
			foreach (var assembly in assemblies.OrderBy ((arg) => arg.FullName)) {
				if (references)
					ShowReferences (level, assembly.MainModule);
				else
					ShowDefinitions (level, assembly.MainModule);
			}

			if (Output != Output.AnsiOnly) {
				string fname = args [3];
				string output = Output == Output.AnsiAndTextFile ? AnsiConsole.ExportText () : AnsiConsole.ExportHtml ();
				File.WriteAllText (fname, output);
			}

			return 0;
		}

		public sealed class DiffFriendlyTreeGuide : TreeGuide
		{
			public override string GetPart (TreeGuidePart part)
			{
				return part switch
				{
					TreeGuidePart.Space => "  ",
					TreeGuidePart.Continue => "  ",
					TreeGuidePart.Fork => "* ",
					TreeGuidePart.End => "* ",
					_ => throw new ArgumentOutOfRangeException (nameof (part), part, "Unknown tree part."),
				};
			}
		}

		static void ShowReferences (Level level, ModuleDefinition md)
		{
			Tree atree = new ($"[bold]A:[/] {md.Assembly.FullName}");
			atree.Style = new (Color.Blue);
			if (Output != Output.AnsiOnly)
				atree.Guide = new DiffFriendlyTreeGuide ();

			if (md.HasAssemblyReferences) {
				foreach (var ar in md.AssemblyReferences.OrderBy ((arg) => arg.ToString ())) {
					var arnode = atree.AddNode ($"[bold]AR:[/] {ar}");
					if (level > Level.Assembly) {
						var type_refs = md.GetTypeReferences ();
						foreach (var tr in type_refs.Where ((arg) => arg.Scope.ToString () == ar.FullName).OrderBy ((arg) => arg.FullName)) {
							var trnode = arnode.AddNode ($"[bold]TR:[/] {tr}");
							if (level > Level.Type) {
								var member_refs = md.GetMemberReferences ();
								foreach (var mr in member_refs.Where ((arg) => arg.DeclaringType.FullName == tr.FullName).OrderBy ((arg) => arg.FullName))
									trnode.AddNode ($"[bold]MR:[/] {mr.Beautify ()}");
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

		static void ShowDefinitions (Level level, ModuleDefinition md)
		{
			Tree atree = new ($"[bold]A:[/] {md.Assembly.FullName}");
			atree.Style = new (Color.Blue);
			if (Output != Output.AnsiOnly)
				atree.Guide = new DiffFriendlyTreeGuide ();

			if (level > Level.Assembly) {
				foreach (var td in md.Types.OrderBy ((arg) => arg.FullName)) {
					var trnode = atree.AddNode ($"[bold]TD:[/] {td}");
					if (level > Level.Type) {
						foreach (var m in td.Methods.OrderBy ((arg) => arg.FullName))
							trnode.AddNode ($"[bold]MD:[/] {m.Beautify ()}");
					}
				}
			}
			AnsiConsole.Write (atree);
			AnsiConsole.WriteLine ();
		}

		static string Beautify (this MemberReference mr)
		{
			var name = mr.FullName;
			var ctor_pos = name.IndexOf ("::.ctor(");
			if (ctor_pos != -1) {
				name = name [(ctor_pos + 2)..];
			} else {
				var cctor_pos = name.IndexOf ("::.cctor(");
				if (cctor_pos != -1)
					name = name [(cctor_pos + 2)..];
				else
					name = name.Replace (mr.DeclaringType.FullName + "::", String.Empty);
			}
			return name.EscapeMarkup ();
		}
	}
}
