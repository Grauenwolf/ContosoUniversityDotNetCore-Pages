using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Students;

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
		public int Id { get; init; }
	}

	public record Command
	{
		public int Id { get; init; }
		[Display(Name = "First Name")]
		public string FirstMidName { get; init; }
		public string LastName { get; init; }
		public DateTime EnrollmentDate { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Student, Command>();
	}


	public async Task<Command> Handle(Query message) => await _db
		.Students
		.Where(s => s.Id == message.Id)
		.ProjectTo<Command>(_configuration)
		.SingleOrDefaultAsync();

	public async Task Handle(Command message)
	{
		_db.Students.Remove(await _db.Students.FindAsync(message.Id));

		return;
	}


}