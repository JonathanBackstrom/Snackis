using Snackis.Application.Interfaces;
using Snackis.Domain.Entities;
using System.Text.RegularExpressions;

namespace Snackis.Application.Services;

public class ProfanityFilter : IProfanityFilter
{
    public string Censor(string text, IReadOnlyList<string> bannedWords)
    {
        if (string.IsNullOrEmpty(text) || bannedWords.Count == 0)
            return text;

        foreach (var word in bannedWords)
        {
            if (string.IsNullOrEmpty(word))
                continue;

            var pattern = $@"\b{Regex.Escape(word)}\b";

            text = Regex.Replace(text, pattern, new string('*', word.Length), RegexOptions.IgnoreCase);

        }

        return text;
    }
}
