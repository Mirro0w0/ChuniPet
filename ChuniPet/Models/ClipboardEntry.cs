namespace ChuniPet.Models;

using SQLite;

public class ClipboardEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Content { get; set; } = "";
    public string Preview { get; set; } = "";
    public bool HasLineBreaks { get; set; }
    public DateTime CopiedAt { get; set; }
}