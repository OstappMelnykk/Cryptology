using CryptologyLabs;
using Microsoft.AspNetCore.Mvc;

namespace encryption.Controllers
{
	public class CaesarController : Controller
	{
		private static Dictionary<char, int> frequencyTable;


		public IActionResult Index(string content = "")
		{
			ViewBag.Content = content;
			return View();

		}


		public IActionResult DownloadFile(string sourceText)
		{
			string fileName = "example_file.txt";
			string filePath = Path.Combine("..", Directory.GetCurrentDirectory(), fileName);
			System.IO.File.WriteAllText(filePath, sourceText);

			return PhysicalFile(filePath, "application/octet-stream", fileName);
		}




		public IActionResult Encrypt(string sourceText, int step)
		{
			frequencyTable = new();

			ViewBag.ToReturn = new string(sourceText.Select(v => EncodeChar(v, step)).ToArray());
			ViewBag.sourceText = sourceText;

			return View("Encrypt");
		}

		public IActionResult Decrypt(string sourceText, int step)
		{
			ViewBag.ToReturn = new string(sourceText.Select(v => DecodeChar(v, step)).ToArray());
			ViewBag.sourceText = sourceText;

			return View("Decrypt");

		}

		public IActionResult PrintFrequencyTable(string sourceText, int step)
		{
			ViewBag.Encounters = frequencyTable;
			return View("PrintFrequencyTable");

		}


		public IActionResult Attack(string sourceText, string encryptedText)
		{
			int step = 0;

			while (step < 10000)
			{
				if (new string(encryptedText.Select(v => DecodeChar(v, step)).ToArray()) == sourceText)
				{
					return Content(step.ToString());
				}

				step++;
			}
			if (step == 1000)
				return Content("The brute force attack failed");


			return null;
		}







		private char EncodeChar(char from, int step)
		{
			if (Alphabets.ukrainian.Contains(from))
			{
				var current = Alphabets.ukrainian.IndexOf(from);

				if (frequencyTable.ContainsKey(Alphabets.ukrainianCapital[current]))
					frequencyTable[Alphabets.ukrainianCapital[current]]++;
				else
					frequencyTable[Alphabets.ukrainianCapital[current]] = 1;

				int index = (current + step) % Alphabets.ukrainianLen;

				if (index < 0)
					index += Alphabets.ukrainianLen;

				return Alphabets.ukrainian[index];
			}

			if (Alphabets.ukrainianCapital.Contains(from))
			{
				var current = Alphabets.ukrainianCapital.IndexOf(from);

				if (frequencyTable.ContainsKey(from))
					frequencyTable[from]++;
				else
					frequencyTable[from] = 1;

				int index = (current + step) % Alphabets.ukrainianLen;

				if (index < 0)
					index += Alphabets.ukrainianLen;

				return Alphabets.ukrainianCapital[index];
			}

			if (from > 64 && from < 91)
			{
				int current = from - 'A';

				if (frequencyTable.ContainsKey(from))
					frequencyTable[from]++;
				else
					frequencyTable[from] = 1;

				int index = (current + step) % Alphabets.englishLen;

				if (index < 0)
					index += Alphabets.englishLen;

				return (char)(index + 'A');
			}

			if (from > 96 && from < 123)
			{
				int current = from - 'a';

				if (frequencyTable.ContainsKey((char)(current + 'A')))
					frequencyTable[(char)(current + 'A')]++;
				else
					frequencyTable[(char)(current + 'A')] = 1;

				int index = (current + step) % Alphabets.englishLen;

				if (index < 0)
					index += Alphabets.englishLen;

				return (char)(index + 'a');
			}

			return from;
		}

		private char DecodeChar(char from, int step)
		{
			if (Alphabets.ukrainian.Contains(from))
			{
				var current = Alphabets.ukrainian.IndexOf(from);

				int index = (current - step) % Alphabets.ukrainianLen;

				if (index < 0)
					index += Alphabets.ukrainianLen;

				return Alphabets.ukrainian[index];
			}

			if (Alphabets.ukrainianCapital.Contains(from))
			{
				var current = Alphabets.ukrainianCapital.IndexOf(from);

				int index = (current - step) % Alphabets.ukrainianLen;

				if (index < 0)
					index += Alphabets.ukrainianLen;

				return Alphabets.ukrainianCapital[index];
			}

			if (from > 64 && from < 91)
			{
				int current = from - 'A';

				int index = (current - step) % Alphabets.englishLen;

				if (index < 0)
					index += Alphabets.englishLen;

				return (char)(index + 'A');
			}

			if (from > 96 && from < 123)
			{
				int current = from - 'a';

				int index = (current - step) % Alphabets.englishLen;

				if (index < 0)
					index += Alphabets.englishLen;

				return (char)(index + 'a');
			}

			return from;
		}
	}
}
