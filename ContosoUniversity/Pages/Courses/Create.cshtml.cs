using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Courses;

public class Create : PageModel
{
	private readonly SchoolContext _db;

	public Create(SchoolContext db)
	{
		_db = db;
	}

	[BindProperty]
	public Command Data { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		await Handle(Data);

		return this.RedirectToPageJson("Index");
	}

	public record Command
	{
		public int Number { get; init; }
		public string Title { get; init; }
		public int Credits { get; init; }
		public Department Department { get; init; }
	}


	public async Task<int> Handle(Command message)
	{
		var course = new Course
		{
			Id = message.Number,
			Credits = message.Credits,
			Department = message.Department,
			Title = message.Title
		};

		await _db.Courses.AddAsync(course);

		await _db.SaveChangesAsync();

		return course.Id;
	}
}