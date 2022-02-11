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

public class Edit : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	[BindProperty]
	public Command Data { get; set; }

	public Edit(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

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
		public int? Credits { get; init; }
		public Department Department { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Course, Command>();
	}

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(m => m.Title).NotNull().Length(3, 50);
			RuleFor(m => m.Credits).NotNull().InclusiveBetween(0, 5);
		}
	}


	public async Task Handle(Command request)
	{
		var course = await _db.Courses.FindAsync(request.Id);

		course.Title = request.Title;
		course.Department = request.Department;
		course.Credits = request.Credits!.Value;

		await _db.SaveChangesAsync();

	}
}