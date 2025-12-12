using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FBX", menuName = "Sounds/Fbx", order = 1)]
public class FBX : ScriptableObject
{
    public string fbxName;
    public AudioClip audioClip;
}
