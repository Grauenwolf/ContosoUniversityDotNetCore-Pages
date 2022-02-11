﻿using ContosoUniversity.Models;
using ContosoUniversity.Pages.Instructors;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests.Pages.Instructors;

[Collection(nameof(SliceFixture))]
public class DeleteTests
{
	private readonly SliceFixture _fixture;

	public DeleteTests(SliceFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task Should_query_for_command()
	{
		using var scope = _fixture.GetTestResources();
		var englishDept = new Department
		{
			Name = "English",
			StartDate = DateTime.Today
		};
		var english101 = new Course
		{
			Department = englishDept,
			Title = "English 101",
			Credits = 4,
			Id = _fixture.NextCourseNumber()
		};
		var command = new CreateEdit.Command
		{
			FirstMidName = "George",
			LastName = "Costanza",
			OfficeAssignmentLocation = "Austin",
			HireDate = DateTime.Today,
			SelectedCourses = new[] { english101.Id.ToString() }
		};
		var instructorId = await (new CreateEdit(scope.Db, scope.Mapper)).Handle(command);

		await _fixture.InsertAsync(englishDept, english101);

		var result = await (new Delete(scope.Db, scope.Mapper)).Handle(new Delete.Query { Id = instructorId });

		result.ShouldNotBeNull();
		result.FirstMidName.ShouldBe(command.FirstMidName);
		result.OfficeAssignmentLocation.ShouldBe(command.OfficeAssignmentLocation);
	}

	[Fact]
	public async Task Should_delete_instructor()
	{
		using var scope = _fixture.GetTestResources();
		var instructorId = await (new CreateEdit(scope.Db, scope.Mapper)).Handle(new CreateEdit.Command
		{
			FirstMidName = "George",
			LastName = "Costanza",
			OfficeAssignmentLocation = "Austin",
			HireDate = DateTime.Today
		});
		var englishDept = new Department
		{
			Name = "English",
			StartDate = DateTime.Today,
			InstructorId = instructorId
		};
		var english101 = new Course
		{
			Department = englishDept,
			Title = "English 101",
			Credits = 4,
			Id = _fixture.NextCourseNumber()
		};

		await _fixture.InsertAsync(englishDept, english101);

		await (new CreateEdit(scope.Db, scope.Mapper)).Handle(new CreateEdit.Command
		{
			Id = instructorId,
			FirstMidName = "George",
			LastName = "Costanza",
			OfficeAssignmentLocation = "Austin",
			HireDate = DateTime.Today,
			SelectedCourses = new[] { english101.Id.ToString() }
		});

		await (new Delete(scope.Db, scope.Mapper)).Handle(new Delete.Command { Id = instructorId });

		var instructorCount = await _fixture.ExecuteDbContextAsync(db => db.Instructors.Where(i => i.Id == instructorId).CountAsync());

		instructorCount.ShouldBe(0);

		var englishDeptId = englishDept.Id;
		englishDept = await _fixture.ExecuteDbContextAsync(db => db.Departments.FindAsync(englishDeptId));

		englishDept.InstructorId.ShouldBeNull();

		var courseInstructorCount = await _fixture.ExecuteDbContextAsync(db => db.CourseAssignments.Where(ci => ci.InstructorId == instructorId).CountAsync());

		courseInstructorCount.ShouldBe(0);
	}

}