Shader "Hidden/Custom RP/Post FX Stack"{

    SubShader{
        Cull Off
        ZTest Always
        ZWrite Off

        HLSLINCLUDE
        #include "../ShaderLibrary/Common.hlsl"
        #include "PostFXStackPasses.hlsl"
        ENDHLSL

        // Pass{
        //     Name "Bloom Combine"

        //     HLSLPROGRAM
        //         #pragma target 3.5
        //         #pragma vertex DefaultPassVertex
        //         #pragma fragment BloomCombinePassFragment
        //     ENDHLSL
        // }

        Pass{
            Name "Bloom Add"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomAddPassFragment
            ENDHLSL
        }

        Pass{
            Name "Bloom Horizontal"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomHorizontalPassFragment
            ENDHLSL
        }
        
        Pass{
            Name "Bloom Prefilter"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomPrefilterPassFragment
            ENDHLSL
        }

        Pass{
            Name "Bloom Prefilter Firefiles"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomPrefilterFirefliesPassFragment
            ENDHLSL
        }

        Pass{
            Name "Bloom Scatter"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomScatterPassFragment
            ENDHLSL
        }

        Pass{
            Name "Bloom Scatter Final"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomScatterFinalPassFragment
            ENDHLSL
        }

        Pass{
            Name "Bloom Vertical"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment BloomVerticalPassFragment
            ENDHLSL
        }

        Pass{
            Name "Copy"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment CopyPassFragment
            ENDHLSL
        }

        Pass{
            Name "ColorGrading None"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment ColorGradingNonePassFragment
            ENDHLSL
        }

        Pass{
            Name "ColorGrading ACES"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment ColorGradingACESPassFragment
            ENDHLSL
        }

        Pass{
            Name "ColorGrading Neutral"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment ColorGradingNeutralPassFragment
            ENDHLSL
        }

        Pass{
            Name "ColorGrading Reinhard"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment ColorGradingReinhardPassFragment
            ENDHLSL
        }

        Pass{
            Name "Scanner Effect"

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex ScannerEffectPassVertex
                #pragma fragment ScannerEffectPassFragment
            ENDHLSL
        }

        Pass{
            Name "Final"

            Blend [_FinalSrcBlend] [_FinalDstBlend]

            HLSLPROGRAM
                #pragma target 3.5
                #pragma vertex DefaultPassVertex
                #pragma fragment FinalPassFragment
            ENDHLSL
        }

        Pass {
			Name "Final Rescale"

			Blend [_FinalSrcBlend] [_FinalDstBlend]
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex DefaultPassVertex
				#pragma fragment FinalPassFragmentRescale
			ENDHLSL
		}
    }
}
    