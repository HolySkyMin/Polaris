using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letterbox : MonoBehaviour
{
	public RawImage rawImage;
	public Material boxMat;
	public int baseWidth = 10;

	void Start()
	{
		rawImage.color = Color.white;
		Fit();
	}

	// Update is called once per frame
	void Update()
	{
		Fit();
	}

	public void Fit()
	{
		float ratio = (float)Screen.height / Screen.width;
		rawImage.uvRect = new Rect(0, 0, baseWidth, baseWidth * ratio);

		boxMat.SetFloat("_Width", Screen.width);
		boxMat.SetFloat("_Height", Screen.height);
		boxMat.SetFloat("_MaxUVWidth", rawImage.uvRect.width);
		boxMat.SetFloat("_MaxUVHeight", rawImage.uvRect.height);
	}
}
