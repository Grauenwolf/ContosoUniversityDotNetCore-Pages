using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Courses;

public class Delete : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Delete(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	[BindProperty]
	public Command Data { get; set; }

	public async Task OnGetAsync(Query query) => Data = await Handle(query);

	public async Task<IActionResult> OnPostAsync()
	{
		await Handle(Data);

		return this.RedirectToPageJson(nameof(Index));
	}

	public record Query
	{
		public int? Id { get; init; }
	}

	public class QueryValidator : AbstractValidator<Query>
	{
		public QueryValidator()
		{
			RuleFor(m => m.Id).NotNull();
		}
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Course, Command>();
	}


	public Task<Command> Handle(Query message) =>
		_db.Courses
			.Where(c => c.Id == message.Id)
			.ProjectTo<Command>(_configuration)
			.SingleOrDefaultAsync();

	public record Command
	{
		[Display(Name = "Number")]
		public int Id { get; init; }
		public string Title { get; init; }
		public int Credits { get; init; }

		[Display(Name = "Department")]
		public string DepartmentName { get; init; }
	}

	public async Task Handle(Command message)
	{
		var course = await _db.Courses.FindAsync(message.Id);

		_db.Courses.Remove(course);

		return;
	}

}