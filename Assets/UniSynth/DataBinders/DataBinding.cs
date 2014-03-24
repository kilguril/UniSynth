using UnityEngine;
using System.Collections;

namespace UniSynth
{
	public class DataBinding
	{
		public enum BindingMode
		{
			VALUE,
			MAP_RANGE
		}
		
		protected ObservableProperty< float > m_source;
		protected BindingMode					m_mode;
		
		protected float						m_sourceFrom;
		protected float						m_sourceTo;
		protected float						m_targetFrom;
		protected float						m_targetTo;
		
		public void SetSource( ObservableProperty< float > source )
		{
			m_source = source;
			m_source.OnPropertyChanged += HandleSourcePropertyChanged;
		}
		
		private void HandleSourcePropertyChanged( float val )
		{
			float output = 0.0f;
			
			switch( m_mode )
			{
			case BindingMode.VALUE:
				output = Mathf.Clamp( val, m_sourceFrom, m_sourceTo );
				break;
				
			case BindingMode.MAP_RANGE:
				output = Mathf.Clamp( val, m_sourceFrom, m_sourceTo );
				output -= m_sourceFrom;
				
				output /= ( m_sourceTo - m_sourceFrom );
				
				output *= ( m_targetTo - m_targetFrom );
				output += m_targetFrom;
				
				break;
			}
			
			ApplyChange( output );
		}
		
		protected virtual void ApplyChange( float val ) { }
	}
}