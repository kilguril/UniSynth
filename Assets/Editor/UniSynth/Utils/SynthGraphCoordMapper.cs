using UnityEngine;
using System.Collections;

namespace UniSynth2.Editor.Utils
{
	public class SynthGraphCoordMapper
	{
		float m_fromMin;
		float m_fromMax;
		float m_toMin;
		float m_toMax;
		
		float m_fromSpan;
		float m_toSpan;
		
		bool  m_clamp;
	
		public SynthGraphCoordMapper( float fromMin, float fromMax, float toMin, float toMax, bool clamp )
		{
			m_fromMin = fromMin;
			m_fromMax = fromMax;
			
			m_toMin   = toMin;
			m_toMax	  = toMax;
			
			m_clamp   = clamp;
			
			m_toSpan   = m_toMax   - m_toMin;
			m_fromSpan = m_fromMax - m_fromMin;
		}
		
		public float MapFromTo( float val )
		{
			if ( m_clamp )
			{
				val = Mathf.Clamp( val, m_fromMin, m_fromMax );
			}
			
			val = ( val - m_fromMin ) / m_fromSpan;
			
			return m_toMin + ( val * m_toSpan );
		}
	}
}