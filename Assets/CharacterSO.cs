using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterSO", menuName = "Core-Systems-2020-Git-Example/CharacterSO", order = 0)]
public class CharacterSO : ScriptableObject {
    public string Name;
    public Texture2D avatar;
    public string Title;
}