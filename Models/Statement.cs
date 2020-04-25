using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LoS.Enums;
using LoS.Helpers;

namespace LoS.Models
{
	public class Statement
	{

		public List<Predicate> Predicates { get; set; }
		public Predicate Result { get; set; }

		public Statement() { Predicates = new List<Predicate>(); }

		public static Statement FromString(string line)
		{
			if (string.IsNullOrWhiteSpace(line)) return null;

			Statement res = new Statement();

			string[] args = line.Split(new string[] { "->", "=>" }, StringSplitOptions.RemoveEmptyEntries);

			if (args.Length != 2) return null;

			//string[] predicates = args[0].Split(new string[] { "AND", "and", "And" }, StringSplitOptions.RemoveEmptyEntries);

			//foreach (string str in predicates)
			//	res.Predicates.Add(Predicate.FromString(str.Trim()));

			res.Predicates = ParsePredicates(args[0]);

			res.SetResult(Predicate.FromString(args[1].Trim()));

			return res;

		}

		private static List<Predicate> ParsePredicates(string leftText)
		{
			leftText = leftText.Trim();
			if (string.IsNullOrWhiteSpace(leftText)) return null;

			List<Predicate> result = new List<Predicate>();
			Predicate predicate = new Predicate();

			int length = leftText.Length;

			while(length > 0)
			{

				if (leftText.ToUpper().StartsWith("AND") || leftText.ToUpper().StartsWith("OR"))
				{
					if (!string.IsNullOrEmpty(predicate.Name))
					{
						result.Add(predicate);
						predicate = new Predicate();
					}

					string operation = leftText.Substring(0, 3).Trim().ToUpper();
					switch (operation)
					{
						case "AND":
							predicate.Operation = Operation.And;
							break;
						case "OR":
							predicate.Operation = Operation.Or;
							break;
					}

					leftText = leftText.Substring(3).Trim();
					length = leftText.Length;

					//continue;
				}

				int indexFirstClosingScope = leftText.IndexOf(")");
				string tmpStringPredicate = leftText.Substring(0, indexFirstClosingScope + 1);
				leftText = leftText.Substring(indexFirstClosingScope + 1).Trim();
				length = leftText.Length;

				var tmpOperation = predicate.Operation;
				predicate = Predicate.FromString(tmpStringPredicate);
				predicate.Operation = tmpOperation;

				result.Add(predicate);

				predicate = new Predicate();

			}

			return result;
		}

		public override string ToString()
		{
			return string.Join(" ", Predicates) + $" -> {Result}";

			//string result = "";
			//foreach (Predicate p in Predicates)
			//	result += p.ToString() + " AND ";

			//if (result.Length >= 4) result = result.Substring(0, result.Length - 5);
			//if (Result != null) result += " -> " + Result.ToString();

			//return result;
		}

	}
}