// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader"
{
    Properties
    {
         _BodyColor("Body Color",color) = (1,1,1,1)
         _OutlineColor("Outline Color", color) = (0,0,0,0)
         _AccentColor("Accent Color", color) = (0,0,0,0)
         _Radius("Radius",Range(0,2)) = 0
         _OutlineThickness("OutlineThickness",Range(0,0.5)) = 0
         _TopDividerOffset("TopDividerOffset", Range(0,1)) = 0.5 
         _BottomDividerOffset("BottomDividerOffset",Range(0,1)) = 0.5
         _BackColor("BackColor",color) = (0,0,0,0)
         _Alpha ("Alpha", Range(0,1)) = 1

         [Toggle(_DividerOutline)] _UseDividers("Use Divider Outline", Float) = 1


    }
        SubShader
    {

        Pass //outline pass
        {

            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off


            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

        
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _Radius;
            float _OutlineThickness;
            float4 _OutlineColor;
            float4 _BodyColor;
            float4 _AccentColor;
            float4 _BackColor;
            float _TopDividerOffset;
            float _BottomDividerOffset;
            float _UseDividers;
            float _Alpha;

            fixed4 frag(v2f i,  fixed face : VFACE) : SV_TARGET
            {

               float2 uv = i.uv * 2 - 1;
               float xy = uv.x * uv.x + uv.y * uv.y;
               if (xy > _Radius || xy < 1 - _Radius)
               {
                   discard;
               }

               _BodyColor.a = _Alpha;
               _OutlineColor.a = _Alpha;
               _AccentColor.a = _Alpha;
               _BackColor.a = _Alpha;

               float Radius2 = _Radius - _OutlineThickness;     
               if (xy > Radius2 || xy < 1 - Radius2) return _OutlineColor;
               
               _OutlineThickness *= 0.5;
               if (uv.x < -1 + _OutlineThickness || uv.x > 1 - _OutlineThickness || uv.y < -1 + _OutlineThickness || uv.y > 1 - _OutlineThickness) return _OutlineColor;
               
               if(face < 0) return _BackColor;

               float lowerBound = _TopDividerOffset - _OutlineThickness;

               if (_UseDividers == 0) _OutlineColor = _BodyColor;

  
               if (uv.y > lowerBound) return _AccentColor;
               if (uv.y > lowerBound - _OutlineThickness) return _OutlineColor;

               float upperBound = -1 + _BottomDividerOffset + _OutlineThickness;
               if (uv.y < upperBound) return _AccentColor;
               if (uv.y < upperBound + _OutlineThickness) return _OutlineColor;

               return _BodyColor;
            }

            ENDCG

        }

    }
}
