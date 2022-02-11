using AutoMapper;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Courses;

public class Index : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Index(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	public Result Data { get; private set; }

	public async Task OnGetAsync() => Data = await Handle(new Query());

	public record Query
	{
	}

	public record Result
	{
		public List<Course> Courses { get; init; }

		public record Course
		{
			public int Id { get; init; }
			public string Title { get; init; }
			public int Credits { get; init; }
			public string DepartmentName { get; init; }
		}
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Course, Result.Course>();
	}


	public async Task<Result> Handle(Query message)
	{
		var courses = await _db.Courses
			.OrderBy(d => d.Id)
			.ProjectToListAsync<Result.Course>(_configuration);

		return new Result
		{
			Courses = courses
		};
	}

}