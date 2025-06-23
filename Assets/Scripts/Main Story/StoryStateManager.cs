using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System;

// Enum de las fases de la historia
public enum StoryPhaseOption
{
    Prologue, Puzzle1, Puzzle2, Puzzle3, Puzzle4, Puzzle5, Puzzle6, Puzzle7, Puzzle8, Ending, Error
}

// Enum de los tipos de subfases
public enum StorySubphaseType
{
    Ending, Trigger, Investigation, Dialogue, ReadyForPuzzle, Error 
}

// Enum de la relación tempral entre las subfases
public enum SubphaseTemporaryOrder
{
    IsRecentBefore, IsDistantBefore, IsRecentAfter, IsDistantAfter, IsCurrent, Error
}

// Clase para gestionar el estado actual de la historia del juego
public static class StoryStateManager
{
    public static GameStoryDictionary gameStory { get; private set; }

    // Método para devolver la primera fase de la historia
    public static StoryPhase CreateFirstPhase()
    {
        if (gameStory == null || gameStory.phases == null || gameStory.phases.Count == 0)
        {
            Debug.LogError("No se puede crear la primera fase: la historia del juego no está inicializada o no tiene fases.");
            return null;
        }

        StoryPhaseOption phaseName = ParseStringToPhaseOption(gameStory.phases[0].name);
        List<StorySubphase> storySubphases = ObtainSubphases(gameStory.phases[0].subphases);
        StoryPhase firstPhase = new StoryPhase(phaseName, storySubphases);
        
        return firstPhase;
    }

    // Método para devolver la última fase de la historia
    public static StoryPhase CreateLastPhase()
    {
        if (gameStory == null || gameStory.phases == null || gameStory.phases.Count == 0)
        {
            Debug.LogError("No se puede crear la última fase: la historia del juego no está inicializada o no tiene fases.");
            return null;
        }

        StoryPhaseInformation lastPhaseInfo = gameStory.phases[gameStory.phases.Count - 1];
        StoryPhaseOption phaseName = ParseStringToPhaseOption(lastPhaseInfo.name);
        List<StorySubphase> storySubphases = ObtainSubphases(lastPhaseInfo.subphases);
        StoryPhase lastPhase = new StoryPhase(phaseName, storySubphases);
        lastPhase.currentSubphase = lastPhase.storySubphases[lastPhase.storySubphases.Count - 1];

        return lastPhase;
    }

    // Método para crear la lista de fases y subfases
    public static List<string> CreateSubphasesList()
    {
        List<string> subphases = gameStory.phases
        .SelectMany(phase => phase.subphases.Select(subphase => $"{phase.name}.{subphase.name}")).ToList();

        return subphases;
    }

    // Método para avanzar la historia según corresponda
    public static StoryPhase AdvanceStory(StoryPhase currentStoryPhase, string objectName)
    {
        if (gameStory == null || gameStory.phases == null || gameStory.phases.Count == 0)
        {
            Debug.LogError("El listado de fases no está inicializado o no tiene fases.");
            return null;
        }

        int currentPhaseIndex = gameStory.phases.FindIndex(p => ParseStringToPhaseOption(p.name) == currentStoryPhase.phaseName);

        if (currentPhaseIndex == -1)
        {
            Debug.LogError($"La fase actual '{currentStoryPhase.phaseName}' no se encontró en el listado de fases.");
            return null;
        }

        int currentSubphaseIndex = currentStoryPhase.storySubphases.FindIndex(s => 
            s.subphaseName == currentStoryPhase.currentSubphase.subphaseName &&
            s.subphaseType == currentStoryPhase.currentSubphase.subphaseType &&
            s.actions == currentStoryPhase.currentSubphase.actions);

        if (currentSubphaseIndex == -1)
        {
            Debug.LogError($"No se encontró la subfase actual '{currentStoryPhase.currentSubphase.subphaseName}' en la fase '{currentStoryPhase.phaseName}'.");
            return null;
        }

        if (currentSubphaseIndex == currentStoryPhase.storySubphases.Count - 1)
        {        
            return NextStoryPhase(currentStoryPhase, currentPhaseIndex);
        }

        currentStoryPhase.NextStorySubphase(objectName);

        return currentStoryPhase;
    }

    // Método para devolver la siguiente fase de la historia
    public static StoryPhase NextStoryPhase(StoryPhase currentStoryPhase, int currentIndex)
    {
        if (currentIndex >= gameStory.phases.Count - 1)
        {
            Debug.Log("Se ha alcanzado el final de la historia.");
            return null;
        }

        StoryPhaseInformation nextPhaseInfo = gameStory.phases[currentIndex + 1];

        return new StoryPhase(ParseStringToPhaseOption(nextPhaseInfo.name), ObtainSubphases(nextPhaseInfo.subphases));
    }

    // Método para convertir un string en un tipo del enum StoryPhaseOption
    public static StoryPhaseOption ParseStringToPhaseOption(string nameString)
    {
        if (Enum.TryParse(nameString, out StoryPhaseOption phase))
        {
            return phase;
        }
        else
        {
            Debug.LogError("No se pudo convertir la fase.");
            return StoryPhaseOption.Error;
        }
    }
    
