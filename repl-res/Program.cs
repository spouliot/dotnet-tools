using Mono.Cecil;

AssemblyDefinition ad = AssemblyDefinition.ReadAssembly (args [0]);
EmbeddedResource er = new (args [1], ManifestResourceAttributes.Public, File.ReadAllBytes (args [1]));
var mm = ad.MainModule;
if (mm.HasResources) {
	for (int i = 0; i < mm.Resources.Count; i++) {
		if (mm.Resources [i].Name == er.Name) {
			mm.Resources.RemoveAt (i);
			break;
		}
	}
}
mm.Resources.Add (er);
ad.Write (args [0]);
