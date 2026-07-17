namespace Domain.Products;

public sealed class CategoryGender
{
    public Guid CategoryId { get; set; }
    public Category Category { get; private set; } = null!;
    public Guid GenderId { get; set; }
    public Gender Gender { get; private set; } = null!;
    public string? PhotoFileName { get; private set; }

    public void SetPhoto(string fileName)
    {
        PhotoFileName = fileName;
    }

    public void RemovePhoto()
    {
        PhotoFileName = null;
    }
}
