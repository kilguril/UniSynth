using UnityEngine;
using System.Collections;

namespace UniSynth2.Editor.Utils
{
	public class SynthGraphEditorUtils
	{
		private static Material m_lineMaterial;
		private static Material LineMaterial
		{
			get { return ( m_lineMaterial != null ? m_lineMaterial : CreateMaterial() ); }
			set { m_lineMaterial = value; }
		}
		
		public static void DrawSingleLine( Vector2 from, Vector2 to, Color color )
		{
			BeginLines( color );
			DrawLine( from, to );
			EndLines();
		}
		
		public static void BeginLines( Color color )
		{
			GL.PushMatrix();
			
			LineMaterial.SetPass( 0 );
			
			GL.LoadPixelMatrix();		
			GL.Begin( GL.LINES );
			GL.Color( color );
		}
		
		public static void DrawLine( Vector2 from, Vector2 to )
		{
			GL.Vertex( (Vector3)from );
			GL.Vertex( (Vector3)to );	
		}
		
		public static void EndLines()
		{
			GL.End();
			GL.PopMatrix();
		}

		private static Material CreateMaterial()
		{
			m_lineMaterial = new Material
				( 
				 "Shader \"Lines/VertexColored\" {" +
				 "SubShader { Pass { " +
				 "    ZWrite Off Cull Off Fog { Mode Off } " +
				 "    BindChannels {" +
				 "      Bind \"vertex\", vertex Bind \"color\", color }" +
				 "} } }" 
				 );
			
			m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;	
			
			return m_lineMaterial;
		}
	}
}