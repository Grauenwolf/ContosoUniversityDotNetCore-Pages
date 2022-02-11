using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages;

public class AboutPage : PageModel
{
	private readonly SchoolContext _db;

	public AboutPage(SchoolContext context)
	{
		_db = context;
	}

	public IEnumerable<EnrollmentDateGroup> Data { get; private set; }

	public async Task OnGetAsync()
	{
		var groups = await _db
			.Students
			.GroupBy(x => x.EnrollmentDate)
			.Select(x => new EnrollmentDateGroup
			{
				EnrollmentDate = x.Key,
				StudentCount = x.Count()
			})
			.ToListAsync();

		Data = groups;
	}
}