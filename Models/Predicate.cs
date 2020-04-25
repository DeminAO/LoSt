using LoS.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LoS.Models
{
	public class Predicate
	{
		public Operation? Operation { get; set; }
		public string Name { get; set; }
		public string[] Args { get; set; }

		public Predicate(string name, IEnumerable<string> args, Operation? operation = null)
		{
			Name = name;
			Operation = operation;
			Args = args.ToArray();
		}

		public Predicate()
		{
			Name = string.Empty;
			Args = null;
			Operation = null;
		}

		public static Predicate FromString(string line)
		{
			if (string.IsNullOrWhiteSpace(line)) return null;
			int openScopeIndex = line.IndexOf('(') + 1;
			int closeScopeIndex = line.IndexOf(')');

			if (openScopeIndex < 1 || closeScopeIndex <= 3) return null;

			string name = line.Substring(0, openScopeIndex - 1);
			string[] args = line
				.Substring(openScopeIndex, closeScopeIndex - openScopeIndex)
				.Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

			var res = new Predicate(name, args);

			return res;
		}

		public override string ToString() => $"{(Operation == null ? string.Empty : Operation.ToString() + " " )}{Name}({string.Join(",", Args)})";

	}
}