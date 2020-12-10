using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{

    CharacterSO character;

    RawImage image;
    
    // Start is called before the first frame update
    void Start()
    {
        image.texture = character.avatar;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
