using System;

namespace SocketServerSystem
{
	public class Singleton<T>
	{
		static public T Instance { get; private set; }
		public Singleton<T>(T _instance)
			{
				        if (Instance == null)
        {
			      Instance = (T) this;
			}
			}
	}
}
