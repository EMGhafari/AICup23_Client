Shader "Custom/TextParticles"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	//The number of rows and columns in theory can be less than 10, but definitely not more
		_Cols("Columns Count", Int) = 10
		_Rows("Rows Count", Int) = 10
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "PreviewType" = "Plane" "Queue" = "Transparent+1"}
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float4 uv : TEXCOORD0;
			//Those very vectors with customData
			float4 customData1 : TEXCOORD1;
			float4 customData2 : TEXCOORD2;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float4 uv : TEXCOORD0;
			float4 customData1 : TEXCOORD1;
			float4 customData2 : TEXCOORD2;
		};

		uniform sampler2D _MainTex;
		uniform uint _Cols;
		uniform uint _Rows;

		v2f vert(appdata v)
		{
			v2f o;
			//Why is the message length transmitted in the last bits of the w-coordinate of the vector?
			//This is the easiest way to get this length inside the shader.
			//It is enough to get the remainder of dividing by 100.
			float textLength = ceil(fmod(v.customData2.w, 100));

			o.vertex = UnityObjectToClipPos(v.vertex);
			//Getting the size of the UV-texture based on the number of rows and columns
			o.uv.xy = v.uv.xy * fixed2(textLength / _Cols, 1.0 / _Rows);
			o.uv.zw = v.uv.zw;
			o.color = v.color;
			o.customData1 = floor(v.customData1);
			o.customData2 = floor(v.customData2);
			return o;
		}

		fixed4 frag(v2f v) : SV_Target
		{
			fixed2 uv = v.uv.xy;
			//Symbol's index in the message
			uint ind = floor(uv.x * _Cols);

			uint x = 0;
			uint y = 0;

			//Vector coordinate index containing this element
			//0-3 - customData1
			//4-7 - customData2
			uint dataInd = ind / 3;
			//We get the value of all 6 bits packed in the desired "float"
			uint sum = dataInd < 4 ? v.customData1[dataInd] : v.customData2[dataInd - 4];

			//Directly unpacking of the "float" and getting the row and the column character
			for (int i = 0; i < 3; ++i)
			{
				if (dataInd > 3 & i == 3) break;
				//rounding to a larger value, otherwise we will get 10^2 = 99 etc.
				uint val = ceil(pow(10, 5 - i * 2));
				x = sum / val;
				sum -= x * val;

				val = ceil(pow(10, 4 - i * 2));
				y = sum / val;
				sum -= floor(y * val);

				if (dataInd * 3 + i == ind) i = 3;
			}

			float cols = 1.0 / _Cols;
			float rows = 1.0 / _Rows;
			//Shifting the UV-coordinates using the number of rows, columns, index and
			//row and column number of the element
			uv.x += x * cols - ind * rows;
			uv.y += y * rows;

			return tex2D(_MainTex, uv.xy) * v.color;
		}
		ENDCG
		}
	}
}