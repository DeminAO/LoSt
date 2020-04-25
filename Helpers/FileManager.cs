using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoS.Helpers
{
	public static class FileManager
	{
        public static string Combine(params string[] paths) => Path.Combine(paths);

        public static string GetFolderCommonApplicationDataPath() => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static string GetExecutingAssemplyPath() => GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string GetDirectoryName(string path) => Path.GetDirectoryName(path);

        public static bool GetIsDirectoryExists(string path) => Directory.Exists(path);

        public static void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!GetIsDirectoryExists(path))
                CreateDirectory(path);
        }

        public static bool GetIsFileExists(string fullPathFileName) => File.Exists(fullPathFileName);

        public static bool GetIsFileEmpty(string fullPathFileName) => new FileInfo(fullPathFileName).Length == 0;

        public static bool GetIsFileExistsAndNotEmpty(string fullPathFileName)
            => GetIsFileExists(fullPathFileName) && !GetIsFileEmpty(fullPathFileName);

        /// <summary>
        /// Проверяет условие над файлом.
        /// Истино, если файл существует, не пуст и удовлетворяет условию
        /// </summary>
        /// <param name="fullPathFileName">Полный путь к файлу</param>
        /// <param name="condition">Условие для проверки файла</param>
        public static bool IsFileExistsNotAmptyAndMeetCondition(string fullPathFileName, Predicate<FileInfo> condition)
        {
            var info = new FileInfo(fullPathFileName);
            return info.Exists && info.Length > 0 && condition.Invoke(info);
        }

        public static void CreateAndCloseFileIfNotExists(string fullPathFileName)
        {
            if (!File.Exists(fullPathFileName))
                File.Create(fullPathFileName).Dispose();
        }

        public static void WriteAllText(string fullPathFileName, string content)
            => File.WriteAllText(fullPathFileName, content);

        public static void WriteAllBytes(string fullPathFileName, byte[] content)
            => File.WriteAllBytes(fullPathFileName, content);

        public static byte[] ReadAllBytes(string fullPathFileName) => File.ReadAllBytes(fullPathFileName);
        public static string ReadAllText(string fullPathFileName) => File.ReadAllText(fullPathFileName);
        public static IEnumerable<string> ReadLines(string fullPathFileName) => File.ReadLines(fullPathFileName);

        public static void SetReadOnlyAttributeTofile(string fullPathFileName) => File.SetAttributes(fullPathFileName, FileAttributes.ReadOnly);

        public static void DeleteFileIfExists(string fullPathFileName)
        {
            if (File.Exists(fullPathFileName))
                File.Delete(fullPathFileName);
        }

        public static string GetTempPath() => Path.GetTempPath();

        public static string GetCurrentDomainBaseDirectory() => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Возвращает отсортированные по алфовиту полные пути к файлам, подходящих под шаблон поиска.
        /// </summary>
        /// <param name="directoryName">Директория для поиска файлов</param>
        /// <param name="searchPattern">
        /// Строка поиска, которая будет сравниваться с именами файлов. Этот параметр может
        /// содержать сочетание допустимого литерального пути и подстановочных символов (* и ?). 
        /// Шаблон по умолчанию, возвращающий все файлы, — "*".
        /// </param>
        /// <param name="searchLevel">0 - TopDirectoryOnly, 1 - AllDirectories</param>
        /// <param name="condition">Дополнительное условие для поиска файлов</param>
        public static IEnumerable<string> GetFullPathFileNamesByDirectory(string directoryName, string searchPattern, int searchLevel = 1, Func<FileInfo, bool> condition = null)
        {
            CreateDirectoryIfNotExists(directoryName);

            return new DirectoryInfo(directoryName)
                .GetFiles(searchPattern, (SearchOption)searchLevel)
                .Where(f => condition == null || condition.Invoke(f))
                .OrderBy(f => f.Name)
                .Select(x => x.FullName);
        }

        public static string GetFileNameWithoutExtension(string fullPathFileName) => Path.GetFileNameWithoutExtension(fullPathFileName);

        public static string GetFileExtension(string fullPathFileName) => Path.GetExtension(fullPathFileName);

        public static string GetFileName(string fullPathFileName) => Path.GetFileName(fullPathFileName);

        public static DateTime GetCreationTime(string fullPathFileName) => File.GetCreationTime(fullPathFileName);

        public static DateTime GetLastWriteTime(string fullPathFileName) => File.GetLastWriteTime(fullPathFileName);

        public static void SetLastWriteTime(string fullPathFileName, DateTime dateTime) => File.SetLastWriteTime(fullPathFileName, dateTime);

        public static string GetCurrentProcessMainModuleFileName() => Process.GetCurrentProcess().MainModule.FileName;

        public static StringWriter CreateStringWriter() => new StringWriter();

    }
}
