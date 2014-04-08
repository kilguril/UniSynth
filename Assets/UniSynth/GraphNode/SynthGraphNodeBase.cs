using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public class SynthGraphNodeBase : ISynthGraphNode
	{		
		protected ISynthGraphNode[] m_sourceNodes;
		
		public SynthGraphNodeBase()
		{
			m_sourceNodes = new ISynthGraphNode[0];
		}
		
		public int GetSourceCount ()
		{
			if ( m_sourceNodes == null )
			{
				return 0;
			}
			
			return m_sourceNodes.Length;
		}
		
		public ISynthGraphNode[] GetSourceNodes ()
		{
			return m_sourceNodes;	
		}

		public void SetSourceNode (ISynthGraphNode node, int index)
		{
			if ( index < 0 || index >= m_sourceNodes.Length )
			{
				// Assert
				return;
			}
			
			m_sourceNodes[ index ] = node;
		}

		public virtual void Process (float[] data, SoundClipState state)
		{
			for( int i = 0; i < m_sourceNodes.Length; i++ )
			{
				m_sourceNodes[ i ].Process( data, state );
			}
		}	
	}
}