using ContosoUniversity.Models;
using ContosoUniversity.Pages.Instructors;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Delete = ContosoUniversity.Pages.Courses.Delete;


namespace ContosoUniversity.IntegrationTests.Pages.Courses;

[Collection(nameof(SliceFixture))]
public class DeleteTests
{
	private readonly SliceFixture _fixture;

	public DeleteTests(SliceFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task Should_query_for_command()
	{
		using var scope = _fixture.GetTestResources();
		var adminId = await (new CreateEdit(scope.Db, scope.Mapper)).Handle(new CreateEdit.Command
		{
			FirstMidName = "George",
			LastName = "Costanza",
			HireDate = DateTime.Today
		});

		var dept = new Department
		{
			Name = "History",
			InstructorId = adminId,
			Budget = 123m,
			StartDate = DateTime.Today
		};

		var course = new Course
		{
			Credits = 4,
			Department = dept,
			Id = _fixture.NextCourseNumber(),
			Title = "English 101"
		};

		await _fixture.InsertAsync(dept, course);

		var result = await (new Delete(scope.Db, scope.Mapper)).Handle(new Delete.Query { Id = course.Id });

		result.ShouldNotBeNull();
		result.Credits.ShouldBe(course.Credits);
		result.DepartmentName.ShouldBe(dept.Name);
		result.Title.ShouldBe(course.Title);
	}

	[Fact]
	public async Task Should_delete()
	{
		using var scope = _fixture.GetTestResources();
		var adminId = await (new CreateEdit(scope.Db, scope.Mapper)).Handle(new CreateEdit.Command
		{
			FirstMidName = "George",
			LastName = "Costanza",
			HireDate = DateTime.Today
		});

		var dept = new Department
		{
			Name = "History",
			InstructorId = adminId,
			Budget = 123m,
			StartDate = DateTime.Today
		};

		var course = new Course
		{
			Credits = 4,
			Department = dept,
			Id = _fixture.NextCourseNumber(),
			Title = "English 101"
		};

		await _fixture.InsertAsync(dept, course);

		await (new Delete(scope.Db, scope.Mapper)).Handle(new Delete.Command { Id = course.Id });

		var result = await scope.Db.Courses.Where(c => c.Id == course.Id).SingleOrDefaultAsync();

		result.ShouldBeNull();
	}
}