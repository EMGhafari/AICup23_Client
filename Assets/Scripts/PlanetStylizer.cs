using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlanetStylizer : MonoBehaviour
{
    [SerializeField] PlanetStyles styleSheet;

    MeshRenderer meshRenderer;
    PlanetStyles.Style style;


    MaterialPropertyBlock SurfaceBlock;
    MaterialPropertyBlock CloudBlock; 
    MaterialPropertyBlock OutlineBlock;


    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        LoadARandomStyle(styleSheet);
    }

    void LoadARandomStyle(PlanetStyles sheet)
    {
        style = sheet.RandomizeStyle();
        sheet.CreateObjectsOnTheSurface(transform, style);

        SurfaceBlock = new MaterialPropertyBlock();
        CloudBlock = new MaterialPropertyBlock();
        OutlineBlock = new MaterialPropertyBlock();

        SurfaceBlock.SetColor("_Body_Color", style.bodyColor);
        SurfaceBlock.SetColor("_Fresnel_Color", style.fresnelColor);
        SurfaceBlock.SetFloat("_Bump_Scale", style.bumpScale);
        SurfaceBlock.SetFloat("_Bump_Intensity", style.bumpIntensity);

        CloudBlock.SetColor("_CloudColor", style.cloudColor);
        CloudBlock.SetFloat("_Transparency", style.cloudAlphaClip);
        CloudBlock.SetFloat("_CloudSpeed", style.cloudSpeed);
        CloudBlock.SetFloat("_Size", style.cloudSize);
        CloudBlock.SetFloat("_CloudScale", style.cloudScale);


        OutlineBlock.SetFloat("_Thickness", style.bumpIntensity + 0.05f);


        meshRenderer.SetPropertyBlock(SurfaceBlock, 0);
        if (meshRenderer.sharedMaterials.Length > 1) meshRenderer.SetPropertyBlock(CloudBlock, 1);
        if (meshRenderer.sharedMaterials.Length > 2) meshRenderer.SetPropertyBlock(OutlineBlock, 2);
    }


    public void SetOutline(int value, PlayerStyles.PlayerStyle player)
    {
        Color color = Color.white;
        color = player.color;
        OutlineBlock.SetFloat("_Enabled", value);
        OutlineBlock.SetColor("_Color", color);
        if (meshRenderer.sharedMaterials.Length > 2) meshRenderer.SetPropertyBlock(OutlineBlock, 2);
    }

    public void SetOutline(int value)
    {
        OutlineBlock.SetFloat("_Enabled", value);
        if (meshRenderer.sharedMaterials.Length > 2) meshRenderer.SetPropertyBlock(OutlineBlock, 2);
    }
}
