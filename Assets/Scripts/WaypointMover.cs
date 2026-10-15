using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 6f;
    // public float waitTime = 2f;
    public bool loopWaypoints = false;
    public bool isBuying;
    public bool hasExited, hasStoppedAtStore;

    public Customer customer;
    private SpriteRenderer customerSprite;
    private Transform[] waypoints;
    private int currentWaypointIndex;

    public bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    
    void Start()
    {
        customer = GetComponent<Customer>();
        customerSprite = GetComponent<SpriteRenderer>();
        customerSprite.color = Color.black;
        waypointParent = GameObject.Find("WaypointParent").transform;
        
        ActivateAllWaypoints();
        ChooseEntryExitPoints();
        

        List<Transform> activeWaypoints = new List<Transform>();

        for (int i=0; i<waypointParent.childCount; i++)
        {
            if (waypointParent.GetChild(i).gameObject.activeSelf)
            {
                activeWaypoints.Add(waypointParent.GetChild(i));
            }
        }

        waypoints = activeWaypoints.ToArray();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseController.IsGamePaused || isBuying)
        {
            StopFootsteps();
            return;
        }

        // move to waypoint
        MoveToWaypoint();

    
    }
        
    // there are two exit points
    // the disabled one will be the entry point
    private void ChooseEntryExitPoints()
    {
        GameObject[] exitWaypoints = GameObject.FindGameObjectsWithTag("ExitWP");

        int index = Random.Range(0, exitWaypoints.Length);

        // deactivate one 
        exitWaypoints[index].gameObject.SetActive(false);

        transform.position = exitWaypoints[index].transform.position;

    }

    private void MoveToWaypoint()
    {
        StartFootsteps();
        Transform target = waypoints[currentWaypointIndex];

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // sin to make wave pattern for moving the sprite (like walking)
        transform.position += new Vector3(0f, Mathf.Sin(Time.time * 8f) * 0.001f, 0f);

        if (Vector2.Distance(transform.position, target.position) < 0.001f)
        {
            StopFootsteps();
            // wait at waypoint
            if(currentWaypointIndex == waypoints.Length - 1)
            {
                hasExited = true;
                return;
            }

            if (!hasStoppedAtStore)
            {
                StartCoroutine(DarkenSprite(false));
                StartCoroutine(WaitAtWaypoint());
            }
            // StartCoroutine(DarkenSprite(false));
            // StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        // buying
        isBuying = true;

        hasStoppedAtStore = true;

        customer.StartCustomer();
        // yield return new WaitForSeconds(waitTime);

        // // turn into
        while (isBuying)
        {
            yield return null;
        }

        // done buying
        StartCoroutine(DarkenSprite(true));

        currentWaypointIndex++;

    }
    
    // handles the fading in and fading out of the customer while walking/waiting
    // when walking, customer's sprite is black
    private IEnumerator DarkenSprite(bool turnBlack)
    {
        Color targetColor = turnBlack ? Color.black : Color.white;
        Color color = customerSprite.color;

        while (Vector3.Distance(new Vector3(color.r, color.g, color.b), 
                                new Vector3(targetColor.r, targetColor.g, targetColor.b)) > 0.01f)
        {
            color = Color.Lerp(color, targetColor, Time.deltaTime * 2f);
            color.a = 1f;
            customerSprite.color = color;
            yield return null;
        }        

        targetColor.a = 1f;
        customerSprite.color = targetColor;
    }


    private void ActivateAllWaypoints()
    {
        for(int i=0; i<waypointParent.childCount; i++)
        {
            waypointParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void StartFootsteps()
    {
        if (!playingFootsteps)
        {
            playingFootsteps = true;
            InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
        }
    }

    private void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    private void PlayFootstep()
    {
        SoundFXController.Play("Footsteps", true);
    }


}
