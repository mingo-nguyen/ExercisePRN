
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using HoaiNXRazorPages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace HoaiNXRazorPages.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly MyDbContext _context;
        public TagRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public Task DeleteTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetTagByIdAsync(int tagId)
        {
            throw new NotImplementedException();
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
        }

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public Task UpdateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}
