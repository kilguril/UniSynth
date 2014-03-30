using UnityEngine;
using System.Collections;

namespace UniSynth
{
	public class WaveKeyframeDataBinding : DataBinding
	{
		public enum BoundProperty
		{
			TIME,
			FREQUENCY,
			GAIN
		}
				
		private WaveKeyframe			    m_target;
		private BoundProperty				m_property;
		
		public WaveKeyframeDataBinding( BoundProperty boundProperty, BindingMode bindingMode, float srcFrom, float srcTo, float targetFrom, float targetTo )
		{
			m_property 		= boundProperty;
			m_mode	   		= bindingMode;
			
			m_sourceFrom 	= srcFrom;
			m_sourceTo 		= srcTo;
			
			m_targetFrom	= targetFrom;
			m_targetTo		= targetTo;
		}
		
		public void SetTarget( WaveKeyframe target )
		{
			m_target = target;
		}
		
		protected override void ApplyChange( float val )
		{
			switch( m_property )
			{
				case BoundProperty.FREQUENCY:
					m_target.m_frequency = val;
					break;
					
				case BoundProperty.GAIN:
					m_target.m_gain = val;
					break;
					
				case BoundProperty.TIME:
					m_target.m_time = val;
					break;
			}
		}
	}
}
