using UnityEditor;
using UnityEngine;
using System.Linq;

// Clase para gestionar el dropdown de fases de la historia en el editor
[CustomPropertyDrawer(typeof(SubphaseSelectorAttribute))]
public class SubphaseSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (StoryStateManager.gameStory == null)
        {
            StoryStateManager.LoadGameStory();
        }

        if (StoryStateManager.gameStory == null || StoryStateManager.gameStory.phases == null)
        {
            EditorGUI.HelpBox(position, "No se ha podido cargar la historia.", MessageType.Error);
            return;
        }

        var subphases = StoryStateManager.gameStory.phases
            .SelectMany(phase => phase.subphases.Select(subphase => $"{phase.name}.{subphase.name}"))
            .ToList();

        if (subphases.Count == 0)
        {
            EditorGUI.HelpBox(position, "No hay subfases definidas.", MessageType.Warning);
            return;
        }

        int currentIndex = subphases.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, subphases.ToArray());

        if (newIndex != currentIndex)
        {
            property.stringValue = subphases[newIndex];
        }
    }
}
