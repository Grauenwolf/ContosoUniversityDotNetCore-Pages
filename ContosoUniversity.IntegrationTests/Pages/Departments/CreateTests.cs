using ContosoUniversity.Pages.Departments;
using ContosoUniversity.Pages.Instructors;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests.Pages.Departments;

[Collection(nameof(SliceFixture))]
public class CreateTests
{
	private readonly SliceFixture _fixture;

	public CreateTests(SliceFixture fixture) => _fixture = fixture;


	[Fact]
	public async Task Should_create_new_department()
	{
		using var scope = _fixture.GetTestResources();

		var adminId = await (new CreateEdit(scope.Db, scope.Mapper)).Handle(
			new CreateEdit.Command
			{
				FirstMidName = "George",
				LastName = "Costanza",
				HireDate = DateTime.Today
			});

		Create.Command command = null;


		var admin = await scope.Db.Instructors.FindAsync(adminId);

		command = new Create.Command
		{
			Budget = 10m,
			Name = "Engineering",
			StartDate = DateTime.Now.Date,
			Administrator = admin
		};

		await (new Create(scope.Db, scope.Mapper)).Handle(command);


		var created = await _fixture.ExecuteDbContextAsync(db => db.Departments.Where(d => d.Name == command.Name).SingleOrDefaultAsync());

		created.ShouldNotBeNull();
		created.Budget.ShouldBe(command.Budget.GetValueOrDefault());
		created.StartDate.ShouldBe(command.StartDate.GetValueOrDefault());
		created.InstructorId.ShouldBe(adminId);
	}
}