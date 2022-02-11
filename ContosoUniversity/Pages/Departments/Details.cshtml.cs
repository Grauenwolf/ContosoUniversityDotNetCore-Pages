﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Departments;

public class Details : PageModel
{
	private readonly SchoolContext _db;
	private readonly IConfigurationProvider _configuration;

	public Model Data { get; private set; }

	public Details(SchoolContext db, IConfigurationProvider configuration)
	{
		_db = db;
		_configuration = configuration;
	}

	public async Task OnGetAsync(Query query)
		=> Data = await Handle(query);

	public record Query
	{
		public int Id { get; init; }
	}

	public record Model
	{
		public string Name { get; init; }

		public decimal Budget { get; init; }

		public DateTime StartDate { get; init; }

		public int Id { get; init; }

		[Display(Name = "Administrator")]
		public string AdministratorFullName { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateProjection<Department, Model>();
	}


	public Task<Model> Handle(Query message) =>
		_db.Departments
			.Where(m => m.Id == message.Id)
			.ProjectTo<Model>(_configuration)
			.DecompileAsync()
			.SingleOrDefaultAsync();
}