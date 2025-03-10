using System.Text.RegularExpressions;
using System.Text;

// QUITAR
// Enum que tiene los posibles tipos de resultados proporcionados
public enum ResultType
{
    Empty, Failure, Success
}

public static class PuzzleUtils
{
    // Método estático para eliminar caracteres no alfanuméricos de una cadena
    public static string RemoveNonAlphanumeric(string textToRemoveCharacters)
    {
        textToRemoveCharacters = Regex.Replace(textToRemoveCharacters, @"[^\w]", "");
        textToRemoveCharacters = Regex.Replace(textToRemoveCharacters, @"[_ªº]", "");
        return textToRemoveCharacters;
    }

    // Método estático para convertir caracteres con acentos en su equivalente sin dicho acento. La ñ permanece igual
    public static string RemoveAccents(string textToRemoveAccents)
    {
        string normalizedString = textToRemoveAccents.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
