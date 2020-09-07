using BlazorAppEF.Data;
using BlazorAppEF.UseCases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppEF.Services
{
    public class BlogService
    {
        private readonly BlogModel _blogModel;
        public BlogService(BloggingContext dbContext)
        {
            _blogModel = new BlogModel(dbContext);

        }

        public Task<Blog[]> GetAll()
        {
            return _blogModel.GetAll();
        }
    }
}
