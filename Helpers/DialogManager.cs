using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Windows;

namespace LoS.Helpers
{
	public static class DialogManager
	{
        public static bool FolderDialog(string description, string initialFolder, out string selectedFolder)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                if (!string.IsNullOrWhiteSpace(initialFolder))
                {
                    dialog.SelectedPath = initialFolder;
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFolder = dialog.SelectedPath;
                    return true;
                }
            }
            selectedFolder = "";
            return false;
        }

        public static bool OpenFilesDialog(bool checkFileExists, string Filter, out IEnumerable<string> FileNames)
        {
            FileNames = new List<string>();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = Filter,
                CheckFileExists = checkFileExists
            };
            bool result = ((bool)openFileDialog.ShowDialog());
            if (result)
            {
                FileNames = openFileDialog.FileNames.ToList();
            }
            return result;
        }

        public static bool OpenFileDialog(bool checkFileExists, string Filter, out string FileName)
        {
            FileName = "";
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                //openFileDialog.Filter = "All Image Files | *.jpg;*.png | All files | *.*";
                Filter = Filter,
                CheckFileExists = checkFileExists
            };
            bool result = ((bool)openFileDialog.ShowDialog());
            if (result)
            {
                FileName = openFileDialog.FileName;
            }
            return result;
        }

        public static bool SaveFileDialog(bool checkFileExists, string Filter, out string FileName)
        {
            FileName = "";
            Microsoft.Win32.SaveFileDialog openFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = Filter,
                AddExtension = true
            };
            bool result = ((bool)openFileDialog.ShowDialog());
            if (result)
            {
                FileName = openFileDialog.FileName;
            }
            return result;
        }

        private static MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon)
            => System.Windows.MessageBox.Show(message, caption, buttons, icon);

        /// <summary>
        /// Диалоговое окно, требующее подтверждения операции
        /// </summary>
        /// <returns>true - if Yes</returns>
        public static bool ShowApprovalDialog(string message, string caption)
            => ShowMessageBox(
                message,
                caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;

        /// <summary>
        /// Показавает сообщение об ошибке
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="title">Заголовок</param>
        /// <param name="withContactTechSupport">Прибавить ли к сообщению необходимость обратиться в техподдержку</param>
        public static void ShowErrorMessage(string message = "", string title = "Ошибка!", bool withContactTechSupport = true)
        {
            ShowMessageBox(
                string.Join(" ",
                    "Ошибка выполнения операции.",
                    message,
                    withContactTechSupport
                        ? "Обратитесь в тех.поддержку."
                        : string.Empty
                ),
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Предупреждающее сообщение
        /// </summary>
        public static void ShowAlertMessage(string message)
            => ShowMessageBox(message, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        /// <summary>
        /// Возвращает true, если MessageBoxResult.Yes,
        /// false, если MessageBoxResult.No
        /// и null, если MessageBoxResult.Cansel
        /// </summary>
        /// <param name="messageYes">Подсказка пользователю если он нажмет на Yes</param>
        /// <param name="messageNo">Подсказка пользователю если он нажмет на No</param>
        /// <param name="title">Заголовок сообщения</param>
        public static bool? ShowQuestion(string messageYes, string messageNo, string title)
        {
            MessageBoxResult result = ShowMessageBox(
                string.Join(" ", "(Да) -", messageYes, "\nНет -", messageNo),
                title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Cancel) return null;
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Возвращает true, если MessageBoxResult.Yes,
        /// false, если MessageBoxResult.No
        /// и null, если MessageBoxResult.Cansel
        /// </summary>
        /// <param name="message">Сообщение пользователю</param>
        /// <param name="title">Заголовок сообщения</param>
        public static bool? ShowQuestion(string message, string title = "Внимание!")
        {
            MessageBoxResult result = ShowMessageBox(
                message,
                title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Cancel) return null;
            return result == MessageBoxResult.Yes;
        }
    }
}
