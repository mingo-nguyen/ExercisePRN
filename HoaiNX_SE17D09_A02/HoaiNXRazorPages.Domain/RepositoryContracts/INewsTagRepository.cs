using HoaiNXRazorPages.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Domain.RepositoryContracts;

public interface INewsTagRepository
{
    Task<IEnumerable<NewsTag>> GetNewsTagsAsync();
    Task<NewsTag> GetNewsTagByIdAsync(int newsTagId);
    Task AddNewsTagAsync(NewsTag newsTag);
    Task UpdateNewsTagAsync(NewsTag newsTag);
    Task DeleteNewsTagAsync(NewsTag newsTag);
}
