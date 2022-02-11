using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Departments;

public class Index : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Index(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	public List<Model> Data { get; private set; }

	public async Task OnGetAsync()
		=> Data = await Handle(new Query());

	public record Query
	{
	}

	public record Model
	{
		public string Name { get; init; }

		public decimal Budget { get; init; }

		public DateTime StartDate { get; init; }

		public int Id { get; init; }

		public string AdministratorFullName { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Department, Model>();
	}




	public Task<List<Model>> Handle(Query message) => _db
		.Departments
		.ProjectTo<Model>(_configuration)
		.DecompileAsync()
		.ToListAsync();
}