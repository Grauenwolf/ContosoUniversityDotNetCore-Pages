﻿using ContosoUniversity.Models;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Index = ContosoUniversity.Pages.Students.Index;

namespace ContosoUniversity.IntegrationTests.Pages.Students;

[Collection(nameof(SliceFixture))]
public class IndexTests
{
	private readonly SliceFixture _fixture;

	public IndexTests(SliceFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task Should_return_all_items_for_default_search()
	{
		using var scope = _fixture.GetTestResources();
		var suffix = DateTime.Now.Ticks.ToString();
		var lastName = "Schmoe" + suffix;
		var student1 = new Student
		{
			EnrollmentDate = DateTime.Today,
			FirstMidName = "Joe",
			LastName = lastName
		};
		var student2 = new Student
		{
			EnrollmentDate = DateTime.Today,
			FirstMidName = "Jane",
			LastName = lastName
		};
		await _fixture.InsertAsync(student1, student2);

		var query = new Index.Query { CurrentFilter = lastName, PageSize = 100 };

		var result = await (new Index(scope.Db, scope.Mapper)).Handle(query);

		result.Results.Count.ShouldBeGreaterThanOrEqualTo(2);
		result.Results.Select(r => r.Id).ShouldContain(student1.Id);
		result.Results.Select(r => r.Id).ShouldContain(student2.Id);
	}

	[Fact]
	public async Task Should_sort_based_on_name()
	{
		using var scope = _fixture.GetTestResources();
		var suffix = DateTime.Now.Ticks.ToString();
		var lastName = "Schmoe" + suffix;
		var student1 = new Student
		{
			EnrollmentDate = DateTime.Today,
			FirstMidName = "Joe",
			LastName = lastName + "zzz"
		};
		var student2 = new Student
		{
			EnrollmentDate = DateTime.Today,
			FirstMidName = "Jane",
			LastName = lastName + "aaa"
		};
		await _fixture.InsertAsync(student1, student2);

		var query = new Index.Query { CurrentFilter = lastName, SortOrder = "name_desc", PageSize = 100 };

		var result = await (new Index(scope.Db, scope.Mapper)).Handle(query);

		result.Results.Count.ShouldBe(2);
		result.Results[0].Id.ShouldBe(student1.Id);
		result.Results[1].Id.ShouldBe(student2.Id);
	}
}