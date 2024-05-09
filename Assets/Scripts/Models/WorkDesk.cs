using System.Collections;
using DG.Tweening;
using UnityEngine;
using DemoGame.Interactions;

namespace DemoGame
{
    /// <summary>
    /// Working Process after submitting the papers
    /// </summary>
    public class WorkDesk : MonoBehaviour, IInteractable
    {
        public Animator female_anim;
        [SerializeField] private Transform MoneyStorage;
        [SerializeField] private Transform DollarPlace;
        [SerializeField] private GameObject Dollar;
        private float YAxis;
        private IEnumerator makeMoneyIE;

        private void Start()
        {
            makeMoneyIE = ReturnOutput();
        }

        public void ProceedInput()
        {
            female_anim.SetBool(AppConstants.tag_PAnimWork, true);

            InvokeRepeating("DOSubmitPapers", 2f, 1f);

            StartCoroutine(makeMoneyIE);
        }

        public IEnumerator ReturnOutput()
        {
            var counter = 0;
            var DollarPlaceIndex = 0;

            yield return new WaitForSecondsRealtime(2);

            while (counter < transform.childCount)
            {
                GameObject NewDollar = Instantiate(Dollar, new Vector3(DollarPlace.GetChild(DollarPlaceIndex).position.x,
                        YAxis, DollarPlace.GetChild(DollarPlaceIndex).position.z),
                    DollarPlace.GetChild(DollarPlaceIndex).rotation);

                NewDollar.transform.DOScale(new Vector3(0.4f, 0.4f, 0.6f), 0.5f).SetEase(Ease.OutElastic);
                NewDollar.transform.SetParent(MoneyStorage);
                if (DollarPlaceIndex < DollarPlace.childCount - 1)
                {
                    DollarPlaceIndex++;
                }
                else
                {
                    DollarPlaceIndex = 0;
                    YAxis += 0.5f;
                }

                yield return new WaitForSecondsRealtime(3f);
            }
        }

        void DOSubmitPapers()
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(transform.childCount - 1).gameObject, 1f);
            }
            else
            {
                female_anim.SetBool(AppConstants.tag_PAnimWork, false);

                var Desk = transform.parent;

                Desk.GetChild(Desk.childCount - 1).GetComponent<Renderer>().enabled = true;

                StopCoroutine(makeMoneyIE);

                YAxis = 0f;
            }
        }
        //public void PickUp()
        //{

        //}
    }
}
