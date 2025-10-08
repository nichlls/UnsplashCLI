namespace UnsplashCLI.Models.Unsplash;

public class UnsplashPhoto
{
    public string Id { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AltDescription { get; set; }
    public UnsplashUser? User { get; set; }
    public UnsplashUrls? Urls { get; set; }
}

public class UnsplashSearchResult
{
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public List<UnsplashPhoto>? Results { get; set; }
}

public class UnsplashUser
{
    public string? Name { get; set; }
    public string? Username { get; set; }
}

public class UnsplashUrls
{
    public string? Small { get; set; }
    public string? Regular { get; set; }
    public string? Full { get; set; }
}
