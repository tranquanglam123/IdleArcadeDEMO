using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DemoGame.Interactions;
using DG.Tweening;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DemoGame
{
    /// <summary>
    /// Player Movement, Interaction Management
    /// </summary>

    public class Player : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private float playerSpeed;
        [SerializeField] private Transform paperPlace;
        public int MoneyValue;

        //Player Settings
        private Camera Cam;
        private Animator PlrAnim;
        private float YAxis, delay;

        //Movement
        private bool canMove = true;
        private Vector3 direction;

        //Interactions
        private List<Transform> papers = new();

        //GameControl
        private bool submitPapers = false;


        void Start()
        {
            Cam = Camera.main;
            PlrAnim = GetComponent<Animator>();
            papers.Add(paperPlace);

        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HandleMovement();

            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleMovingAnimation();
            }

            if (Input.GetMouseButtonUp(0))
            {
                HandleStopAnimation();

            }


            if (Physics.Raycast(transform.position, transform.forward, out var hit, 1f))
            {
                Debug.DrawRay(transform.position, transform.forward * 1f, Color.green);

                HandleInteraction(hit);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * 1f, Color.red);

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(AppConstants.tag_PaperPlace))
            {
                InteractWithObject(other.GetComponent<WorkDesk>().gameObject);
            }

            if (other.CompareTag(AppConstants.tag_Currency))
            {
                PickUpObject(other.gameObject);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(AppConstants.tag_PaperPlace))
            {
                HandleMovingAnimation();
                delay = 0f;
            }

            if (other.CompareTag(AppConstants.tag_Table))
            {
                HandleMovingAnimation();
            }
        }
        void InteractWithObject(GameObject objectToInteract)
        {
            if (objectToInteract.TryGetComponent(out IInteractable interactableObject))
            {
                interactableObject.ProceedInput();
            }
        }

        void PickUpObject(GameObject objectToPickup)
        {
            var par = objectToPickup.transform.parent;
            if (par != null)
            {
                if (par.TryGetComponent(out IPickupable pickupable))
                {
                    pickupable.PickUp(objectToPickup);
                }
            }
        }

        protected virtual void HandleMovement()
        {
            if (canMove)
            {
                Plane plane = new Plane(Vector3.up, transform.position);
                Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out var distance))
                    direction = ray.GetPoint(distance);

                transform.position = Vector3.MoveTowards(transform.position, new Vector3(direction.x, 0f, direction.z),
                    playerSpeed * Time.deltaTime);

                var offset = direction - transform.position;

                if (offset.magnitude > 1f)
                    transform.LookAt(direction);
            }
        }

        protected virtual void HandleMovingAnimation()
        {
            if (papers.Count > 1)
            {
                PlrAnim.SetBool(AppConstants.tag_PAnimCarry, false);
                PlrAnim.SetBool(AppConstants.tag_PAnimRunWPapers, true);
            }
            else
            {
                PlrAnim.SetBool(AppConstants.tag_PAnimRun, true);
            }
        }

        protected virtual void HandleStopAnimation()
        {
            PlrAnim.SetBool(AppConstants.tag_PAnimRun, false);

            if (papers.Count > 1)
            {
                PlrAnim.SetBool(AppConstants.tag_PAnimCarry, true);
                PlrAnim.SetBool(AppConstants.tag_PAnimRunWPapers, false);
            }
        }

        protected virtual void HandleInteraction(RaycastHit hit)
        {
            if (hit.collider.CompareTag(AppConstants.tag_Table) && papers.Count < 21)
            {
                if (hit.collider.transform.childCount > 2)
                {
                    var paper = hit.collider.transform.GetChild(1);
                    //paper.rotation = Quaternion.Euler(paper.rotation.x, Random.Range(0f, 180f), paper.rotation.z);
                    if (papers.Count == 1)
                    {
                        submitPapers = false;
                        paper.position = new Vector3(paperPlace.transform.position.x, paperPlace.transform.position.y + 0.17f, paperPlace.transform.position.z);
                        paper.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (papers.Count >= 2)
                    {
                        submitPapers = true;
                        Transform pos = papers[^1];
                        paper.position = new Vector3(pos.position.x, pos.position.y + 0.17f, pos.position.z);
                        paper.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    papers.Add(paper);
                    paper.parent = paperPlace;



                    if (hit.collider.transform.parent.GetComponent<Printer>().CountPapers > 1)
                        hit.collider.transform.parent.GetComponent<Printer>().CountPapers--;

                    if (hit.collider.transform.parent.GetComponent<Printer>().YAxis > 0f)
                        hit.collider.transform.parent.GetComponent<Printer>().YAxis -= 0.17f;

                    PlrAnim.SetBool(AppConstants.tag_PAnimCarry, true);
                    PlrAnim.SetBool(AppConstants.tag_PAnimRun, false);
                }
            }

            if (hit.collider.CompareTag(AppConstants.tag_PaperPlace) && papers.Count > 1)
            {
                var WorkDesk = hit.collider.transform;

                if (WorkDesk.childCount > 0)
                {
                    YAxis = WorkDesk.GetChild(WorkDesk.childCount - 1).position.y;
                }
                else
                {
                    YAxis = WorkDesk.position.y;
                }

                for (var index = papers.Count - 1; index >= 1; index--)
                {
                    papers[index].DOJump(new Vector3(WorkDesk.position.x, YAxis, WorkDesk.position.z), 2f, 1, 0.2f)
                        .SetDelay(delay).SetEase(Ease.Flash);

                    papers.ElementAt(index).parent = WorkDesk;
                    papers.RemoveAt(index);

                    YAxis += 0.17f;
                    delay += 0.02f;
                }

                WorkDesk.parent.GetChild(WorkDesk.parent.childCount - 1).GetComponent<Renderer>().enabled = false;

                if (papers.Count <= 1)
                {
                    PlrAnim.SetBool(AppConstants.tag_PAnimIdle, true);
                    PlrAnim.SetBool(AppConstants.tag_PAnimRunWPapers, false);
                }

            }


        }
        public bool SubmitSounds()
        {
            return submitPapers;
        }
        public bool StopPlayingSubmitSounds()
        {
            submitPapers = false;
            return submitPapers;
        }

    }
}
