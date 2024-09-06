using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Observe
{
	public class ObsSkyAreaBtn : MonoBehaviour
	{
		public int index;
		public int price;
		public int memorialPrice;
		[TextArea] public string openRule;
		public Text priceLabel;
		public Text openRuleLabel;
		public GameObject pricePanel;
		public GameObject openRulePanel;
		public ObserveManager manager;

		void Start()
		{
			priceLabel.text = price.ToString();
			openRuleLabel.text = openRule;

			pricePanel.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
		}

		public void Clicked()
		{
			StartCoroutine(Clicked_Routine());
		}

		IEnumerator Clicked_Routine()
		{
			yield return MessageSet.Now.ShowMoneySpendAsk("해당 구역을 개방할까요?", price, memorialPrice, result =>
			{
				if (result)
				{
					var starlightPayed = GameManager.Instance.PayMoney(MoneyType.Starlight, price);
					var memorialPayed = GameManager.Instance.PayMoney(MoneyType.MemorialPiece, memorialPrice);
					if (starlightPayed && memorialPayed)
						manager.UnlockSky(index);
					else
					{
						if (!starlightPayed)
							MessageSet.Now.ShowNoMoneyAlert(MoneyType.Starlight);
						else
							MessageSet.Now.ShowNoMoneyAlert(MoneyType.MemorialPiece);
					}
						
				}
			});
		}
	}
}