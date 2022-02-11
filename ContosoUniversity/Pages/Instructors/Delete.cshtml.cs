using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Instructors;

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

	public async Task OnGetAsync(Query query)
		=> Data = await Handle(query);

	public async Task<ActionResult> OnPostAsync()
	{
		await Handle(Data);

		return this.RedirectToPageJson(nameof(Index));
	}

	public record Query
	{
		public int? Id { get; init; }
	}

	public class Validator : AbstractValidator<Query>
	{
		public Validator()
		{
			RuleFor(m => m.Id).NotNull();
		}
	}

	public record Command
	{
		public int? Id { get; init; }

		public string LastName { get; init; }
		[Display(Name = "First Name")]
		public string FirstMidName { get; init; }

		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime? HireDate { get; init; }

		[Display(Name = "Location")]
		public string OfficeAssignmentLocation { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Instructor, Command>();
	}


	public Task<Command> Handle(Query message) => _db
		.Instructors
		.Where(i => i.Id == message.Id)
		.ProjectTo<Command>(_configuration)
		.SingleOrDefaultAsync();

	public async Task Handle(Command message)
	{
		var instructor = await _db.Instructors
			.Include(i => i.OfficeAssignment)
			.Where(i => i.Id == message.Id)
			.SingleAsync();

		instructor.Handle(message);

		_db.Instructors.Remove(instructor);

		var department = await _db.Departments
			.Where(d => d.InstructorId == message.Id)
			.SingleOrDefaultAsync();
		if (department != null)
		{
			department.InstructorId = null;
		}

		await _db.SaveChangesAsync();
	}

}