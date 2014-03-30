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
		public const float		WINDOW_HEADER_H = 17.0f;
		
		public const float		NODE_WIDTH			   = 150.0f;
		public const float		NODE_LINE_HEIGHT	   = 17.0f;
		
		public const float		SPLIT_VIEW_HANDLE_SIZE = 20.0f;
		public const float		SPLIT_VIEW_RANGE_MIN   = 0.01f;
		public const float		SPLIT_VIEW_RANGE_MAX   = 0.99f;
	
		// Get GUI control for node
		public static NodeGUIBase GetGUIControl( ISynthGraphNode node )
		{
			return new NodeGUIBase( node );
		}
		
		// Creates a horizontal list of controls with a unified return value
		public static int ButtonList( GUIButtonValue[] buttons )
		{
			int rval = -1;
			
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
	}
}