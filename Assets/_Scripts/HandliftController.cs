using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class HandliftController : MonoBehaviour
{
    public struct PositionAndRotation
    {
        public Vector3 position;
        public Quaternion rotation;

        public void FromTransform(Transform transform)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
        }

        public Transform GetTransform()
        {
            GameObject forTransform = new GameObject();

            forTransform.transform.position = this.position;
            forTransform.transform.rotation = this.rotation;

            return forTransform.transform;
        }
    }

    public bool showDebug = true;

    public bool enabledHandliftController = true;
    [Header("Generals")]
    public Transform palettHandler;
    public Transform selectorTransform;
    [Header("Settings")]
    public float rotateSpeed = 3;
    public float joiningSpeed = 2;
    public float howFarJoinToPalette = 1.95f;


    Camera cam;
    byte status = 0;

    NavMeshAgent agent;
    public PaletteController currentPalette = null;

    PositionAndRotation target = new PositionAndRotation();

    void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    }

    void LateUpdate()
    {
        if (!enabledHandliftController)
            return;

        if (GameController.isGamePaused)
            return;

        switch (status)
        {
            case 0: //idle
                break;

            case 1: //go to free pos
                if (transform.position.x == target.position.x && transform.position.z == target.position.z)
                {
                    if (selectorTransform != null)
                        selectorTransform.gameObject.SetActive(false);

                    if (palettHandler.childCount == 0)
                    {
                        if (currentPalette != null)
                            currentPalette = null;
                    }
                    status = 0;
                }
                if (showDebug && status != 1)
                    Debug.Log(1 + " zmiana na " + status);
                break;

            case 10: //go to palett
                //Debug.Log(transform.position.x +":"+targetPos.x +" - "+ transform.position.z + ":" + targetPos.z);

                //if (transform.position.x == target.position.x && transform.position.z == target.position.z)
                if (!agent.pathPending && !agent.hasPath && agent.remainingDistance == 0 && agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    if (selectorTransform != null)
                        selectorTransform.gameObject.SetActive(false);

                    status = 11;

                    //Debug.Log(targetRotation.eulerAngles + ": - :"+ transform.rotation.eulerAngles);

                    agent.isStopped = true;
                    agent.ResetPath();
                }
                if (showDebug && status != 10)
                    Debug.Log(10 + " zmiana na " + status);
                break;

            case 11: //rotate to palett
                //agent.isStopped = true;
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotateSpeed * Time.deltaTime);

                if (transform.rotation == target.rotation || Mathf.Abs(Quaternion.Angle(target.rotation, transform.rotation)) < 1)
                {
                    if (currentPalette.canBeTaken && palettHandler.childCount == 0)
                    {
                        agent.enabled = false;
                        currentPalette.navMeshObstacle.enabled = false;

                        target.position = transform.forward * (howFarJoinToPalette - palettHandler.localPosition.z) + transform.position;

                        status = 12;
                    }
                    else
                    {
                        //Debug.Log(currentPalette.name);
                        status = 0;
                    }
                }
                if (showDebug && status != 11)
                    Debug.Log(11 + " zmiana na " + status);
                break;

            case 12: //drive inside and bring palett
                transform.position = Vector3.MoveTowards(transform.position, target.position, joiningSpeed * Time.deltaTime);

                if (transform.position.x == target.position.x && transform.position.z == target.position.z)
                {
                    if (currentPalette.canBeTaken)
                    {
                        currentPalette.transform.parent = palettHandler;
                        currentPalette.transform.localPosition = new Vector3(0, 0, 0);
                        currentPalette.canBeTaken = false;
                        agent.enabled = true;

                        agent.isStopped = false;

                        status = 0;
                    }
                    else
                    {
                        status = 0;
                    }
                }
                if (showDebug && status != 12)
                    Debug.Log(12 + " zmiana na " + status);
                break;

            case 20: //go to leaveIt pos
                if (Vector3.Distance(transform.position, target.position) < 0.5f)
                {
                    if (selectorTransform != null)
                        selectorTransform.gameObject.SetActive(false);

                    if (palettHandler.childCount == 0)
                    {
                        if (currentPalette != null)
                            currentPalette = null;

                        status = 0;
                    }
                    else
                    {
                        agent.isStopped = true;
                        agent.ResetPath();

                        agent.enabled = false;

                        target.position = -transform.forward * howFarJoinToPalette + transform.position;
                        currentPalette.transform.parent = null;
                        currentPalette.transform.position = new Vector3(currentPalette.transform.position.x, 0, currentPalette.transform.position.z);

                        status = 21;
                    }
                }
                if (showDebug && status != 20)
                    Debug.Log(20 + " zmiana na " + status);
                break;

            case 21: //leave pallet
                transform.position = Vector3.MoveTowards(transform.position, target.position, joiningSpeed * Time.deltaTime);

                if (transform.position.x == target.position.x && transform.position.z == target.position.z)
                {
                    agent.enabled = true;
                    currentPalette.navMeshObstacle.enabled = true;
                    currentPalette.canBeTaken = true;

                    currentPalette = null;

                    agent.isStopped = false;

                    status = 0;
                }
                if (showDebug && status != 21)
                    Debug.Log(21 + " zmiana na " + status);
                break;
        }

        if (transform.hasChanged)
        {
            cam.transform.position = transform.position - Vector3.forward * 5 + Vector3.up * 15;
        }



        if (GameController.currentDialogue != null || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(
#if !UNITY_EDITOR
    0
#else
    -1
#endif
    ))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (status == 0 || status == 1 || status == 10 || status == 11 || status == 20)
            {
                if (selectorTransform != null)
                    selectorTransform.gameObject.SetActive(false);

                agent.isStopped = true;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "PlaceArea")
                    {
                        target.position = hit.point;

                        if (selectorTransform != null)
                        {
                            selectorTransform.gameObject.SetActive(true);
                            selectorTransform.position = hit.point;
                        }

                        agent.SetDestination(target.position);
                        agent.isStopped = false;

                        status = 20;
                    }
                    else if (hit.transform.CompareTag("WalkableGround"))
                    {
                        target.position = hit.point;

                        if (selectorTransform != null)
                        {
                            selectorTransform.gameObject.SetActive(true);
                            selectorTransform.position = hit.point;
                        }

                        agent.SetDestination(target.position);
                        agent.isStopped = false;

                        status = 1;
                    }
                    else if (hit.transform.CompareTag("Palette") || (hit.transform.parent != null && hit.transform.parent.CompareTag("PaletteObject")))
                    {
                        PaletteController tempPalette;
                        tempPalette = hit.transform.GetComponent<PaletteController>();
                        if (tempPalette == null)
                        {
                            tempPalette = hit.transform.parent.parent.parent.GetComponent<PaletteController>();
                        }

                        if (tempPalette != null)
                        {
                            if (tempPalette.canBeTaken && tempPalette.takePositions.Length > 0)
                            {
                                Transform bestPosition = null;

                                bestPosition = tempPalette.transform;
                                float bestDistance = -1;

                                foreach (Transform takePosition in tempPalette.takePositions)
                                {
                                    //agent.ResetPath();
                                    if (bestDistance == -1)
                                    {
                                        agent.SetDestination(takePosition.position);
                                        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                                        {
                                            bestDistance = Vector3.Distance(transform.position, takePosition.position); //agent.remainingDistance;
                                            bestPosition = takePosition;
                                        }
                                    }
                                    else
                                    {
                                        agent.SetDestination(takePosition.position);
                                        if (bestDistance > Vector3.Distance(transform.position, takePosition.position) && agent.pathStatus == NavMeshPathStatus.PathComplete)
                                        {
                                            bestDistance = Vector3.Distance(transform.position, takePosition.position); //agent.remainingDistance;
                                            bestPosition = takePosition;
                                        }
                                    }

                                    if (bestDistance == 0) break;
                                }

                                target.FromTransform(bestPosition);


                                //agent.ResetPath();

                                agent.isStopped = false;
                                agent.SetDestination(target.position);

                                if (selectorTransform != null)
                                {
                                    selectorTransform.gameObject.SetActive(true);
                                    selectorTransform.position = target.position;
                                }

                                if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                                {
                                    if (palettHandler.childCount == 0)
                                    {
                                        currentPalette = tempPalette;

                                        status = 10;
                                    }
                                    else
                                    {
                                        status = 1;
                                    }
                                }
                                else
                                {
                                    agent.ResetPath();
                                }
                            }
                        }
                        else
                        {
                            if (showDebug)
                                Debug.Log("not found script");
                        }
                    }
                    else
                    {
                        if (showDebug)
                            Debug.Log(hit.transform.tag + ": " + hit.collider.name);
                    }
                }
            }
        }
    }
}