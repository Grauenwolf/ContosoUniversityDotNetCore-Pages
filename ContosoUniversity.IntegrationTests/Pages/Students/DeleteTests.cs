﻿using ContosoUniversity.Models;
using ContosoUniversity.Pages.Students;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests.Pages.Students;

[Collection(nameof(SliceFixture))]
public class DeleteTests
{
	private readonly SliceFixture _fixture;

	public DeleteTests(SliceFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task Should_get_delete_details()
	{
		using var scope = _fixture.GetTestResources();
		var cmd = new Create.Command
		{
			FirstMidName = "Joe",
			LastName = "Schmoe",
			EnrollmentDate = DateTime.Today
		};

		var studentId = await (new Create(scope.Db, scope.Mapper)).Handle(cmd);

		var query = new Delete.Query
		{
			Id = studentId
		};

		var result = await (new Delete(scope.Db, scope.Mapper)).Handle(query);

		result.FirstMidName.ShouldBe(cmd.FirstMidName);
		result.LastName.ShouldBe(cmd.LastName);
		result.EnrollmentDate.ShouldBe(cmd.EnrollmentDate.GetValueOrDefault());
	}

	[Fact]
	public async Task Should_delete_student()
	{
		using var scope = _fixture.GetTestResources();
		var createCommand = new Create.Command
		{
			FirstMidName = "Joe",
			LastName = "Schmoe",
			EnrollmentDate = DateTime.Today
		};

		var studentId = await (new Create(scope.Db, scope.Mapper)).Handle(createCommand);

		var deleteCommand = new Delete.Command
		{
			Id = studentId
		};

		await (new Delete(scope.Db, scope.Mapper)).Handle(deleteCommand);

		var student = await _fixture.FindAsync<Student>(studentId);

		student.ShouldBeNull();
	}
}