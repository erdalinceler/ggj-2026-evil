using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcOutfitData", menuName = "Outfit/NPC Outfit Data")]
public class NpcOutfitData : ScriptableObject
{
    public Gender gender;
    public List<Sprite> hair = new List<Sprite>();
    public List<Sprite> eye = new List<Sprite>();
    public List<Sprite> nose = new List<Sprite>();
    public List<Sprite> body = new List<Sprite>();
    public List<Sprite> cloth = new List<Sprite>();
    public List<Sprite> earrings = new List<Sprite>();
    public List<Sprite> beard = new List<Sprite>();
}
