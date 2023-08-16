using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace Necromancy.UI
{
    [Serializable]
    public struct SymbolsTextureData
    {
        //Link to the font atlas
        public Texture texture;
        //An array of character sets in order starting from the top left
        public char[] chars;

        //Dictionary with the coordinates of each character – a row and a column number
        private Dictionary<char, Vector2> charsDict;

        public void Initialize()
        {
            charsDict = new Dictionary<char, Vector2>();
            for (int i = 0; i < chars.Length; i++)
            {
                var c = char.ToLowerInvariant(chars[i]);
                if (charsDict.ContainsKey(c)) continue;
                //Calculation of the coordinates of the symbol, 
                //we transform the serial number of the symbol
                //into the row and column number, knowing that the row length is 10.
                var uv = new Vector2(i % 10, 9 - i / 10);
                charsDict.Add(c, uv);
            }
        }

        public Vector2 GetTextureCoordinates(char c)
        {
            c = char.ToLowerInvariant(c);
            if (charsDict == null) Initialize();

            if (charsDict.TryGetValue(c, out Vector2 texCoord))
                return texCoord;
            return Vector2.zero;
        }
    }

    [RequireComponent(typeof(ParticleSystem))]
    public class TextRendererParticleSystem  : MonoBehaviour
    {
        public SymbolsTextureData textureData;

        [SerializeField][TextArea] string testString;

        

        private ParticleSystemRenderer particleSystemRenderer;
        private new ParticleSystem particleSystem;        

        [ContextMenu("TestText")]
        public void TestText()
        {
            SpawnParticle(transform.position, testString, Color.red, 1);
        }

        public void SpawnParticle(Vector3 position, float amount, Color color, float? startSize = null)
        {
            var amountInt = Mathf.RoundToInt(amount);
            if (amountInt == 0) return;
            var str = amountInt.ToString();
            if (amountInt > 0) str = "+" + str;
            SpawnParticle(position, str, color, startSize);
        }
        
        public void SpawnParticle(Vector3 position, string message, Color color, float? startSize = null)
        {
            var texCords = new Vector2[24];
            //an array of 24 elements - 23 symbols + the length of the mesage
            var messageLenght = Mathf.Min(23, message.Length);
            texCords[texCords.Length - 1] = new Vector2(0, messageLenght);
            for (int i = 0; i < texCords.Length; i++)
            {
                if (i >= messageLenght) break;
                //Calling the method GetTextureCoordinates() from SymbolsTextureData to obtain the symbol's position
                texCords[i] = textureData.GetTextureCoordinates(message[i]);
            }

            var custom1Data = CreateCustomData(texCords);
            var custom2Data = CreateCustomData(texCords, 12);

            //Кэшируем ссылку на ParticleSystem
            if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();

            if (particleSystemRenderer == null)
            {
                //Если ссылка на ParticleSystemRenderer, кэшируем и убеждаемся в наличии нужных потоков
                particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                var streams = new List<ParticleSystemVertexStream>();
                particleSystemRenderer.GetActiveVertexStreams(streams);
                //Добавляем лишний поток Vector2(UV2, SizeXY, etc.), чтобы координаты в скрипте соответствовали координатам в шейдере
                if (!streams.Contains(ParticleSystemVertexStream.UV2)) streams.Add(ParticleSystemVertexStream.UV2);
                if (!streams.Contains(ParticleSystemVertexStream.Custom1XYZW)) streams.Add(ParticleSystemVertexStream.Custom1XYZW);
                if (!streams.Contains(ParticleSystemVertexStream.Custom2XYZW)) streams.Add(ParticleSystemVertexStream.Custom2XYZW);
                particleSystemRenderer.SetActiveVertexStreams(streams);
            }

            //Initializing emission parameters
            //The color and position are obtained from the method parameters
            //Set startSize3D to X so that the characters are not stretched or compressed
            //when changing the length of the message
            var emitParams = new ParticleSystem.EmitParams
            {
                //startColor = Color.Lerp(color,particleSystem.main.startColor.color,0.5f),
                position = position,
                applyShapeToPosition = true,
                startSize3D = new Vector3(messageLenght, 1, 1)
            };
            //If we want to create particles of different sizes, then in the parameters of SpawnParticle it is necessary
            //to transfer the desired startSize value
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

        //Vector2 array packing function with symbols' coordinates in "float" 
        public float PackFloat(Vector2[] vecs)
        {
            if (vecs == null || vecs.Length == 0) return 0;
            //Bitwise adding the coordinates of the vectors in float
            var result = vecs[0].y * 10000 + vecs[0].x * 100000;
            if (vecs.Length > 1) result += vecs[1].y * 100 + vecs[1].x * 1000;
            if (vecs.Length > 2) result += vecs[2].y + vecs[2].x * 10;
            return result;
        }

        //Create Vector4 function for the stream with CustomData
        private Vector4 CreateCustomData(Vector2[] texCoords, int offset = 0)
        {
            var data = Vector4.zero;
            for (int i = 0; i < 4; i++)
            {
                var vecs = new Vector2[3];
                for (int j = 0; j < 3; j++)
                {
                    var ind = i * 3 + j + offset;
                    if (texCoords.Length > ind)
                    {
                        vecs[j] = texCoords[ind];
                    }
                    else
                    {
                        data[i] = PackFloat(vecs);
                        i = 5;
                        break;
                    }
                }
                if (i < 4) data[i] = PackFloat(vecs);
            }
            return data;
        }
    }
}