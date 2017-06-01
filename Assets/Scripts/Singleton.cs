using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected Singleton() {}

	private static T _instance;
	private static object _lock = new object();

	public static T instance
	{
		get
		{
			if (isApplicationQuitting)
				return null;

			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T>();

					if (_instance == null)
						_instance = new GameObject(typeof(T).ToString()).AddComponent<T>();

					DontDestroyOnLoad(_instance.gameObject);
				}

				return _instance;
			}
		}
	}

	private static bool isApplicationQuitting = false;

	public void OnDestroy()
	{
		isApplicationQuitting = true;
	}
}
