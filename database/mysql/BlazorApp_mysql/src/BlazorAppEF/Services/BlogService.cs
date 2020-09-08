using BlazorAppEF.Models;
using BlazorAppEF.Shared.Models;
using System.Threading.Tasks;

namespace BlazorAppEF.Services
{
    public class BlogService
    {
        private readonly BlogModel _model;
        public BlogService(BlogModel model)
        {
            _model = model;
        }

        public Task<Blog[]> All()
        {
            return _model.GetAll();
        }

        public async Task Create(Blog blog)
        {
            await _model.Create(blog);
        }

        public async Task<Blog> Get(int id)
        {
            return await _model.GetByBlogId(id);
        }

        public async Task Delete(int id)
        {
            await _model.Delete(id);
        }
    }
}
