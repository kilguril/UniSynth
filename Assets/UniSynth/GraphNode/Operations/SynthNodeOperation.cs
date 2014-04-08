using UnityEngine;
using System.Collections;


namespace UniSynth2
{
	public class SynthNodeOperation : SynthGraphNodeBase 
	{
#region static methods
		private delegate float Operation( float lhs, float rhs );
	
		private static float OpAdd( float lhs, float rhs )
		{
			return lhs + rhs;
		}
		
		private static float OpSubtract( float lhs, float rhs )
		{
			return lhs - rhs;
		}
		
		private static float OpMultiply( float lhs, float rhs )
		{
			return lhs * rhs;
		}
		
		private static float OpDivide( float lhs, float rhs )
		{
			return lhs / rhs;
		}
#endregion
		
		public enum WaveOperation
		{
			ADD,
			SUBTRACT,
			MULTIPLY,
			DIVIDE
		}		
		
		public WaveOperation m_operation;
	
		public SynthNodeOperation()
		{
			m_sourceNodes = new ISynthGraphNode[2];
		}
	
		public override void Process (float[] data, SoundClipState state)
		{
			// TODO: Optimize buffer usage
			float[] lhs = new float[ data.Length ];
			float[] rhs = new float[ data.Length ];
			
			if ( m_sourceNodes[ 0 ] != null )
			{
				m_sourceNodes[ 0 ].Process( lhs, state );
			}
			
			if ( m_sourceNodes[ 1 ] != null )
			{
				m_sourceNodes[ 1 ].Process( rhs, state );
			}
			
			Operation op = OpAdd;
			
			switch( m_operation )
			{
				case WaveOperation.ADD:
					op = OpAdd;
				break;
				
				case WaveOperation.SUBTRACT:
					op = OpSubtract;
				break;
				
				case WaveOperation.MULTIPLY:
					op = OpMultiply;
				break;
				
				case WaveOperation.DIVIDE:
					op = OpDivide;
				break;
			}
		
			for( int i = 0; i < data.Length; i++ )
			{
				data[ i ] = op( lhs[ i ], rhs[ i ] );
			}
		}
	}
}