using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HoaiNXRazorPages.Domain.Entities;

public partial class SystemAccount
{
    public short AccountId { get; set; }
    [Required]
    public string? AccountName { get; set; }
    [Required]
    public string? AccountEmail { get; set; }
    
    public int? AccountRole { get; set; }

    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
