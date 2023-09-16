using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/Player Styles")]
public class PlayerStyles : ScriptableObject
{
    [SerializeField] Texture2D[] AvatarsToChoose;
    [SerializeField] [ColorUsage(true, true)] Color[] ColorsToChoose;

    [SerializeField] List<PlayerStyleReference> playerStyles = new List<PlayerStyleReference>();
    
    [System.Serializable]
    public struct PlayerStyleReference
    {
        public int avatarIndex;
        public int colorIndex;
        public string name;
    }

    [System.Serializable]
    public struct PlayerStyle
    {
        public PlayerStyle(Texture2D avatarTex, Color color, string name)
        {
            this.color = color;
            this.avatar = avatarTex;
            this.name = name;
        }
        public Texture2D avatar;
        public Color color;
        public string name;
    }

    public PlayerStyle GetStyle(int index)
    {
        return new PlayerStyle(AvatarsToChoose[playerStyles[index].avatarIndex], ColorsToChoose[playerStyles[index].colorIndex], playerStyles[index].name);
    }
    public PlayerStyle GetStyle(PlayerStyleReference reference)
    {
        return new PlayerStyle(AvatarsToChoose[reference.avatarIndex], ColorsToChoose[reference.colorIndex], reference.name);
    }

    public void AddStyle(PlayerStyleReference reference)
    {
        playerStyles.Add(reference);
    }

    public void ChangeStyle(int index, string name)
    {
        playerStyles[index] = new PlayerStyleReference { avatarIndex = playerStyles[index].avatarIndex, colorIndex = playerStyles[index].colorIndex, name = name };
    }
}