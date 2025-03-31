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
    Beginning, Trigger, Investigation, Dialogue, ReadyForPuzzle, Error 
}

// Enum de la relación tempral entre las subfases
public enum SubphaseTemporaryOrder
{
    IsBefore, IsAfter, IsCurrent, IsDistant, Error
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
        StoryPhase firstPhase = new StoryPhase(phaseName, storySubphases, null);
        
        return firstPhase;
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

        int currentSubphaseIndex = currentStoryPhase.storySubphases.IndexOf(currentStoryPhase.currentSubphase);

        if (currentSubphaseIndex == -1)
        {
            Debug.LogError($"No se encontró la subfase actual '{currentStoryPhase.currentSubphase}' en la fase '{currentStoryPhase.phaseName}'.");
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

        return new StoryPhase(ParseStringToPhaseOption(nextPhaseInfo.name), ObtainSubphases(nextPhaseInfo.subphases),
            currentStoryPhase.storySubphases.Last());
    }

    // Método para convertir un string en un tipo del enum StoryPhaseOption
    public static StoryPhaseOption ParseStringToPhaseOption(string nameString)
    {
        if (Enum.TryParse(nameString, out StoryPhaseOption phase))
        {
            Debug.Log("Fase convertida con éxito: " + phase);
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
    public StorySubphase currentSubphase { get; private set; }
    public List<string> subphaseObjectNames { get; private set; }
    private StorySubphase previousSubphase;

    public StoryPhase(StoryPhaseOption name, List<StorySubphase> subphases, StorySubphase previousSubphaseInput)
    {
        phaseName = name;
        storySubphases = new List<StorySubphase>(subphases);
        StorySubphase beginningSubPhase = new StorySubphase(StorySubphaseType.Beginning, name.ToString() + " Beginning");
        storySubphases.Insert(0, beginningSubPhase);
        currentSubphase = beginningSubPhase;
        previousSubphase = previousSubphaseInput;
        subphaseObjectNames = new List<string>();
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
            previousSubphase = currentSubphase;
            currentSubphase = storySubphases[currentIndex + 1];
            subphaseObjectNames.Clear();
        }
    }

    // Método para comprobar si la subfase dada es la actual
    public SubphaseTemporaryOrder CheckCurrentPhase(string currentPhaseString)
    {
        string[] parts = currentPhaseString.Split('.');

        if (parts.Length != 2)
        {
            Debug.LogError("La cadena de la fase no está en el formato correcto (fase.subfase).");
            return SubphaseTemporaryOrder.Error;
        }

        string phaseNameString = parts[0];
        string subphaseNameString = parts[1];
        StoryPhaseOption phaseNameEnum = StoryStateManager.ParseStringToPhaseOption(phaseNameString);

        if (phaseName != phaseNameEnum)
        {
            bool isFirstSubphase = currentSubphase == storySubphases.First();
            bool isLastSubphase = currentSubphase == storySubphases.Last();
            bool isPreviousPhase = (int)phaseNameEnum == (int)phaseName - 1;
            bool isNextPhase = (int)phaseNameEnum == (int)phaseName + 1;

            if (isFirstSubphase && isPreviousPhase && previousSubphase != null && previousSubphase.subphaseName == subphaseNameString)
            {
                return SubphaseTemporaryOrder.IsBefore;
            }
            
            if (isLastSubphase && isNextPhase)
            {
                var nextPhase = StoryStateManager.gameStory.phases
                    .FirstOrDefault(p => p.name == phaseNameString);

                if (nextPhase != null && nextPhase.subphases != null 
                    && nextPhase.subphases.FirstOrDefault()?.name == subphaseNameString)
                {
                    return SubphaseTemporaryOrder.IsAfter;
                }
            }

            return SubphaseTemporaryOrder.IsDistant;
        }

        int comparedSubphaseIndex = storySubphases.FindIndex(s => s.subphaseName == subphaseNameString);
        int currentSubphaseIndex = storySubphases.IndexOf(currentSubphase);

        if (comparedSubphaseIndex == -1)
        {
            Debug.LogError($"La subfase '{subphaseNameString}' no existe en la fase actual.");
            return SubphaseTemporaryOrder.Error;
        }

        if (comparedSubphaseIndex < currentSubphaseIndex)
            return SubphaseTemporaryOrder.IsBefore;

        if (comparedSubphaseIndex > currentSubphaseIndex)
            return SubphaseTemporaryOrder.IsAfter;

        return SubphaseTemporaryOrder.IsCurrent;
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