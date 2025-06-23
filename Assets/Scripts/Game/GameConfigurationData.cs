using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameConfigData", menuName = "GameConfig/GlobalConfig")]
public class GameConfigurationData : ScriptableObject
{
    [Header("GameStateManager")]
    public string mainScene = "HomeScene";
    public float fadeTransitionDuration = 1.1f;
    public float circleTransitionDuration = 0.8f;

    [Header("SceneTransitionManager")]
    public List<string> scenesWithoutInTransition;
    public List<string> scenesWithoutOutTransition;
    public TransitionType defaultTransitionType = TransitionType.Circle;

    [Header("GameLogicManager")]
    public string sceneToDestroy = "MenuScene";
    public Vector3 newGameStartPosition = new Vector3(-25.5f, 12f, 0f);
    public int firstClueSubphaseIndex = 3;
    public int secondClueSubphaseIndex = 16;
    public int thirdClueSubphaseIndex = 25;

    [Header("GameUIManager")]
    [Header("Clues Sprites Section")]
    public Sprite firstClueSprite1;
    public Sprite firstClueSprite2;
    public Sprite secondClueSprite1;
    public Sprite secondClueSprite2;
    public Sprite thirdClueSprite1;
    public Sprite thirdClueSprite2;

    [Header("Suspects Sprites Section")]
    public Sprite suspectSprite1;
    public Sprite suspectSprite2;
    public Sprite suspectSprite3;
    public Sprite suspectSprite4;
    public Sprite suspectSprite5;
    public Sprite suspectSprite6;
    public Sprite suspectSprite7;
    public Sprite suspectSprite8;

    [Header("Default Sprite Section")]
    public Sprite defaultSprite;

    [Header("Cursors Textures Section")]
    public Texture2D defaultCursor;
    public Texture2D interactCursor;
    
    [Header("Variable Section")]
    public Vector2 hotspot = Vector2.zero;
}