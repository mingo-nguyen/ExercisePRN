using HoaiNXRazorPages.Domain.Entities;


namespace HoaiNXRazorPages.Domain.RepositoryContracts
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetTagsAsync();
        Task<Tag> GetTagByIdAsync(int tagId);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(Tag tag);
        Task<Tag> GetTagByNameAsync(string tagName);
    }
}
