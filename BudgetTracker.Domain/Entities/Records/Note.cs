using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.Entities.Records;

public sealed record Note
{
    public string Text { get; }

    public Note(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("Note text cannot be null or empty", "INVALID_NOTE");

        if (text.Length > 500)
            throw new DomainException("Note text cannot exceed 500 characters", "NOTE_TOO_LONG");

        Text = text.Trim();
    }

    public static implicit operator string(Note note) => note.Text;
    public static implicit operator Note(string text) => new(text);
}