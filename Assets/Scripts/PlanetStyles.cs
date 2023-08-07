using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetStyles : ScriptableObject
{
    [System.Serializable]
    struct ColorStyle
    {
        public Color body;
        public Color fresnel;
    }


    [Header("Surface")]
    [SerializeField] ColorStyle[] colorStyles = default;
    [SerializeField] float[] bumpScales = default;
    [SerializeField] float[] bumpIntensities = default;

    [Space(20)]
    [Header("Cloud")]
    [SerializeField] float[] cloudScales = default;
    [Tooltip("This does not actually represent alphaclips. the previous functionality is deprecated. It now represnet transparency")]
    [SerializeField] float[] cloudAlphaClips = default;
    [SerializeField] Color[] cloudColors = default;
    [SerializeField] float[] cloudSizes = default;
    [SerializeField] float[] cloudSpeeds = default;

    [SerializeField] float minCloudSizeMultiplier = 1.5f;


    [Space(20)]
    [Header("Objects On Surface")]
    [SerializeField] GameObject[] ObjectsOnTheSurface;
    [SerializeField] float objectScale = 0.1f;
    [SerializeField] Vector2Int objectCount = default;


    [Space(20)]
    [Header("Outline Setting")]
    public OutlineColorStyle outlineColorStyle;


    [System.Serializable]
    public struct OutlineColorStyle
    {
        [ColorUsage(true,true)] public Color attacker;
        [ColorUsage(true, true)] public Color defender;
        [ColorUsage(true, true)] public Color add;
    }


    public class Style
    {
        public Color bodyColor = Color.white;
        public Color fresnelColor = Color.white;
        public float bumpScale = 20;
        public float bumpIntensity;
        public Color cloudColor;
        public float cloudAlphaClip;
        public float cloudSpeed;
        public float cloudSize;
        public float cloudScale;
    }


    public Style RandomizeStyle()
    {
        Style output = new Style();

        ColorStyle colorStyle = colorStyles[Random.Range(0, colorStyles.Length)];
        float bumpScale = bumpScales[Random.Range(0, bumpScales.Length)];
        float bumpIntensity = bumpIntensities[Random.Range(0, bumpIntensities.Length)];
        float cloudScale = cloudScales[Random.Range(0, cloudScales.Length)];
        float cloudAlphaClip = cloudAlphaClips[Random.Range(0, cloudAlphaClips.Length)];
        float cloudSize = cloudSizes[Random.Range(0, cloudSizes.Length)];
        Color cloudColor = cloudColors[Random.Range(0, cloudColors.Length)];
        float cloudSpeed = cloudSpeeds[Random.Range(0, cloudSpeeds.Length)];

        if(cloudSize / bumpIntensity < minCloudSizeMultiplier)
        {
            cloudSize = bumpIntensity * minCloudSizeMultiplier;
        }

        output.bodyColor = colorStyle.body;
        output.fresnelColor = colorStyle.fresnel;
        output.bumpScale = bumpScale;
        output.bumpIntensity = bumpIntensity;
        output.cloudScale = cloudScale;
        output.cloudAlphaClip = cloudAlphaClip;
        output.cloudSize = cloudSize;
        output.cloudColor = cloudColor;
        output.cloudSpeed = cloudSpeed;

        return output;
    }


    public void CreateObjectsOnTheSurface(Transform transform, Style style)
    {
        int t = Random.Range(objectCount.x, objectCount.y);
        GameObject objects = new GameObject();
        objects.transform.position = transform.position;
        objects.transform.SetParent(transform);

        for (int i = 0; i < t; i++)
        {
            Vector3 dir = Random.insideUnitSphere;
            GameObject obj = Instantiate(ObjectsOnTheSurface[Random.Range(0, ObjectsOnTheSurface.Length)], transform.position, Quaternion.identity);

            obj.transform.rotation = obj.transform.rotation * Quaternion.FromToRotation(obj.transform.up, dir);
            obj.transform.position = transform.position + dir * (0.5f + style.bumpIntensity / 1.25f);
            obj.transform.localScale = Vector3.one * objectScale;
            obj.transform.SetParent(objects.transform);
        }
    }
}
