using LoS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoS.Helpers
{
	public static class Holders
	{
		public static List<string> ToListArgs(this Predicate predicate)
			=> predicate.Args.ToList();

		public static List<List<String>> ToListArgs(this List<Predicate> predicates)
			=> predicates.Select(x => x.ToListArgs()).ToList();

		public static bool ContainsAtName(this List<Predicate> predicates, string name)
			=> predicates.Any(x => x.Name == name);
		
		/// <summary>
		/// Проверяет, есть ли указанный предикат в списке. Абсолютное сравнение : и по имени, и по аргументам
		/// </summary>
		public static bool ContainsAtAllProp(this List<Predicate> predicates, Predicate predicate)
			=> predicates
				.Where(x => x.Name == predicate.Name)
				// можно и по другому, но это самый простой способ сравнить два списка на абсолютное равенство
				.Any(x => string.Join(string.Empty, x.Args) == string.Join(string.Empty, predicate.Args));

		public static Predicate Copy(this Predicate predicate) 
			=> new Predicate(predicate.Name, predicate.Args);

		public static List<Predicate> Copy(this List<Predicate> predicates, string name = "")
			=> new List<Predicate>(
				predicates
					.Where(x => 
						name == string.Empty 
						|| 
						x.Name == name
						)
					.Select(x => x.Copy())
				);

		public static void SetResult(this Statement statement, Predicate predicate) 
			=> statement.Result = predicate.Copy();

		public static void Clear(this Statement st) 
			=> st.Predicates.Clear();

		public static void Add(this Statement st, Predicate p)
		{
			if (!st.Predicates.ContainsAtAllProp(p)) st.Predicates.Add(p);
		}

		public static bool Contains(this List<Statement> statements, Statement statement)
		{
			if (!statements.Any()) return false;

			bool allEquals = false;

			// TODO: рефакторинг
			foreach (Statement s in statements)
			{
				bool oneEqual = true;
				if ((statement.Predicates.Count == 1) && (s.Predicates.Count == 1))
				{
					foreach (Predicate p in statement.Predicates)
						if (!s.Predicates.ContainsAtName(p.Name)) oneEqual = false;
				}
				else
				{
					foreach (Predicate p in statement.Predicates)
						if (!s.Predicates.ContainsAtAllProp(p)) oneEqual = false;
				}
				allEquals = allEquals || oneEqual;
			}

			return allEquals;
		}

		public static bool ContainsAllPropPredicate(this Statement statement, Predicate predicate)
			=> statement.Predicates.Any(p => predicate.Args.All(arg => p.Args.Contains(arg)));

		public static bool IsSet(this List<List<string>> table, List<string> raw)
			=> table.Any(traw =>
			{
				// indexes are very important
				for (int i = 0; i < traw.Count; ++i)
				{
					if (traw[i] != raw[i]) return false;
				}

				return true;
			});

	}
}