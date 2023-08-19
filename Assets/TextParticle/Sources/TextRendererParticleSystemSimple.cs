using System;
using System.Collections.Generic;
using UnityEngine;


    [Serializable]
    public struct SymbolsTextureData
    {
        //Link to the font atlas
        public Texture texture;
        //An array of character sets in order starting from the top left
        public char[] chars;

        //Dictionary with the coordinates of each character – a row and a column number
        private Dictionary<char, int> charsDict;

        public void Initialize()
        {
            charsDict = new Dictionary<char, int>();
            for (int i = 0; i < chars.Length; i++)
            {
                var c = char.ToLowerInvariant(chars[i]);
                if (charsDict.ContainsKey(c)) continue;
                charsDict.Add(c, i);
            }
        }

        public int GetTextureIndex(char c)
        {
            c = char.ToLowerInvariant(c);
            if (charsDict == null) Initialize();

            if (charsDict.TryGetValue(c, out int index))
                return index;
            return 0;
        }
    }

    [RequireComponent(typeof(ParticleSystem))]
    public class TextRendererParticleSystemSimple : MonoBehaviour
    {
        public SymbolsTextureData textureData;

        [SerializeField][TextArea] string testString;

        private ParticleSystemRenderer particleSystemRenderer;
        private new ParticleSystem particleSystem;

        [ContextMenu("TestText")]
        public void TestText()
        {
            textureData.Initialize();
            SpawnParticle(transform.position, testString, Color.red, 1);
        }

        public void SpawnParticle(Vector3 position, float amount, Color? color = null, float? startSize = null)
        {
            var amountInt = Mathf.RoundToInt(amount);
            if (amountInt == 0) return;
            var str = amountInt.ToString();
            if (amountInt > 0) str = "+" + str;
            SpawnParticle(position, str, color, startSize);
        }

        public void SpawnParticle(Vector3 position, string message, Color? color = null, float? startSize = null)
        {
            var texCords = new int[8];
            var messageLength = Mathf.Min(7, message.Length);
            texCords[texCords.Length - 1] = messageLength;
            for (int i = 0; i < texCords.Length; i++)
            {
                if (i >= messageLength) break;
                texCords[i] = textureData.GetTextureIndex(message[i]);
            }

            var custom1Data = new Vector4(texCords[0], texCords[1], texCords[2], texCords[3]);
            var custom2Data = new Vector4(texCords[4], texCords[5], texCords[6], texCords[7]);

           
            if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();

            if (particleSystemRenderer == null)
            {
                particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                var streams = new List<ParticleSystemVertexStream>();
                particleSystemRenderer.GetActiveVertexStreams(streams);
                if (!streams.Contains(ParticleSystemVertexStream.UV2)) streams.Add(ParticleSystemVertexStream.UV2);
                if (!streams.Contains(ParticleSystemVertexStream.Custom1XYZW)) streams.Add(ParticleSystemVertexStream.Custom1XYZW);
                if (!streams.Contains(ParticleSystemVertexStream.Custom2XYZW)) streams.Add(ParticleSystemVertexStream.Custom2XYZW);
                particleSystemRenderer.SetActiveVertexStreams(streams);
            }

            var emitParams = new ParticleSystem.EmitParams
            {
                startColor = color??particleSystem.main.startColor.color,
                position = position,
                applyShapeToPosition = true,
                startSize3D = new Vector3(messageLength, 1, 1)
            };
            if (startSize.HasValue) emitParams.startSize3D *= startSize.Value * particleSystem.main.startSizeMultiplier;

            //Directly the spawn of the particles
            particleSystem.Emit(emitParams, 1);

            //Transferring the custom data to the needed streams
            var customData = new List<Vector4>();
            //Getting the stream ParticleSystemCustomData.Custom1 from ParticleSystem
            particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
            //Changing the data of the last element, i.e. the particle, that we have just created
            customData[customData.Count - 1] = custom1Data;
            //Returning the data to ParticleSystem
            particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

            //The same for ParticleSystemCustomData.Custom2
            particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
            customData[customData.Count - 1] = custom2Data;
            particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
        }
    }