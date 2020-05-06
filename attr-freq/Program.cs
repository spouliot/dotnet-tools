using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;

namespace AttributesFrequency {
	class MainClass {
		static List<string> assemblies = new List<string> ();
		static SortedDictionary<string, int> attributes = new SortedDictionary<string, int> ();

		public static int Main (string[] args)
		{
			string path = args [0];

			if (Directory.Exists (path)) {
				foreach (var file in Directory.GetFiles (path, "*.dll"))
					Process (file);
			} else if (File.Exists (path)) {
				Process (path);
			} else {
				return 1;
			}

			Console.WriteLine ($"# {Path.GetFileName (path)}");
			if (assemblies.Count > 0) {
				foreach (var assembly in assemblies.OrderBy (x => x))
					Console.WriteLine ($"* {Path.GetFileNameWithoutExtension (assembly)}");
			}
			Console.WriteLine ();

			Console.WriteLine ("# Alphabetical Order");
			Console.WriteLine ("|Attribute Name|Frequency|");
			Console.WriteLine ("|--------------|--------:|");
			foreach (var t in attributes)
				Console.WriteLine ($"|{t.Key} | {t.Value} |");
			Console.WriteLine ();

			Console.WriteLine ("# Frequency Order");
			Console.WriteLine ("|Frequency|Attribute Name|");
			Console.WriteLine ("|--------:|--------------|");
			foreach (var t in attributes.OrderBy (x => x.Value).Reverse ())
				Console.WriteLine ($"| {t.Value} | {t.Key} |");
			Console.WriteLine ();

			return 0;
		}

		static void Add (ICustomAttributeProvider cap)
		{
			if (!cap.HasCustomAttributes)
				return;
			foreach (var ca in cap.CustomAttributes) {
				var cat = ca.AttributeType.FullName;
				if (!attributes.ContainsKey (cat))
					attributes.Add (cat, 1);
				else
					attributes [cat] = attributes [cat] + 1;
			}
		}

		static void Process (string file)
		{
			assemblies.Add (file);
			var ad = AssemblyDefinition.ReadAssembly (file);
			Add (ad);
			foreach (var module in ad.Modules) {
				Add (module);
				foreach (var type in module.Types) {
					ProcessType (type);
				}
			}
		}

		static void ProcessType (TypeDefinition type)
		{
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (nested);
			}
			Add (type);
			if (type.HasMethods) {
				foreach (var m in type.Methods)
					Add (m);
			}
			if (type.HasProperties) {
				foreach (var p in type.Properties)
					Add (p);
			}
			if (type.HasEvents) {
				foreach (var e in type.Events)
					Add (e);
			}
			if (type.HasFields) {
				foreach (var f in type.Fields)
					Add (f);
			}
		}
	}
}
