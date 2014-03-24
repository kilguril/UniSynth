using UnityEngine;
using System.Collections;

namespace UniSynth
{
	public class ObservableProperty< T >
	{
		public delegate void PropertyChanged( T val );
		public event PropertyChanged OnPropertyChanged;
	
		public T Value
		{
			get { return m_value; }
			set { 
					m_value = value;
					
					if ( OnPropertyChanged != null )
					{
						OnPropertyChanged( value );
					}
				}
		}
		
		private T m_value;
		
		public ObservableProperty( T defaultValue )
		{
			m_value = defaultValue;
		}
	}
}