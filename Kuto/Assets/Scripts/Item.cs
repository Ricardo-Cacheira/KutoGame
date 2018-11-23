using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class Item : ScriptableObject {

	[SerializeField] string id;
	public string ID { get { return id; } }
	
	public string name;
	public Sprite icon;

	#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		string path = AssetDatabase.GetAssetPath(this);
		id = AssetDatabase.AssetPathToGUID(path);
	}
	#endif

	public virtual Item GetCopy()
	{
		return this;
	}

	public virtual void Destroy()
	{

	}
}
