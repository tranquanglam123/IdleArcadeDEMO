using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
namespace DemoGame
{


    public class UnlockDesk : MonoBehaviour
    {
        [SerializeField] GameObject unlockProgressObj;
        [SerializeField] GameObject newDesk;
        [SerializeField] Image progressBar;
        [SerializeField] TextMeshProUGUI dollarAmount;
        [SerializeField] int deskPrice, deskRemainPrice;
        [SerializeField] float ProgressValue;
        public NavMeshSurface buildNavMesh;

        void Start()
        {
            dollarAmount.text = deskPrice.ToString(AppConstants.tag_DefaultPrice);
            deskRemainPrice = deskPrice;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(AppConstants.tag_Player) && PlayerPrefs.GetInt(AppConstants.tag_Currency) > 0)
            {
                ProgressValue = Mathf.Abs(1f - CalculateMoney() / deskPrice);

                if (PlayerPrefs.GetInt(AppConstants.tag_Currency) >= deskPrice)
                {
                    PlayerPrefs.SetInt(AppConstants.tag_Currency, PlayerPrefs.GetInt(AppConstants.tag_Currency) - deskPrice);

                    deskRemainPrice = 0;
                }
                else
                {
                    deskRemainPrice -= PlayerPrefs.GetInt(AppConstants.tag_Currency);
                    PlayerPrefs.SetInt(AppConstants.tag_Currency, 0);
                }

                progressBar.fillAmount = ProgressValue;

                GameManager.instance.MoneyCounter.text = PlayerPrefs.GetInt(AppConstants.tag_Currency).ToString(AppConstants.tag_DefaultPrice);
                dollarAmount.text = deskRemainPrice.ToString(AppConstants.tag_DefaultPrice);

                if (deskRemainPrice == 0)
                {
                    GameObject desk = Instantiate(newDesk, new Vector3(transform.position.x, 2.2f, transform.position.z)
                        , Quaternion.Euler(0f, -90f, 0f));

                    desk.transform.DOScale(1.1f, 1f).SetEase(Ease.OutElastic);
                    desk.transform.DOScale(1f, 1f).SetDelay(1.1f).SetEase(Ease.OutElastic);

                    unlockProgressObj.SetActive(false);

                    buildNavMesh.BuildNavMesh();
                }

            }
        }

        private float CalculateMoney()
        {
            return deskRemainPrice - PlayerPrefs.GetInt(AppConstants.tag_Currency);
        }
    }
}
