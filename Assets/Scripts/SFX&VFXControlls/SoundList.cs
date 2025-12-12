using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SoundList", menuName = "Sounds/SoundsList", order = 1)]
public class SoundList : ScriptableObject
{
    [SerializeField] private List<FBX> fbx;

    public Dictionary<string, AudioClip> fbxDictionary = new();

    public void MakeFBXDictionary()
    {
        fbxDictionary.Clear();
        foreach (var item in fbx)
        {
            fbxDictionary.Add(item.fbxName, item.audioClip);
        }
    }
}
