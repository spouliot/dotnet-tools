namespace Tools {
	
	public class Item {
		public string? Name { get; set; }
		public string? Directory { get; set; }
		public FileInfo? Info1 { get; set; }
		public FileInfo? Info2 { get; set; }
	}

	public abstract class DirectoryComparer {

		protected DirectoryComparer ()
		{
			Output = Console.Out;
			// it' up to the subclass to ensure this but the compiler complains
			Directory1 = null!;
			Directory2 = null!;
		}

		public DirectoryInfo Directory1 { get; protected set; }
		public DirectoryInfo Directory2 { get; protected set; }

		public TextWriter Output { get; set; }

		static string GetRootName (FileInfo info, int start)
		{
			return info.FullName [start..];
		}

		public virtual string GetKey (string rootedName)
		{
			return rootedName;
		}

		public void Compare ()
		{
			Dictionary<string,Item> items = new ();
			var s1 = Directory1.FullName.Length;
			foreach (var file in Directory1.GetFiles("*.*", SearchOption.AllDirectories)) {
				var fullname = GetRootName (file, s1);
				var key = GetKey(fullname);
				items.Add(key, new Item () {
					Directory = Path.GetDirectoryName (fullname),
					Name = Path.GetFileName (file.Name),
					Info1 = file,
				});
			}
			// process 2nd directory
			var s2 = Directory2.FullName.Length;
			foreach (var file in Directory2.GetFiles ("*.*", SearchOption.AllDirectories)) {
				var fullname = GetRootName (file, s2);
				var key = GetKey(fullname);
				if (!items.TryGetValue (key, out var item)) {
					item = new Item () {
						Directory = Path.GetDirectoryName (fullname),
						Name = Path.GetFileName (file.Name),
					};
					items.Add(key, item);
				}
				item.Info2 = file;
			}

			// display
			Start ();
			foreach (var kvp in items.OrderBy ((arg) => arg.Key)) {
				Process (kvp.Value);
			}
			End ();
		}

		public virtual void Start ()
		{
		}

		public abstract void Process (Item item);

		public virtual void End ()
		{
		}
	}
}
