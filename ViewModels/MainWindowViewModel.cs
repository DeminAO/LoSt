using LoS.Helpers;
using LoS.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LoS.ViewModels
{
	class MainWindowViewModel : BindableBase
	{
		#region Props

		private const string defaultCodeText = "Предикаты:\n\nФакты:\n\nАксиомы:";
		private string codeText = defaultCodeText;
		public string CodeText
		{
			get => codeText;
			set => SetProperty(ref codeText, value);
		}
		
		private string logText;
		public string LogText
		{
			get => logText;
			set => SetProperty(ref logText, value);
		}

		private bool isLoading;
		public bool IsLoading
		{
			get => isLoading;
			set => SetProperty(ref isLoading, value);
		}

		public ICommand OpenFileCommand { get; private set; }
		public ICommand SaveFileCommand { get; private set; }
		public ICommand StartCommand { get; private set; }

#endregion Props

		#region Ctor

		public MainWindowViewModel()
		{
			StartCommand = new DelegateCommand(() => OnStartCommand());
			OpenFileCommand = new DelegateCommand(() => OnOpenFileCommand());
			SaveFileCommand = new DelegateCommand(() => OnSaveFileCommand());
		}

		#endregion Ctor

		#region Commands

		private async Task OnSaveFileCommand()
		{
			if (string.IsNullOrWhiteSpace(CodeText)) return;

			if (!DialogManager.SaveFileDialog(false, "All files (*.*)|*.*", out string fileName)) return;

			FileManager.CreateAndCloseFileIfNotExists(fileName);

			FileManager.WriteAllText(fileName, CodeText);
		}

		private async Task OnOpenFileCommand()
		{
			if (!DialogManager.OpenFileDialog(true, "AllFiles (*.*)|*.*", out string fileName)) return;

			if (!FileManager.GetIsFileExistsAndNotEmpty(fileName)) return;

			if (
				!string.IsNullOrWhiteSpace(CodeText) 
				&& 
				CodeText != defaultCodeText 
				&& 
				DialogManager.ShowApprovalDialog("Сохранить текущий файл?", "Сохранение изменений")
				)
			{
				await OnSaveFileCommand();
			}

			CodeText = FileManager.ReadAllText(fileName);
		}

		private async Task OnStartCommand()
		{
			IsLoading = true;

			#region logic

			LogText = string.Empty;

			if (string.IsNullOrWhiteSpace(CodeText)) return;

			List<Predicate> predicatesByCode = new List<Predicate>();
			List<Predicate> factsByCode = new List<Predicate>();
			List<Predicate> foundedFacts = new List<Predicate>();
			List<Statement> statementsByCode = new List<Statement>();

			ParseStringToCollections(CodeText, out statementsByCode, out predicatesByCode, out factsByCode);

			foundedFacts = await FindFactsAsync(statementsByCode, predicatesByCode, factsByCode);

			if (!foundedFacts.Any())
			{
				Log("Новых фактов нет");
			}
			else
			{
				Log($"Новые факты: {foundedFacts.Count}");
				Log(foundedFacts.Select(x => $"\t{x}"));
			}

			#endregion logic

			IsLoading = false;
		}

		#endregion Commands

		#region Helpers

		private List<Predicate> FindFacts(IEnumerable<Statement> statements, IEnumerable<Predicate> predicates, IEnumerable<Predicate> facts)
		{
			List<Predicate> result = new List<Predicate>();



			return result;
		}

		private async Task<List<Predicate>> FindFactsAsync(List<Statement> statements, List<Predicate> predicates, List<Predicate> facts)
		{
			List<Predicate> foundedFacts = new List<Predicate>();

			bool wasChanged = true;

			Log("\n\nПоиск новых фактов:\n");

			while (wasChanged)
			{
				wasChanged = false;

				foreach (Statement currentStatement in statements)
				{
					Log(currentStatement + ": ");

					List<string> currentStatementArgs = currentStatement.Predicates.SelectMany(x => x.Args).Distinct().ToList();

					//List<string> currentStatementArgs = new List<string>();

					//foreach (Predicate codeStatementPadicate in currentStatement.Predicates)
					//{
					//	currentStatementArgs
					//		.AddRange(
					//			codeStatementPadicate.Args
					//				.Where(x => !currentStatementArgs.Contains(x))
					//			);

					//}

					List<List<string>> factsArgsTableByStatement = null;

					foreach (Predicate codeStatementPredicate in currentStatement.Predicates)
					{
						List<Predicate> listByFactsAll = facts.Copy(codeStatementPredicate.Name);
						List<List<string>> factsAllArgsTable = listByFactsAll.ToListArgs();

						if (factsArgsTableByStatement == null)
						{
							factsArgsTableByStatement = factsAllArgsTable;
							continue;
						}
						else
						{
							List<List<string>> tbl0 = new List<List<string>>();

							foreach (List<string> rowByTable in factsArgsTableByStatement)
							{
								foreach (Predicate rowByFactsAll in listByFactsAll)
								{

									List<String> tmp = new List<string>(rowByTable);

									bool nothingToAdd = false;

									for (int i = 0; i < codeStatementPredicate.Args.Count(); ++i)
									{
										int argind = currentStatementArgs.IndexOf(codeStatementPredicate.Args[i]);

										if (argind < rowByTable.Count)
											if (rowByTable[argind] != rowByFactsAll.Args[i])
											{
												nothingToAdd = true;
												break;
											}

										if (argind >= rowByTable.Count)
											tmp.Add(rowByFactsAll.Args[i]);

									}

									if (nothingToAdd) continue;

									if (!tbl0.IsSet(tmp)) tbl0.Add(tmp);
								}
							}

							factsArgsTableByStatement = tbl0;
						}

					}

					foreach (List<string> row in factsArgsTableByStatement)
					{
						string tmpr = "\t";

						tmpr += string.Join(" ", row);

						var indexes = currentStatement.Result.Args.Select(x => currentStatementArgs.IndexOf(x));
						var args = indexes.Select(x => row[x]);
						var factNew = new Predicate(currentStatement.Result.Name, args);

						Log($"{tmpr} -> {factNew}");

						if (!facts.ContainsAtAllProp(factNew))
						{
							facts.Add(factNew);
							foundedFacts.Add(factNew);
							wasChanged = true;

							Log("\t\t Добавлен");
						}
					}
				}
			}

			return foundedFacts;
		}
		private void ParseStringToCollections(string text, out List<Statement> statements, out List<Predicate> predicates, out List<Predicate> facts)
		{
			predicates = new List<Predicate>();
			facts = new List<Predicate>();
			statements = new List<Statement>();

			string[] linesByCode = text.Split(new char[] { ';', '\n', '\r' });

			List<string> keyWords = new List<string>
			{
				"Предикаты:",
				"Аксиомы:",
				"Факты:"
			};

			string line;

			string currentKeyWord = "Предикаты:";

			foreach (var codeLine in linesByCode)
			{
				line = codeLine;
				line = line.Replace('\t', ' ');
				line = line.Trim();

				if (line == string.Empty) continue;

				if (keyWords.Contains(line))
				{
					currentKeyWord = line;
					Log("\t " + line);
					continue;
				}

				switch (currentKeyWord)
				{
					case "Предикаты:":
						predicates.Add(Predicate.FromString(line));
						Log("Создан предикат " + predicates.Count + ":\t" + Predicate.FromString(line));
						break;
					case "Аксиомы:":
						statements.Add(Statement.FromString(line));
						Log("Создана аксиома " + statements.Count + ":\t" + Statement.FromString(line));
						break;
					case "Факты:":
						facts.Add(Predicate.FromString(line));
						Log("Создан факт " + facts.Count + ":\t" + Predicate.FromString(line));
						break;
				}
			}

		}

		private void Log(string mes)
		{
			LogText += $"\n{mes}";
		}
		
		private void Log(IEnumerable<string> mes)
		{
			LogText += $"\n{string.Join("\n", mes)}";
		}

		#endregion Helpers
	}
}
