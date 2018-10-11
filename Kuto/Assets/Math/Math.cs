using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Math : MonoBehaviour {

	int[,] matrix = new int[,]
	{
		{1, 0},
		{0, 1},
	};

	Matrix4x4 mat4 = Matrix4x4.identity;

	Mesh mesh;
	Vector3[] vertices, originalVertices;
	public Slider x,y,z;
	public Text xText,yText,zText;
	bool changed;

    void Start()
    {
		mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		originalVertices = mesh.vertices;
		changed = false;
		x.onValueChanged.AddListener(delegate {updateScale(); });
		y.onValueChanged.AddListener(delegate {updateScale(); });
		z.onValueChanged.AddListener(delegate {updateScale(); });
	}
	
	// Update is called once per frame
	void Update () {
		

		if (changed)
		{
			mat4 = Matrix4x4.identity;
			mat4[0,0] = x.value;
			mat4[1,1] = y.value;
			mat4[2,2] = z.value;
			for (int i = 0; i < originalVertices.Length; i++)
			{
				vertices[i] = mat4 * originalVertices[i];
			}
			changed = false;
		}

		xText.text = x.value.ToString();
		yText.text = y.value.ToString();
		zText.text = z.value.ToString();
        mesh.vertices = vertices;
	}

	void updateScale(){
		changed = true;
	}
}
