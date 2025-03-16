using HoaiNXRazorPages.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Application.ServiceContracts
{
    public  interface ITagService
    {
        Task<IEnumerable<Tag>> GetTagsAsync();
        Task<Tag> GetTagByIdAsync(short tagId);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagByIdAsync(short id);
        Task<Tag> GetTagByNameAsync(string tagName);
    }
}
