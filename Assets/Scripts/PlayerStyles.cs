using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/Player Styles")]
public class PlayerStyles : ScriptableObject
{
    [SerializeField] Texture2D[] AvatarsToChoose;
    [SerializeField][ColorUsage(false, true)] Color[] ColorsToChoose;

    List<PlayerStyleReference> playerStyles = new List<PlayerStyleReference>();

    public struct PlayerStyleReference
    {
        public int avatarIndex;
        public int colorIndex;
    }

    public struct PlayerStyle
    {
        public PlayerStyle(Texture2D avatarTex, Color color)
        {
            this.color = color;
            this.avatar = avatarTex;
        }
        public Texture2D avatar;
        public Color color;
    }

    public PlayerStyle GetStyle(int index)
    {
        return new PlayerStyle(AvatarsToChoose[playerStyles[index].avatarIndex], ColorsToChoose[playerStyles[index].colorIndex]);
    }
    public PlayerStyle GetStyle(PlayerStyleReference reference)
    {
        return new PlayerStyle(AvatarsToChoose[reference.avatarIndex], ColorsToChoose[reference.colorIndex]);
    }


    public void AddStyle(PlayerStyleReference reference)
    {
        playerStyles.Add(reference);
    }
}