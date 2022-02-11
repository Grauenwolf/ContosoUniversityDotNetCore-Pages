using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Courses;

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

	public async Task OnGetAsync(Query query) => Data = await Handle(query);

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

	public record Model
	{
		public int Id { get; init; }
		public string Title { get; init; }
		public int Credits { get; init; }
		[Display(Name = "Department")]
		public string DepartmentName { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Course, Model>();
	}




	public Task<Model> Handle(Query message) =>
		_db.Courses
			.Where(i => i.Id == message.Id)
			.ProjectTo<Model>(_configuration)
			.SingleOrDefaultAsync();

}