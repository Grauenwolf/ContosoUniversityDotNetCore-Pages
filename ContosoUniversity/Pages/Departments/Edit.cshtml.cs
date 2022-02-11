using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Departments;

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

	public async Task OnGetAsync(Query query)
		=> Data = await Handle(query);

	public async Task<ActionResult> OnPostAsync(int id)
	{
		await Handle(Data);

		return this.RedirectToPageJson("Index");
	}

	public record Query
	{
		public int Id { get; init; }
	}

	public record Command
	{
		public string Name { get; init; }

		public decimal? Budget { get; init; }

		public DateTime? StartDate { get; init; }

		public Instructor Administrator { get; init; }
		public int Id { get; init; }
		public byte[] RowVersion { get; init; }
	}

	public class Validator : AbstractValidator<Command>
	{
		public Validator()
		{
			RuleFor(m => m.Name).NotNull().Length(3, 50);
			RuleFor(m => m.Budget).NotNull();
			RuleFor(m => m.StartDate).NotNull();
			RuleFor(m => m.Administrator).NotNull();
		}
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Department, Command>();
	}


	public async Task<Command> Handle(Query message) => await _db
		.Departments
		.Where(d => d.Id == message.Id)
		.ProjectTo<Command>(_configuration)
		.SingleOrDefaultAsync();


	public async Task Handle(Command message)
	{
		var dept = await _db.Departments.FindAsync(message.Id);

		dept.Name = message.Name;
		dept.StartDate = message.StartDate!.Value;
		dept.Budget = message.Budget!.Value;
		dept.RowVersion = message.RowVersion;
		dept.Administrator = await _db.Instructors.FindAsync(message.Administrator.Id);

		await _db.SaveChangesAsync();
	}
}