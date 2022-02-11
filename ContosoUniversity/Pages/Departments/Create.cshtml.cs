using AutoMapper;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Departments;

public class Create : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	[BindProperty]
	public Command Data { get; set; }

	public Create(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	public async Task<ActionResult> OnPostAsync()
	{
		await Handle(Data);

		return this.RedirectToPageJson("Index");
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

	public record Command
	{
		[StringLength(50, MinimumLength = 3)]
		public string Name { get; init; }

		[DataType(DataType.Currency)]
		[Column(TypeName = "money")]
		public decimal? Budget { get; init; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? StartDate { get; init; }

		public Instructor Administrator { get; init; }
	}


	public async Task<int> Handle(Command message)
	{
		var department = new Department
		{
			Administrator = message.Administrator,
			Budget = message.Budget!.Value,
			Name = message.Name,
			StartDate = message.StartDate!.Value
		};

		await _db.Departments.AddAsync(department);

		await _db.SaveChangesAsync();

		return department.Id;
	}
}