using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Students;

public class Details : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Details(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	public Model Data { get; private set; }

	public async Task OnGetAsync(Query query)
		=> Data = await Handle(query);

	public record Query
	{
		public int Id { get; init; }
	}

	public record Model
	{
		public int Id { get; init; }
		[Display(Name = "First Name")]
		public string FirstMidName { get; init; }
		public string LastName { get; init; }
		public DateTime EnrollmentDate { get; init; }
		public List<Enrollment> Enrollments { get; init; }

		public record Enrollment
		{
			public string CourseTitle { get; init; }
			public Grade? Grade { get; init; }
		}
	}

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateProjection<Student, Model>();
			CreateProjection<Enrollment, Model.Enrollment>();
		}
	}


	public Task<Model> Handle(Query message) => _db
		.Students
		.Where(s => s.Id == message.Id)
		.ProjectTo<Model>(_configuration)
		.SingleOrDefaultAsync();
}