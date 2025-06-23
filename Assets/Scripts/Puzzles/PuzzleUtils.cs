using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;

public static class PuzzleUtils
{
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

    // Método estático para eliminar caracteres no alfanuméricos de una cadena
    public static string RemoveNonAlphanumeric(string textToRemoveCharacters)
    {
        textToRemoveCharacters = Regex.Replace(textToRemoveCharacters, @"[^\w]", "");
        textToRemoveCharacters = Regex.Replace(textToRemoveCharacters, @"[_ªº]", "");
        return textToRemoveCharacters;
    }

    // Método estático para eliminar caracteres no numéricos de una cadena
    public static string RemoveNonNumeric(string textToRemoveCharacters)
    {
        textToRemoveCharacters = Regex.Replace(textToRemoveCharacters, @"[^\d]", "");
        return textToRemoveCharacters;
    }

    // Método estático para actualizar la lista de los toggles activos
    public static void UpdateToggleList(Toggle toggle, List<string> list)
    {
        if (toggle.isOn)
        {
            if (!list.Contains(toggle.name)) list.Add(toggle.name);
        }
        else
        {
            if (list.Contains(toggle.name)) list.Remove(toggle.name);
        }
    }

    // Método estático para actualizar la lista de los botones activos
    public static int UpdateButtonList(Button button, List<string> list, int maxElements)
    {
        Image buttonImage = button.GetComponent<Image>();

        // Si la imagen es visible
        if (buttonImage.color.a > 0)
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0);
            maxElements -= 1;
            list.Remove(button.name);
        }
        // Si la imagen no es visible
        else
        {
            if(maxElements < 5)
            {
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1);
                maxElements += 1;
                if (!list.Contains(button.name)) list.Add(button.name);
            }
        }

        return maxElements;
    }

    // Método estático para comprobar la solución de un puzle donde hay que mirar la activación de los elementos
    public static bool ValidateDisplaySolution(List<string> activeElements, List<string> requiredElements, string filter = null)
    {
        if (!string.IsNullOrEmpty(filter))
            activeElements = activeElements.Where(e => e.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

        if (activeElements.Count != requiredElements.Count) return false;

        foreach (string required in requiredElements)
        {
            bool found = activeElements.Any(a => a.IndexOf(required, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!found) return false;
        }

        return true;
    }
}
