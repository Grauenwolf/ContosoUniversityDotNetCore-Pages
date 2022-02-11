using ContosoUniversity.Models;
using ContosoUniversity.Pages.Students;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests.Pages.Students;

[Collection(nameof(SliceFixture))]
public class EditTests
{
	private readonly SliceFixture _fixture;

	public EditTests(SliceFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task Should_get_edit_details()
	{
		using var scope = _fixture.GetTestResources();
		var cmd = new Create.Command
		{
			FirstMidName = "Joe",
			LastName = "Schmoe",
			EnrollmentDate = DateTime.Today
		};

		var studentId = await (new Create(scope.Db, scope.Mapper)).Handle(cmd);

		var query = new Edit.Query
		{
			Id = studentId
		};

		var result = await (new Edit(scope.Db, scope.Mapper)).Handle(query);

		result.FirstMidName.ShouldBe(cmd.FirstMidName);
		result.LastName.ShouldBe(cmd.LastName);
		result.EnrollmentDate.ShouldBe(cmd.EnrollmentDate);
	}

	[Fact]
	public async Task Should_edit_student()
	{
		using var scope = _fixture.GetTestResources();
		var createCommand = new Create.Command
		{
			FirstMidName = "Joe",
			LastName = "Schmoe",
			EnrollmentDate = DateTime.Today
		};

		var studentId = await (new Create(scope.Db, scope.Mapper)).Handle(createCommand);

		var editCommand = new Edit.Command
		{
			Id = studentId,
			FirstMidName = "Mary",
			LastName = "Smith",
			EnrollmentDate = DateTime.Today.AddYears(-1)
		};

		await (new Edit(scope.Db, scope.Mapper)).Handle(editCommand);

		var student = await _fixture.FindAsync<Student>(studentId);

		student.ShouldNotBeNull();
		student.FirstMidName.ShouldBe(editCommand.FirstMidName);
		student.LastName.ShouldBe(editCommand.LastName);
		student.EnrollmentDate.ShouldBe(editCommand.EnrollmentDate.GetValueOrDefault());
	}
}