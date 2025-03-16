using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using HoaiNXRazorPages.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        
    public async Task AddTagAsync(Tag tag)
        {
            var tagExists = await _tagRepository.GetTagsAsync();
            int maxId = tagExists.Any() ? tagExists.Max(a => a.TagId) : (int)0;
            tag.TagId = maxId + 1;

            await _tagRepository.AddTagAsync(tag);
        }

        public Task DeleteTagByIdAsync(short id)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetTagByIdAsync(short tagId)
        {
            throw new NotImplementedException();
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await _tagRepository.GetTagByNameAsync(tagName);
        }

        public Task<IEnumerable<Tag>> GetTagsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}
