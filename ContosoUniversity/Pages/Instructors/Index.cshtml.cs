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

namespace ContosoUniversity.Pages.Instructors;


public class Index : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Index(SchoolContext db, IConfigurationProvider configuration)

	{
		_db = db;
		_configuration = configuration;
	}

	public Model Data { get; private set; }

	public async Task OnGetAsync(Query query)
		=> Data = await Handle(query);

	public record Query
	{
		public int? Id { get; init; }
		public int? CourseId { get; init; }
	}

	public record Model
	{
		public int? InstructorId { get; init; }
		public int? CourseId { get; init; }

		public IList<Instructor> Instructors { get; init; }
		public IList<Course> Courses { get; init; }
		public IList<Enrollment> Enrollments { get; init; }

		public record Instructor
		{
			public int Id { get; init; }

			[Display(Name = "Last Name")]
			public string LastName { get; init; }

			[Display(Name = "First Name")]
			public string FirstMidName { get; init; }

			[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
			[Display(Name = "Hire Date")]
			public DateTime HireDate { get; init; }

			public string OfficeAssignmentLocation { get; init; }

			public IEnumerable<CourseAssignment> CourseAssignments { get; init; }
		}

		public record CourseAssignment
		{
			public int CourseId { get; init; }
			public string CourseTitle { get; init; }
		}

		public record Course
		{
			public int Id { get; init; }
			public string Title { get; init; }
			public string DepartmentName { get; init; }
		}

		public record Enrollment
		{
			[DisplayFormat(NullDisplayText = "No grade")]
			public Grade? Grade { get; init; }
			public string StudentFullName { get; init; }
		}
	}

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateProjection<Instructor, Model.Instructor>();
			CreateProjection<CourseAssignment, Model.CourseAssignment>();
			CreateProjection<Course, Model.Course>();
			CreateProjection<Enrollment, Model.Enrollment>();
		}
	}



	public async Task<Model> Handle(Query message)
	{
		var instructors = await _db.Instructors
				.Include(i => i.CourseAssignments)
				.ThenInclude(c => c.Course)
				.OrderBy(i => i.LastName)
				.ProjectTo<Model.Instructor>(_configuration)
				.ToListAsync()
			;

		// EF Core cannot project child collections w/o Include
		// See https://github.com/aspnet/EntityFrameworkCore/issues/9128
		//var instructors = await _db.Instructors
		//    .OrderBy(i => i.LastName)
		//    .ProjectToListAsync<Model.Instructor>();

		var courses = new List<Model.Course>();
		var enrollments = new List<Model.Enrollment>();

		if (message.Id != null)
		{
			courses = await _db.CourseAssignments
				.Where(ci => ci.InstructorId == message.Id)
				.Select(ci => ci.Course)
				.ProjectTo<Model.Course>(_configuration)
				.ToListAsync();
		}

		if (message.CourseId != null)
		{
			enrollments = await _db.Enrollments
				.Where(x => x.CourseId == message.CourseId)
				.ProjectTo<Model.Enrollment>(_configuration)
				.ToListAsync();
		}

		var viewModel = new Model
		{
			Instructors = instructors,
			Courses = courses,
			Enrollments = enrollments,
			InstructorId = message.Id,
			CourseId = message.CourseId
		};

		return viewModel;
	}

}