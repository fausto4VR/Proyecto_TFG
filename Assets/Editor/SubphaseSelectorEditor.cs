using UnityEditor;
using UnityEngine;

// Clase para gestionar el dropdown de fases de la historia en el editor
[CustomPropertyDrawer(typeof(SubphaseSelectorAttribute))]
public class SubphaseSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Se comprueba si salta un error de inicialización
        if (property == null || property.serializedObject == null || property.serializedObject.targetObject == null)
        return;

        // Se carga las fases de la historia si no están cargadas ya
        if (StoryStateManager.gameStory == null)
        {
            StoryStateManager.LoadGameStory();

            // Se refresca la jerarquía para reflejar cambios
            EditorApplication.RepaintHierarchyWindow(); 
        }

        // Se muestra un mensaje de error si las fases de la historia no se han cargado correctamente
        if (StoryStateManager.gameStory == null || StoryStateManager.gameStory.phases == null)
        {
            EditorGUI.HelpBox(position, "No se ha podido cargar la historia.", MessageType.Error);
            return;
        }
        
        // Se llama al método para crear la lista de subfases disponibles en el dropdown
        var subphases = StoryStateManager.CreateSubphasesList();

        // Se muestra una advertencia en el caso de que no haya subfases disponibles
        if (subphases.Count == 0)
        {
            EditorGUI.HelpBox(position, "No hay subfases definidas.", MessageType.Warning);
            return;
        }

        // Se establece el primer valor de la lista si el valor actual de la propiedad está vacío o no es válido
        if (string.IsNullOrEmpty(property.stringValue) || !subphases.Contains(property.stringValue))
        {
            property.stringValue = subphases[0];

            // Se asegura que Unity registre los cambios y los guarde en el objeto serializado
            property.serializedObject.ApplyModifiedProperties(); 

            // Se marca el objeto como modificado
            EditorUtility.SetDirty(property.serializedObject.targetObject); 
        }

        // Se obtiene el índice actual dentro de la lista de opciones
        int currentIndex = subphases.IndexOf(property.stringValue);

        // Se muestra el popup en el inspector con las opciones disponibles
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, subphases.ToArray());

        // Se actualiza el valor de la propiedad si el usuario selecciona una opción diferente
        if (newIndex != currentIndex)
        {
            property.stringValue = subphases[newIndex];

            // Se asegura que Unity registre los cambios y los guarde en el objeto serializado
            property.serializedObject.ApplyModifiedProperties();

            // Se marca el objeto como modificado
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}