    // Método para obtener la lista de subfases a partir de una lista de strings
    private static List<StorySubphase> ObtainSubphases(List<StorySubphaseInformation> subphaseInfos)
    {
        List<StorySubphase> storySubphases = subphaseInfos.Select(subphase =>
        {
            if (!Enum.TryParse(subphase.type, out StorySubphaseType subphaseType))
            {
                Debug.LogWarning($"Tipo de subfase desconocido: {subphase.type}. Se asignará 'Error'.");
                subphaseType = StorySubphaseType.Error;
            }

            return new StorySubphase(subphaseType, subphase.name, subphase.actions);
        }).ToList();

        return storySubphases;
    }

    // Método para cargar la historia del juego desde el fichero json
    public static void LoadGameStory() 
    {
        string path = Path.Combine(Application.streamingAssetsPath, "GameStory.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameStory = JsonUtility.FromJson<GameStoryDictionary>(json);

            if (gameStory == null)
            {
                Debug.LogError("Hubo un error al parsear el archivo JSON GameStory.");
            }
        }
        else
        {
            Debug.LogError("No se ha encontrado el archivo GameStory.json en StreamingAssets.");
        }
    }

    // Estructura de datos que representa la lista de fases de la historia y sirve para parsear el JSON
    [System.Serializable]
    public class GameStoryDictionary
    {
        public List<StoryPhaseInformation> phases;
    }

    // Estructura de datos que representa una fase de la historia y sirve para parsear el JSON
    [System.Serializable]
    public class StoryPhaseInformation
    {
        public string name;
        public List<StorySubphaseInformation> subphases;
    }

    // Estructura de datos que representa una subfase de la historia y sirve para parsear el JSON
    [System.Serializable]
    public class StorySubphaseInformation
    {
        public string type;
        public string name;
        public int actions;
    }
}

// Clase que describe una fase de la historia 
public class StoryPhase
{
    public StoryPhaseOption phaseName { get; private set; }
    public List<StorySubphase> storySubphases { get; private set; }
    public StorySubphase currentSubphase { get; set; }
    public List<string> subphaseObjectNames { get; set; }

    public StoryPhase(StoryPhaseOption name, List<StorySubphase> subphases)
    {
        phaseName = name;
        storySubphases = new List<StorySubphase>(subphases);

        if (storySubphases.Count > 0) currentSubphase = storySubphases[0];
        else Debug.LogError($"La fase '{phaseName}' no tiene ninguna subfase.");

        subphaseObjectNames = new List<string>();
    }

    // Método para obtener un string de la fase en formato [fase].[subfase]
    public string GetPhaseToString()
    {
        string phaseString = $"{phaseName}.{currentSubphase.subphaseName}";
        return phaseString;
    }

    // Método para avanzar a la siguiente subfase dentro de esta fase
    public void NextStorySubphase(string objectName)
    {
        if (storySubphases == null || storySubphases.Count == 0)
        {
            Debug.LogError("No hay subfases disponibles para avanzar.");
            return;
        }

        int currentIndex = storySubphases.IndexOf(currentSubphase);

        if (currentIndex == -1 || currentIndex >= storySubphases.Count - 1)
        {
            Debug.Log("Ya estás en la última subfase o no hay subfase actual.");
            return;
        }

        if (!subphaseObjectNames.Contains(objectName))
        {
            subphaseObjectNames.Add(objectName);
        }

        if (subphaseObjectNames.Count >= currentSubphase.actions)
        {
            currentSubphase = storySubphases[currentIndex + 1];
            subphaseObjectNames.Clear();
        }
    }

    // Método para comprobar si la subfase dada es la actual
    public SubphaseTemporaryOrder ComparePhase(string phaseToCompare)
    {
        string[] parts = phaseToCompare.Split('.');

        if (parts.Length != 2)
        {
            Debug.LogError("La cadena de la fase no está en el formato correcto (fase.subfase): " + phaseToCompare);
            return SubphaseTemporaryOrder.Error;
        }

        List<string> subphases = StoryStateManager.CreateSubphasesList();

        string thisPhase= $"{phaseName}.{currentSubphase.subphaseName}";

        int thisIndex = subphases.IndexOf(thisPhase);
        int compareIndex = subphases.IndexOf(phaseToCompare);

        if (thisIndex == -1 || compareIndex == -1) return SubphaseTemporaryOrder.Error;

        if (thisIndex == compareIndex) return SubphaseTemporaryOrder.IsCurrent;
        else if (thisIndex == compareIndex + 1) return SubphaseTemporaryOrder.IsRecentBefore;
        else if (thisIndex > compareIndex) return SubphaseTemporaryOrder.IsDistantBefore;
        else if (thisIndex == compareIndex - 1) return SubphaseTemporaryOrder.IsRecentAfter;
        else if (thisIndex < compareIndex) return SubphaseTemporaryOrder.IsDistantAfter;

        return SubphaseTemporaryOrder.Error;
    }
}

// Clase que describe una subfase de una fase de la historia
public class StorySubphase
{
    public StorySubphaseType subphaseType { get; private set; }
    public string subphaseName { get; private set; }
    public int actions { get; private set; }

    public StorySubphase(StorySubphaseType type, string name, int actions = 1)
    {
        subphaseType = type;
        subphaseName = name;
        this.actions = actions;
    }
}