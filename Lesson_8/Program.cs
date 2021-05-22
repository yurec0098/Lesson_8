using System;
using System.Configuration;
using System.IO;

namespace Lesson_8
{
	class Program
	{
		static void Main(string[] args)
		{
			if (HelloSettings())
			{
				ReadOrAddSettings("userName", "Ваше имя: {0}", "Введите Ваше имя:");
				ReadOrAddSettings("userAge", "Ваш возраст: {0}", "Введите Ваш возраст:");
				ReadOrAddSettings("userOccupation", "Ваш род деятельности: {0}", "Введите род деятельности:");
			}
			else
			{
				Console.WriteLine();
				Console.WriteLine("У нас возникла проблема с файлом настроек");
				Console.WriteLine("Мы его удалили, если он существовал и был не корректным");
				Console.WriteLine("Пожалуйста перезапустите приложение");
			}

			Console.ReadLine();
		}

		static bool HelloSettings()
		{
			try
			{
				string key = "hello";

				//  как видимо данная коллекция не распространяется по ссылке,
				//  нет смысла использовать переменную,
				//  если мы будем изменять коллекцию
				if (ConfigurationManager.AppSettings[key] == null)
					AddUpdateAppSettings(key, "Вас приветствует приложение, которое способно запоминать)");

				Console.WriteLine(ConfigurationManager.AppSettings[key]);
				Console.WriteLine();

				return true;
			}
			catch (ConfigurationErrorsException ex)
			{
				Console.WriteLine(ex.Message);

				//string ApplicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

				//	При инициализации приложений на .Net в аргументы командной строки класса Environment
				//	первым элементом записывается путь к исполняемому файлу.
				//	Этот же пусть используется для создания пути настроек
				string ApplicationPath = Environment.GetCommandLineArgs()[0];
				if (File.Exists($"{ApplicationPath}.config"))
					File.Delete($"{ApplicationPath}.config");

				return false;
			}
		}

		static void ReadOrAddSettings(string key, string infoText, string addText)
		{
			try
			{
				if (ConfigurationManager.AppSettings[key] == null)
				{
					(int Left, int Top) cursorPosition = Console.GetCursorPosition();
					Console.WriteLine(addText);

					string value = Console.ReadLine();
					while (string.IsNullOrWhiteSpace(value))
					{
						ClearConsoleLines(cursorPosition.Left, cursorPosition.Top, 2);
						Console.WriteLine($"Повторим... {addText}");
						value = Console.ReadLine();
					}

					AddUpdateAppSettings(key, value);
				}

				Console.WriteLine(infoText, ConfigurationManager.AppSettings[key]);
			}
			catch (ConfigurationErrorsException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		static void AddUpdateAppSettings(string key, string value)
		{
			try
			{
				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;

				if (settings[key] != null)
					settings[key].Value = value;
				else
					settings.Add(key, value);

				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
			}
			catch (ConfigurationErrorsException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void ClearConsoleLines(int left, int top, int count)
		{
			Console.SetCursorPosition(left, top);
			for (int index = 0; index < count; ++index)
				Console.WriteLine(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(left, top);
		}
	}
}
