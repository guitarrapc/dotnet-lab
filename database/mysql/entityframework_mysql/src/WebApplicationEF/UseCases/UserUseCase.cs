using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationEF.Data;

namespace WebApplicationEF.UseCases
{
    public interface IBlogUseCase
    {
        Blog[] Get(int userId);
    }

    public class UserUseCase : IBlogUseCase
    {
        private readonly BloggingContext dbContext;
        public UserUseCase(BloggingContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Blog[] Get(int blogId)
        {
            var users = dbContext.Blogs.Where(x => x.BlogId == blogId)
                .OrderBy(x => x.Url)
                .ToArray();
            return users;
        }
    }
}
