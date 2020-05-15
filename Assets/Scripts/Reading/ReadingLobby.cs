﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
	public class ReadingLobby : MonoBehaviour
	{
		public Image selectedCharImg;
		public Text selectedCharName;
		public ReadingCharPicker charPicker;
        public GameObject tutorialObj;

		int selectedChar;

		// Use this for initialization
		void Start()
		{
			charPicker.LoadCharacter();

			LoadSelectedCharacter(1);

            if (!Variables.TutorialFinished)
                tutorialObj.SetActive(true);
		}

		public void SelectCharacter()
		{
			StartCoroutine(SelectCharacterInternal());
		}

		public IEnumerator SelectCharacterInternal()
		{
			yield return charPicker.Show(LoadSelectedCharacter);
		}

		public void LoadSelectedCharacter(int index)
		{
			selectedChar = index;
			selectedCharImg.sprite = Resources.Load<Sprite>("Characters/" + Variables.Characters[index].InternalName + "/image_album");
			selectedCharName.text = Variables.Characters[index].Name;
		}

		public void StartReading()
		{
			Dialogue.DialogueManager.DialogRoot = "Characters/" + Variables.Characters[selectedChar].InternalName + "/";
			SceneChanger.Instance.ChangeScene("ReadingIngame");
		}
	}
}