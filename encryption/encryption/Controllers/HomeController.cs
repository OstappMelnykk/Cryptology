
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace encryption.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			try
			{
				if (file != null && file.Length > 0)
				{
					var uploadsFolder = Path.Combine("..", Directory.GetCurrentDirectory(), "Files");

					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					var uniqueFileName = file.FileName;
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					return RedirectToAction("Index", "Caesar", new { content = ReadTextFile(filePath) });
				}
				else return NotFound("Error. There is no uploaded file");

			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred: {ex.Message}");
			}
		}



		private string ReadTextFile(string filePath)
		{
			if (System.IO.File.Exists(filePath) && Path.GetExtension(filePath)?.ToLower() == ".txt")
			{
				var content = System.IO.File.ReadAllText(filePath);
				return content;
			}
			else
			{
				throw new ArgumentException("Invalid file path or file type.");
			}

		}
	}
}
