using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UniSynth2;
using UniSynth2.Editor;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorGraphWindow : SynthGraphEditorWindow
	{
		private bool		m_dirty;
		
		private SoundClip	m_cachedClip;
	
		private Dictionary< ISynthGraphNode, NodeGUIBase > m_graphCache;
		private Dictionary< ISynthGraphNode, NodeGUIBase > m_floatingSelection;	// Unconnected nodes
	
		public SynthGraphEditorGraphWindow( int windowId, string windowTitle, Rect windowRect ) : base( windowId, windowTitle, windowRect )
		{
			m_graphCache		= new Dictionary<ISynthGraphNode, NodeGUIBase>();
			m_floatingSelection = new Dictionary<ISynthGraphNode, NodeGUIBase>();
			
			SetDirty( true );
		}
	
		protected override void Window (int windowId)
		{
			SoundClip clip = SynthGraphEditorState.ActiveClip;
					
			if ( ( clip != m_cachedClip ) || ( m_dirty ) )
			{
				if ( clip != null )
				{
					RebuildNodeGUICache( clip );
				}
			}
			
			foreach( NodeGUIBase control in m_graphCache.Values )
			{
				control.Draw();
			}
			
			foreach( NodeGUIBase control in m_floatingSelection.Values )
			{
				control.Draw();
			}
		}	
		
		public void AddNode( ISynthGraphNode node )
		{
			m_floatingSelection.Add( node, SynthGraphEditorFactory.GetGUIControl( node ) );
		}
		
		private void SetDirty( bool dirty )
		{
			m_dirty = dirty;
		}
		
		private void RebuildNodeGUICache( SoundClip clip )
		{
			Debug.Log ("Rebuilding node cache!");
			m_graphCache.Clear();
		
			CacheNode( clip.Root );	
			
			m_cachedClip = clip;
			SetDirty( false );
		}	
		
		private void CacheNode( ISynthGraphNode node )
		{
			m_graphCache.Add( node, SynthGraphEditorFactory.GetGUIControl( node ) );
			
			ISynthGraphNode[] inputs = node.GetSourceNodes();
			
			for( int i = 0; i < inputs.Length; i++ )
			{
				if ( inputs[ i ] != null )
				{
					CacheNode( inputs[ i ] );
				}
			}
		}
	}
}