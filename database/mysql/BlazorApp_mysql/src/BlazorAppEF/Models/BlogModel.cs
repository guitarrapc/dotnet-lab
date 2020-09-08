using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppEF.Data;
using BlazorAppEF.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEF.Models
{
    public interface IBlogModel
    {
        Task<Blog[]> GetAll();
        Task<Blog> GetByBlogId(int blogId);
        Task<int> Create(Blog blog);
        Task Edit(int id, Blog blog);
        Task Delete(int id);
        Task<bool> Exists(int id);
    }

    public class BlogModel : IBlogModel
    {
        private readonly BloggingContext _dbContext;
        public BlogModel(BloggingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Blog[]> GetAll()
        {
            return await _dbContext.Blogs
                .OrderBy(x => x.BlogId)
                .ToArrayAsync();
        }

        public async Task<Blog> GetByBlogId(int id)
        {
            return await _dbContext.Blogs.FindAsync(id);
        }

        public async Task<int> Create(Blog blog)
        {
            _dbContext.Add(blog);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task Edit(int id, Blog blog)
        {
            if (id != blog.BlogId)
                throw new ArgumentException($"{nameof(id)} not match {blog.BlogId}");
            if (await Exists(id))
            {
                _dbContext.Update(blog);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            if (await Exists(id))
            {
                var blog = await _dbContext.Blogs.FindAsync(id);
                _dbContext.Blogs.Remove(blog);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await _dbContext.Blogs.AnyAsync(e => e.BlogId == id);
        }
    }
}
