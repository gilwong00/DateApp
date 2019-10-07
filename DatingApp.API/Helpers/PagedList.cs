using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
	public class PagedList<T> : List<T>
	{
		public int CurrentPage { get; set; }

		public int TotalPages { get; set; }

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize); 
			this.AddRange(items);
		}

		public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
		{
			// total count of the collection
			var count = await source.CountAsync(); 
			// determine which and how many items to send back ie page 1 returns index 0-4, page 2 returns 5-9
			var items = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ToListAsync(); 
			return new PagedList<T>(items, count, pageNumber, pageSize);
		}
	}
}