using AutoMapper;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Students;

public class Create : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Create(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	[BindProperty]
	public Command Data { get; set; }

	public void OnGet() => Data = new Command();

	public async Task<IActionResult> OnPostAsync()
	{
		await Handle(Data);

		return this.RedirectToPageJson(nameof(Index));
	}

	public record Command
	{
		public string LastName { get; init; }

		[Display(Name = "First Name")]
		public string FirstMidName { get; init; }

		public DateTime? EnrollmentDate { get; init; }
	}

	public class Validator : AbstractValidator<Command>
	{
		public Validator()
		{
			RuleFor(m => m.LastName).NotNull().Length(1, 50);
			RuleFor(m => m.FirstMidName).NotNull().Length(1, 50);
			RuleFor(m => m.EnrollmentDate).NotNull();
		}
	}


	public async Task<int> Handle(Command message)
	{
		var student = new Student
		{
			FirstMidName = message.FirstMidName,
			LastName = message.LastName,
			EnrollmentDate = message.EnrollmentDate!.Value
		};

		await _db.Students.AddAsync(student);

		await _db.SaveChangesAsync();

		return student.Id;
	}

}