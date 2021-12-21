using Tools;

AppComparer dc = new (args [0], args [1]);
if (args.Length > 2)
	dc.Output = new StreamWriter (args [2]);
dc.Compare ();
dc.Output.Flush ();