namespace Snackis.Application.Interfaces;

public interface IProfanityFilter
{
    string Censor(string text, IReadOnlyList<string> bannedWords);
}
