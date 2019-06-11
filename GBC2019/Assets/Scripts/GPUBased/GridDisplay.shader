Shader "Hidden/GridDisplay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ScalingFactor("ScalingFactor", Int) = 20
		_Width("ScalingFactor", Int) = 128
		_Height("ScalingFactor", Int) = 72

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			struct BlockStruct
			{
				int ID;
				int creationID;
				int type;
				int subType;
				int dead;
				int left;
				int right;
				int up;
				int down;
				int centerX;
				int centerY;
				int velocityX;
				int velocityY;
				int moveTicks;
				int currentTick;
				int canMove;
				int attachToBlockId;
				int attatched;
				int absorbPriority;
				int absorbType;
				int ignoreType;
				int canAbsorb;
				int CanBeAbsorbed;
				int KillNonMatching;
				int KillableByNonMatching;
			};

			struct CellStruct
			{
				int ID;
				int gridPosX;
				int gridPosY;
				int parentBlockID;
				int dying;
				int dead;
				int left;
				int right;
				int up;
				int down;
				int rotTicks;
				float lastX;
				float lastY;
			};

			struct GridCell
			{
				int cells[10];
			};

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			int _ScalingFactor;
			int _Width;
			int _Height;
			RWStructuredBuffer<BlockStruct> blockBuffer : register(u1);
			RWStructuredBuffer<CellStruct> cellBuffer : register(u2);
			RWStructuredBuffer<GridCell> grid : register(u3);

			bool InGridBounds(float x, float y)
			{
				return x >= 0 && x < width && y >= 0 && y < height;
			}

			float4 GetColorFromType(int blockType)
			{
				if (blockType == 1)
				{
					return float4(1, 0, 0, 1);
				}
				if (blockType == 2)
				{
					return float4(0, 1, 0, 1);
				}
				if (blockType == 3)
				{
					return float4(0, 0, 1, 1);
				}
				if (blockType == 4)
				{
					return float4(1, 0.75, 0.016, 1);
				}

				return float4(1, 1, 1, 1);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				int x = floor(i.uv.x * scalingFactor);
				int y = floor(i.uv.y * scalingFactor);
				int gridId = width * y + x;

				if (InGridBounds(x, y))
				{
					for (int i = 0; i < 10; i++)
					{
						int cellId = grid[gridId].cells[i];
						float deltaX = floor(((float)x - cellBuffer[cellId].lastX) * scalingFactor);
						float deltaY = floor(((float)y - cellBuffer[cellId].lastY) * scalingFactor);
						if (cellId >= 0 && cellBuffer[cellId].dead == 1)
						{
							float u = (float)id.x / (float)scalingFactor - (float)x;
							float v = (float)id.y / (float)scalingFactor - (float)y;
							//Result[id.xy - int2(deltaX, deltaY)] = 
							cellBuffer[cellId].lastX = cellBuffer[cellId].lastX * 0.99 + (float)x * 0.01;
							cellBuffer[cellId].lastY = cellBuffer[cellId].lastY * 0.99 + (float)y * 0.01;
							return tex2D(_MainTex, float2(u, v)) * GetColorFromType(blockBuffer[cellBuffer[cellId].parentBlockID].subType);
						}
					}
				}
				return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
