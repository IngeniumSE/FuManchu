// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Verification;

public class ComplexDocumentFacts
{
	[Fact]
	public async Task CanVerifyComplexDocument()
	{
		var service = await CreateService();
		string template = await GetTemplate("OrderConfirmation");

		var model = new
		{
			OrderId = 12345,
			OrderContainsBooking = true,
			OrderContainsDirectBooking = false,

			Customer = new
			{
				Name = "Matthew Abbott",
				Email = "email@somewhere.com"
			},

			Agent = new
			{
				Forename = "Jo"
			},

			BillingAddress = new
			{
				Forename = "Matthew",
				Surname = "Abbott",
				Line1 = "1 Somestreet Lane",
				City = "Somewhere",
				County = "Co. Somewhere",
				Postcode = "AA11 1AA",
				Country = "United Kingdom"
			}
		};

		string result = service.CompileAndRun("OrderConfirmation", template, model);

		await Verify(result);
	}

	static async Task<IHandlebarsService> CreateService()
	{
		var service = new HandlebarsService();
		foreach (var file in Directory.EnumerateFiles("./Content", "*.partial.hbs"))
		{
			string fileName = Path.GetFileName(file);
			fileName = fileName.Substring(0, fileName.Length - ".partial.hbs".Length);

			string content = await GetTemplateContent(file);
			service.RegisterPartial(fileName, content);
		}

		return service;
	}

	static async Task<string> GetTemplate(string templateName)
	{
		string path = $"./Content/{templateName}.hbs";

		return await GetTemplateContent(path);
	}

	static async Task<string> GetTemplateContent(string filePath)
		=> await File.ReadAllTextAsync(filePath);
}
