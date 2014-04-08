using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;

namespace UniSynth2.Editor
{
	public struct GUIButtonValue
	{
		public string m_title;
		public int    m_rval;
		
		public GUIButtonValue( string title, int rval )
		{
			m_title = title;
			m_rval  = rval;
		}
	}
	
	public class SynthGraphEditorFactory
	{
		// Some GUI related constants
		public const float		WINDOW_HEADER_HEIGHT   = 17.0f;
		
		public const float		NODE_WIDTH			   = 150.0f;
		public const float		NODE_LINE_HEIGHT	   = 20.0f;
		public const float		NODE_HEADER_HEIGHT	   = 20.0f;
		public const float		NODE_PADDING		   = 5.0f;
		
		public const float		NODE_CONNECTOR_SIZE	   = 15.0f;
		
		public const float		SPLIT_VIEW_HANDLE_SIZE = 20.0f;
		public const float		SPLIT_VIEW_RANGE_MIN   = 0.01f;
		public const float		SPLIT_VIEW_RANGE_MAX   = 0.99f;
		
		public const int		BUTTON_GROUP_NO_INPUT  = -1;
		
		public static readonly Color	COLOR_LINES	   = Color.white;
	
		#region Node GUI Controls
		// Get GUI control for node
		public static NodeGUIBase GetGUIControl( ISynthGraphNode node )
		{
			System.Type t = node.GetType();
		
			if ( t == typeof( SynthNodeGeneratorWave ) )	
			{
				return new NodeGUIGeneratorWave( node );
			}
			else if ( t == typeof( SynthNodeConstant ) )
			{
				return new NodeGUIConstant( node );
			}
			else if ( t == typeof( SynthNodeOperation ) )
			{
				return new NodeGUIOperation( node );
			}
			else if ( t == typeof( SynthNodeOutputMono ) || t == typeof( SynthNodeOutputStereo ) )
			{
				return new NodeGUIOutput( node );
			}
			else
			{
				return new NodeGUIBase( node );
			}
		}
		#endregion
		
		// Creates a horizontal list of controls with a unified return value
		public static int ButtonList( GUIButtonValue[] buttons )
		{
			int rval = BUTTON_GROUP_NO_INPUT;
			
			GUILayout.BeginVertical();
			
			for ( int i = 0; i < buttons.Length; i++ )
			{
				if ( GUILayout.Button( buttons[ i ].m_title ) )
				{
					rval = buttons[ i ].m_rval;
				}
			}
			
			GUILayout.EndVertical();
			
			return rval;
		}
		
		public static float FloatFieldWithLabel( float value, string label )
		{	
			EditorGUILayout.PrefixLabel( label );
			return EditorGUILayout.FloatField( value );
		}
		
		public static System.Enum EnumFieldWithLabel( System.Enum value, string label )
		{
			EditorGUILayout.PrefixLabel( label );
			return EditorGUILayout.EnumPopup( value );
		}
		
		public static void HorizontalCenteredLabel( Rect rect, string text )
		{
			GUILayout.BeginArea( rect );
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( text );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndArea();		
		}
		
		public static int ConnectorHorizontalGroup( Rect controlRect, int count )
		{
			int rval = BUTTON_GROUP_NO_INPUT;
			
			GUILayout.BeginArea( controlRect );
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			for ( int i = 0; i < count; i++ )
			{
				if ( GUILayout.Button("", GUILayout.Width( NODE_CONNECTOR_SIZE ), GUILayout.Height( NODE_CONNECTOR_SIZE ) ) )
				{
					rval = i;
				}
				
				GUILayout.FlexibleSpace();
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			return rval;
		}
	}
}