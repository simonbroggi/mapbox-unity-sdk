namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Unity.Map;
	using System;

	public class StaticMapReference : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		bool _dontDestroyOnLoad;

		public Action MapInitialized;

		private static StaticMapReference _instance;
		public static StaticMapReference Instance
		{
			get
			{
				return _instance;
			}

			private set
			{
				_instance = value;
			}
		}

		private void Awake()
		{
			if (Instance != null)
			{
				DestroyImmediate(gameObject);
				return;
			}

			Instance = this;

			Action handler = null;
			handler = () =>
			{
				MapInitialized();
				_map.OnInitialized -= handler;
			};

			_map.OnInitialized += handler;

			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		public AbstractMap ReturnMap()
		{
			return _map;
		}
	}
}
