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

namespace ContosoUniversity.Pages.Departments;

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

		return this.RedirectToPageJson("Index");
	}

	public record Query
	{
		public int Id { get; init; }
	}

	public record Command
	{
		public string Name { get; init; }

		public decimal Budget { get; init; }

		public DateTime StartDate { get; init; }

		public int Id { get; init; }

		[Display(Name = "Administrator")]
		public string AdministratorFullName { get; init; }

		public byte[] RowVersion { get; init; }
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
		var department = await _db.Departments.FindAsync(message.Id);

		_db.Departments.Remove(department);

		return;
	}
}